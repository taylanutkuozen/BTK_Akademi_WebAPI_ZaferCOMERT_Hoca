using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using WebAPI.Models;
using WebAPI.Repositories;
namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BooksController : ControllerBase
    {
        private readonly RepositoryContext _context;//context ihtiyaç var
        public BooksController(RepositoryContext context)
        {
            /*_context=new RepositoryContext() --> Eğer böyle tanımlansaydı DbContextOptions nesnesi parametre olarak verecektik ve yeniden bir ConnectionString yazmak gerekecekti---Injection ifadesi burada kullanıldı.*/
            _context = context;   //Resolve         
        }
        [HttpGet]
        public IActionResult GetAllBooks()
        {
            try
            {
                var books = _context.Books.ToList();
                return Ok(books);
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
           
        }
        [HttpGet("{id:int}")]
        public IActionResult GetOneBook([FromRoute(Name ="id")]int id)
        {
            try
            {
                var book = _context.Books.Where(b => b.BookID.Equals(id)).SingleOrDefault();
                if (book is null)
                    return NotFound();
                return Ok(book);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        [HttpPost]
        public IActionResult CreateOneBook([FromBody] Book book)
        {
            try
            {
                if (book is null)
                    return BadRequest();//400
                _context.Books.Add(book);
                _context.SaveChanges();
                return StatusCode(201, book);
            }
            catch (Exception exception)
            {
                return BadRequest(exception.Message);
            }
        }
        [HttpPut("{id:int}")]
        public IActionResult UpdateOneBook([FromRoute(Name ="id")] int id, [FromBody] Book book)
        {
            try
            {
                //check book?
                var entity = _context.Books.Where(b => b.BookID.Equals(id)).SingleOrDefault();
                //book gerçekten var mı?
                if (entity is null)
                {
                    return NotFound(); //404
                }
                //Id'ler tutuyor mu?
                if (id != book.BookID)
                {
                    return BadRequest();
                }
                entity.Title = book.Title;
                entity.Price = book.Price;
                _context.SaveChanges();
                return Ok(book);
            }
            catch (Exception exception)
            {
                return BadRequest(exception.Message);
            }
        }
        [HttpDelete("{id:int}")]
        public IActionResult DeleteOneBooks([FromRoute(Name ="id")] int id)
        {
            try
            {
                var entity = _context.Books.Where(b =>b.BookID.Equals(id)).SingleOrDefault();
                if(entity is null)
                {
                    return NotFound(new
                    {
                        statusCode = 404,
                        message = $"Book with id:{id} could not found"
                    });
                }
                _context.Books.Remove(entity);
                _context.SaveChanges();
                return NoContent();
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }
        }
        [HttpPatch("{id:int}")]
        public IActionResult PartiallyUpdateOneBook([FromRoute(Name ="id")] int id, [FromBody] JsonPatchDocument<Book> bookPatch)
        {
            try
            {
                //check entity
                var entity = _context
                    .Books
                    .Where(b => b.BookID.Equals(id))
                    .SingleOrDefault();
                if(entity is null)
                {
                    return NotFound();
                }
                bookPatch.ApplyTo(entity);
                _context.SaveChanges();
                return NoContent();
            }
            catch (Exception exception)
            {
                return BadRequest(exception.Message);
            }
        }
    }
}