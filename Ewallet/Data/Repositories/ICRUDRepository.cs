using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ewallet.Data.Repositories
{
    public interface ICRUDRepository
    {
        Task<bool> Add<T>(T entity);
        Task<bool> Edit<T>(T entity);
        Task<bool> Delete<T>(T entity);
        Task<bool> SaveChanges();
    }
}
