using Entities.DataTransferObjects;
using Entities.Exceptions;
using Entities.Models;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Services.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Presentation.Controllers
{
    [ApiController]
    [Route("api/books")]
    public class BooksController : ControllerBase
    {
        private readonly IServiceManager _manager;
        public BooksController(IServiceManager manager)
        {
            _manager = manager;
        }
        [HttpGet]
        public IActionResult GetAllBooks()
        {
                var books = _manager.BookService.GetAllBooks(false);
                return Ok(books);
        }
        [HttpGet("{id:int}")]
        public IActionResult GetOneBook([FromRoute(Name = "id")] int id)
        {
            var book = _manager.BookService.GetOneBookByID(id, false);
            return Ok(book);
        }
        [HttpPost]
        public IActionResult CreateOneBook([FromBody] BookDtoForInsertion bookDto)
        {
                if (bookDto is null)
                    return BadRequest();//400
                var book=_manager.BookService.CreateOneBook(bookDto);
                return StatusCode(201, book); 
            /*CreatedAtRoute() bu metotla insert i�lemi yaparsak StatusCode yerine, response header'�na location bilgisi koyabiliriz. Url'�na eri�ebiliriz.*/
        }
        [HttpPut("{id:int}")]
        public IActionResult UpdateOneBook([FromRoute(Name = "id")] int id, [FromBody] BookDtoForUpdate bookDto)
        {
                if (bookDto is null)
                    return BadRequest();//400
                //check book?
                var entity = _manager.BookService.GetOneBookByID(id, true);
                /*Id'ler tutuyor mu?
                if (id != book.BookID)
                {
                    return BadRequest();
                }*/
                _manager.BookService.UpdateOneBook(id, bookDto, true);
                return NoContent();//204
        }
        [HttpDelete("{id:int}")]
        public IActionResult DeleteOneBooks([FromRoute(Name = "id")] int id)
        {
                _manager.BookService.DeleteOneBook(id, false);
                return NoContent();
        }
        [HttpPatch("{id:int}")]
        public IActionResult PartiallyUpdateOneBook([FromRoute(Name = "id")] int id, [FromBody] JsonPatchDocument<BookDto> bookPatch)
        {
                //check entity
                var bookDto = _manager.BookService.GetOneBookByID(id, true);
                bookPatch.ApplyTo(bookDto);
                _manager.BookService.UpdateOneBook(id, new BookDtoForUpdate{
                    BookID=bookDto.BookID,
                    Title=bookDto.Title,
                    Price=bookDto.Price,
                }, true);
                return NoContent();
        }
    }
}