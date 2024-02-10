using Entities.DataTransferObjects;
using Entities.Exceptions;
using Entities.Models;
using Entities.RequestFeatures;
using Marvin.Cache.Headers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Presentation.ActionFilters;
using Services.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
namespace Presentation.Controllers
{
    //[ApiVersion("1.0")]
    [ServiceFilter(typeof(LogFilterAttribute))]
    /*Action bazl� de�il controller bazl�*/
    [ApiController]
    [Route("api/books")]
    //[ResponseCache(CacheProfileName = "5mins")]/*T�m requestler i�in Cache'leme i�lemi. Program.cs'deki controller config i�erisindeki ismi. ExpirationModel*/
    //[HttpCacheExpiration(CacheLocation =CacheLocation.Public,MaxAge =80)]
    /*Kayna�a daha yak�n bir config ifadesi oldu�u i�in ServiceExtension de�il yukar�daki komut �al��acakt�r.*/
    public class BooksController : ControllerBase
    {
        private readonly IServiceManager _manager;
        public BooksController(IServiceManager manager)
        {
            _manager = manager;
        }
        [Authorize]/*�lgili metodun korunaca��n� ifade ettik. User rolune sahip olan bir kullan�c� eri�ebilir.*/
        [HttpHead]
        [HttpGet(Name ="GetAllBooksAsync")]
        [ServiceFilter(typeof(ValidateMediaTypeAttribute))]
        //[ResponseCache(Duration =60)]/*Cachable �zelli�i kazand�. Metota daha yak�n oldu�u i�in ge�erli olacak. Class ba��ndaki Cache Profile bu metot �zerinde i�lem yapamayacakt�r. ExpirationModel*/
        public async Task<IActionResult> GetAllBooksAsync([FromQuery]BookParameters bookParameter)
        {
            var linkParameters = new LinkParameters()
            {
                BookParameters = bookParameter,
                HttpContext = HttpContext
            };
                var result=await _manager
                .BookService
                .GetAllBooksAsync(linkParameters,false);
                Response.Headers.Add("X-Pagination",JsonSerializer.Serialize(result.metaData));
                return result.linkResponse.HasLinks ?
                 Ok(result.linkResponse.LinkedEntities):
                 Ok(result.linkResponse.ShapedEntities);
        }
        [Authorize]
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetOneBookAsync([FromRoute(Name = "id")] int id)
        {
            var book = await _manager.BookService.GetOneBookByIDAsync(id, false);
            return Ok(book);
        }
        [Authorize]/*Admin post i�lemi yapabilir*/
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        [HttpPost(Name ="CreateOneBookAsync")]
        public async Task<IActionResult> CreateOneBookAsync([FromBody] BookDtoForInsertion bookDto)
        {
                var book=await _manager.BookService.CreateOneBookAsync(bookDto);
                return StatusCode(201, book); 
            /*CreatedAtRoute() bu metotla insert i�lemi yaparsak StatusCode yerine, response header'�na location bilgisi koyabiliriz. Url'�na eri�ebiliriz.*/
        }
        [Authorize(Roles="Admin,Editor")]
        [ServiceFilter(typeof(ValidationFilterAttribute))]//, Order =1)]
        /*Order=1 �nce validation yap. order=2 i�in �nce order'1 ger�ekle�sin sonra log atmal� sistem. Order �ncelik s�ras� gibi*/
        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateOneBookAsync([FromRoute(Name = "id")] int id, [FromBody] BookDtoForUpdate bookDto)
        {
                if (bookDto is null)
                    return BadRequest();//400
                if(!ModelState.IsValid)
                {
                    return UnprocessableEntity(ModelState);
                }
                await _manager.BookService.UpdateOneBookAsync(id, bookDto, false);
                return NoContent();//204
        }
        [Authorize(Roles ="Admin")]
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteOneBooks([FromRoute(Name = "id")] int id)
        {
                await _manager.BookService.DeleteOneBookAsync(id, false);
                return NoContent();
        }
        [Authorize(Roles ="Admin")]
        [HttpPatch("{id:int}")]
        public async Task<IActionResult> PartiallyUpdateOneBookAsync([FromRoute(Name = "id")] int id, [FromBody] JsonPatchDocument<BookDtoForUpdate> bookPatch)
        {
            if (bookPatch is null)
                return BadRequest();
                //check entity
            var result = await _manager.BookService.GetOneBookForPatchAsync(id, false);
            bookPatch.ApplyTo(result.bookDtoForUpdate, ModelState);
            TryValidateModel(result.bookDtoForUpdate);
            if (!ModelState.IsValid)
                return UnprocessableEntity(ModelState);
            await _manager.BookService.SaveChangesForPatchAsync(result.bookDtoForUpdate, result.book);
                return NoContent();
        }
        [Authorize]
        [HttpOptions]
        public IActionResult GetBooksOptions()
        {
            Response.Headers.Add("Allow", "GET, PUT, POST, PATCH, DELETE, HEAD, OPTIONS");
            return Ok();
        }
    }
}