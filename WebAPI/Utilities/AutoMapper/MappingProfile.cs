using AutoMapper;
using Entities.DataTransferObjects;
using Entities.Models;

namespace WebAPI.Utilities.AutoMapper
{
    public class MappingProfile : Profile /*Bir automapper'ı ifade eden bir class*/
    {
        public MappingProfile()
        {
            CreateMap<BookDtoForUpdate, Book>().ReverseMap();
            /*ReverseMap ile source'tan destination'a ve destination'dan source'a map işlemi gerçekleşir.*/
            CreateMap<Book, BookDto>();
            CreateMap<BookDtoForInsertion, Book>();
        }
    }
}