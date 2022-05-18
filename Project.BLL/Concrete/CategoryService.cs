using AutoMapper;
using Project.BLL.Abstract;
using Project.Core.Constants;
using Project.DAL.UnitOfWorks.Abstract;
using Project.DTO.DTOs.CategoryDTOs;
using Project.DTO.DTOs.Responses;
using Project.Entity.Entities;

namespace Project.BLL.Concrete
{
    public class CategoryService : ICategoryService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CategoryService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IDataResult<List<CategoryToListDTO>>> GetAsync()
        {
            List<Category> categories = await _unitOfWork.CategoryRepository.GetListAsync();
            return new SuccessDataResult<List<CategoryToListDTO>>(_mapper.Map<List<CategoryToListDTO>>(categories), Messages.Success);
        }
    }
}