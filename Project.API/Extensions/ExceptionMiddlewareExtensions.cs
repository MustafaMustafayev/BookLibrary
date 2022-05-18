using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Diagnostics;
using Project.Core.Constants;
using Project.DTO.DTOs.Responses;

namespace Project.API.Extensions
{
    public static class ExceptionMiddlewareExtensions
    {
        public static void ConfigureExceptionHandler(this IApplicationBuilder app)
        {
            app.UseExceptionHandler(appError =>
            {
                appError.Run(async context =>
                {
                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    context.Response.ContentType = "application/json";
                    var contextFeature = context.Features.Get<IExceptionHandlerFeature>();
                    if (contextFeature != null)
                    {
                        await context.Response.WriteAsync(JsonSerializer.Serialize(new ErrorDataResult<Result>(Messages.GeneralError)));
                    }
                });
            });
        }
    }
}