using Entities.DataTransferObjects;
using Entities.Exceptions;
using Entities.Models;
using Entities.RequestFeatures;
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
    [ServiceFilter(typeof(LogFilterAttribute))]
    /*Action bazl� de�il controller bazl�*/
    [ApiController]
    [Route("api/books")]
    public class BooksController : ControllerBase
    {
        private readonly IServiceManager _manager;
        public BooksController(IServiceManager manager)
        {
            _manager = manager;
        }
        [HttpHead]
        [HttpGet]
        [ServiceFilter(typeof(ValidateMediaTypeAttribute))]
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
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetOneBookAsync([FromRoute(Name = "id")] int id)
        {
            var book = await _manager.BookService.GetOneBookByIDAsync(id, false);
            return Ok(book);
        }
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        [HttpPost]
        public async Task<IActionResult> CreateOneBookAsync([FromBody] BookDtoForInsertion bookDto)
        {
                var book=await _manager.BookService.CreateOneBookAsync(bookDto);
                return StatusCode(201, book); 
            /*CreatedAtRoute() bu metotla insert i�lemi yaparsak StatusCode yerine, response header'�na location bilgisi koyabiliriz. Url'�na eri�ebiliriz.*/
        }
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
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteOneBooks([FromRoute(Name = "id")] int id)
        {
                await _manager.BookService.DeleteOneBookAsync(id, false);
                return NoContent();
        }
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
        [HttpOptions]
        public IActionResult GetBooksOptions()
        {
            Response.Headers.Add("Allow", "GET, PUT, POST, PATCH, DELETE, HEAD, OPTIONS");
            return Ok();
        }
    }
}