using Repositories.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Repositories.EFCore
{
    public class RepositoryManager : IRepositoryManager
    {
        private readonly RepositoryContext _context;
        private readonly IBookRepository _bookRepository;
        private readonly ICategoryRepository _categoryRepository;
        public RepositoryManager(RepositoryContext context,IBookRepository bookRepository,ICategoryRepository categoryRepository)
        {
            _context = context;
            _bookRepository = bookRepository;
            _categoryRepository = categoryRepository;
        }
        public IBookRepository Book => _bookRepository;/*Nesne ancak ve ancak kullanıldığı anda new'lenecek, aksi durumda new'leme yapılmayacak.*/
        /*IoC kaydı tek bir sefer olması için*/
        public ICategoryRepository Category => _categoryRepository;
        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}