using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc.Filters;
using Project.BLL.Abstract;
using Project.Core.Abstract;
using Project.Core.Helper;
using Project.DTO.DTOs.CustomLoggingDTOs;

namespace Project.API.ActionFilters
{
    public class LogActionFilter : IAsyncActionFilter
    {
        private readonly IUtilService _utilService;
        private readonly ILoggingService _loggingService;
        private readonly ConfigSettings _configSettings;

        public LogActionFilter(IUtilService utilService, ILoggingService loggingService, ConfigSettings configSettings)
        {
            _utilService = utilService;
            _loggingService = loggingService;
            _configSettings = configSettings;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var httpContext = context.HttpContext;

            string traceIdentitier = httpContext?.TraceIdentifier;
            string clientIP = httpContext.Features.Get<IHttpConnectionFeature>()?.RemoteIpAddress?.ToString();
            string uri = httpContext.Request.Host + httpContext.Request.Path;

            string token = string.Empty;
            int? userId = null;
            string authHeaderName = _configSettings.AuthSettings.HeaderName;

            if (!string.IsNullOrEmpty(httpContext.Request.Headers[authHeaderName]) && httpContext.Request.Headers[authHeaderName].ToString().Length > 7)
            {
                token = httpContext.Request.Headers[authHeaderName].ToString();
                userId = !string.IsNullOrEmpty(token) ? _utilService.GetUserIdFromToken(httpContext.Request.Headers[authHeaderName].ToString()) : null;
            }

            context.HttpContext.Request.Body.Position = 0;
            using var streamReader = new StreamReader(context.HttpContext.Request.Body);
            string bodyContent = await streamReader.ReadToEndAsync();
            context.HttpContext.Request.Body.Position = 0;

            RequestLogDTO requestLog = new RequestLogDTO()
            {
                TraceIdentifier = traceIdentitier,
                ClientIP = clientIP,
                URI = uri,
                Payload = bodyContent,
                Method = httpContext.Request.Method,
                Token = token,
                UserId = userId,
                RequestDate = DateTime.Now
            };

            await next();

            int responseStatusCode = httpContext.Response.StatusCode;
            ResponseLogDTO responseLog = new ResponseLogDTO()
            {
                TraceIdentifier = traceIdentitier,
                ResponseDate = DateTime.Now,
                StatusCode = responseStatusCode.ToString(),
                Token = token,
                UserId = userId
            };

            requestLog.ResponseLog = responseLog;
            await _loggingService.AddLogAsync(requestLog);
        }
    }
}