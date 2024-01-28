using Entities.DataTransferObjects;
using Entities.LinkModels;
using Entities.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Net.Http.Headers;
using Services.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Services
{
    public class BookLinks : IBookLinks
    {
        private readonly LinkGenerator _linkGenerator;
        private readonly IDataShaper<BookDto> _dataShaper;
        public BookLinks(LinkGenerator linkGenerator,IDataShaper<BookDto> dataShaper)
        {
            _linkGenerator = linkGenerator;
            _dataShaper = dataShaper;
        }
        public LinkResponse TryGenerateLinks(IEnumerable<BookDto> booksDto, string fields, HttpContext httpcontext)
        {
            var shapedBooks=ShapeData(booksDto, fields);
            if (ShouldGenerateLinks(httpcontext))
                return ReturnLinkedBooks(booksDto, fields, httpcontext, shapedBooks);
            return ReturnShapedBooks(shapedBooks);
        }
        private LinkResponse ReturnLinkedBooks(IEnumerable<BookDto> booksDto, string fields, HttpContext httpcontext, List<Entity> shapedBooks)
        {
            var bookDtoList = booksDto.ToList();
            for (int index = 0; index < bookDtoList.Count(); index++)
            {
                var bookLinks = CreateForBook(httpcontext, bookDtoList[index],fields);
                shapedBooks[index].Add("Links", bookLinks);
                /*shapedBooks-->bir varlığı temsil ediyor{List<Entity>}.Entity gidip baktığımızda bir dictionary olduğunu görebiliriz.*/
            }
            var bookCollection = new LinkCollectionWrapper<Entity>(shapedBooks);
            CreateForBooks(httpcontext, bookCollection);
            return new LinkResponse { HasLinks = true, LinkedEntities = bookCollection };
        }
        private LinkCollectionWrapper<Entity> CreateForBooks(HttpContext httpContext,LinkCollectionWrapper<Entity> bookCollectionWrapper)
        {
            bookCollectionWrapper.Links
                .Add
                (new Link() 
                {
                    Href = $"/api/{httpContext.GetRouteData().Values["controller"].ToString().ToLower()}",
                    Relation="self",
                    Method="GET"
                });
            return bookCollectionWrapper;
        }
        private List<Link> CreateForBook(HttpContext httpcontext, BookDto bookDto, string fields)
        {
            var links = new List<Link>()
            {
                new Link()
                {
                    Href=$"/api/{httpcontext.GetRouteData().Values["controller"].ToString().ToLower()}"+ $"/{bookDto.BookID}",
                    Relation="self",
                    Method="GET"
                }, /*GetRoute metodu ile controller bilgisini alabiliyoruz.*/
                new Link()
                {
                    Href=$"/api/{httpcontext.GetRouteData().Values["controller"].ToString().ToLower()}",
                    Relation="create",
                    Method="POST"
                }
            };
            return links;
        }
        private LinkResponse ReturnShapedBooks(List<Entity> shapedBooks)
        {
            return new LinkResponse() { ShapedEntities = shapedBooks };
        }
        private bool ShouldGenerateLinks(HttpContext httpcontext)
        {
            var mediaType =(MediaTypeHeaderValue)httpcontext.Items["AcceptHeaderMediaType"];
            return mediaType.SubTypeWithoutSuffix.EndsWith("hateoas", StringComparison.InvariantCultureIgnoreCase);
                /*prefixleri ayıklamak için*/
        }
        private List<Entity> ShapeData(IEnumerable<BookDto> booksDto, string fields)
        {
            return _dataShaper.ShapeData(booksDto, fields).Select(b=>b.Entity).ToList();
        }
    }
}
/*
booksDto-->(fields)ShapedData
booksDto-->LinkedData
 */