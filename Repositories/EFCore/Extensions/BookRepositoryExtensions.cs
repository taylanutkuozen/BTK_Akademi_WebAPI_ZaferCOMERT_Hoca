using Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Dynamic.Core;
namespace Repositories.EFCore.Extensions
{
    public static class BookRepositoryExtensions
    {
        public static IQueryable<Book> FilterBooks(this IQueryable<Book> books, uint minPrice, uint maxPrice) =>
            books.Where(book =>
        book.Price >= minPrice &&
        book.Price <= maxPrice);
        public static IQueryable<Book> Search(this IQueryable<Book> books, string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return books;
            var lowerCaseTerm = searchTerm.Trim().ToLower();//kara
            return books
                .Where(b => b.Title
                .ToLower()
                .Contains(searchTerm));
        }
        public static IQueryable<Book> Sort(this IQueryable<Book> books, string orderByQueryString)
        {
            if (string.IsNullOrEmpty(orderByQueryString))
                return books.OrderBy(b => b.BookID);
            var orderParams = orderByQueryString.Trim().Split(',');
            var propertyInfos = typeof(Book)
                .GetProperties(BindingFlags.Public | BindingFlags.Instance);/*Reflection*/
            var orderQueryBuilder = new StringBuilder();
            /*title ascending, price descending, id ascending[,]*/
            foreach ( var param in orderParams )
            {
                if (string.IsNullOrWhiteSpace(param))
                    continue;
                var propertyFromQueryName = param.Split(' ')[0]; 
                /*books?orderBy=title,price desc, id asc*/
                var objectProperty = propertyInfos.FirstOrDefault(pi => pi.Name.Equals(propertyFromQueryName, StringComparison.InvariantCultureIgnoreCase));
                if (objectProperty is null)
                    continue;
                var direction = param.EndsWith(" desc") ? "descending" : "ascending";
                orderQueryBuilder.Append($"{objectProperty.Name.ToString()}  {direction},");
            }
            var orderQuery = orderQueryBuilder.ToString().TrimEnd(',',' ');
            if (orderQuery is null)
                return books.OrderBy(b => b.BookID);
            return books.OrderBy(orderQuery);
        }
    }
}