using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Repositories.Contracts
{
    public interface IRepositoryManager
    {
        /*UnitofWork*/
        IBookRepository Book { get; }
        Task SaveAsync(); /*void yerine yalın bir Task ifadesi gelir.*/
    }
}