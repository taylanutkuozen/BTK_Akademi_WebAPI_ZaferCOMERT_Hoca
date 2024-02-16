using AutoMapper;
using Entities.DataTransferObjects;
using Entities.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Repositories.Contracts;
using Services.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Services
{
    public class ServiceManager : IServiceManager
    {
        private readonly IBookService _bookService;
        private readonly IAuthenticationService _authenticationService;
        private readonly ICategoryService _categoryService;
        public ServiceManager(ICategoryService categoryService,IBookService bookService,IAuthenticationService authenticationService)
        {
            _categoryService = categoryService;
            _bookService = bookService;
            _authenticationService = authenticationService;
        }
        /*Repository Context=EF Core bağlıdır.*/
        public IBookService BookService => _bookService;
        public IAuthenticationService AuthenticationService => _authenticationService;
        public ICategoryService CategoryService => _categoryService;
    }
}