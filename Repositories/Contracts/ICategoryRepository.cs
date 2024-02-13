﻿using Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Repositories.Contracts
{
    public interface ICategoryRepository:IRepositoryBase<Category>
    {
        Task<IEnumerable<Category>> GetAllCategoriesAsync(bool trackChanges);
        Task<Category> GetOneCategoryIDAsync(int id,bool trackChanges);
        void CreateOneCategory(Category category);
        void DeleteOneCategory(Category category);
        void UpdateOneCategory(Category category);
    }
}