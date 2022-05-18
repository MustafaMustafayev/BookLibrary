using AutoMapper;
using Project.BLL.Abstract;
using Project.Core.Constants;
using Project.DAL.UnitOfWorks.Abstract;
using Project.DTO.DTOs.BookDTOs;
using Project.DTO.DTOs.Responses;
using Project.Entity.Entities;

namespace Project.BLL.Concrete
{
    public class BookService : IBookService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public BookService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IDataResult<Result>> AddAsync(BookToAddDTO authorToAddDTO)
        {
            Book book = _mapper.Map<Book>(authorToAddDTO);

            List<Author> bookAuthors = await _unitOfWork.AuthorRepository.GetListAsync(authorToAddDTO.AuthorIds);
            book.Authors = bookAuthors;

            await _unitOfWork.BookRepository.AddBookAsync(book);
            await _unitOfWork.CommitAsync();

            return new SuccessDataResult<Result>(Messages.Success);
        }

        public async Task<IDataResult<List<BookToListDTO>>> GetAsync()
        {
            List<Book> books = await _unitOfWork.BookRepository.GetListAsync();
            List<BookToListDTO> bookToListDTOs = _mapper.Map<List<BookToListDTO>>(books);

            return new SuccessDataResult<List<BookToListDTO>>(bookToListDTOs, Messages.Success);
        }

        public async Task<IDataResult<List<BookToListDTO>>> Search(Dictionary<string, string> filters)
        {
            List<Book> books = await _unitOfWork.BookRepository.Search(filters);
            List<BookToListDTO> bookToListDTOs = _mapper.Map<List<BookToListDTO>>(books);

            return new SuccessDataResult<List<BookToListDTO>>(bookToListDTOs, Messages.Success);
        }
    }
}