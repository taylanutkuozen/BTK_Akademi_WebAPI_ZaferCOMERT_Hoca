using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
            var books=_context.Books.ToList();
            return Ok(books);
        }
    }
}