using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace campuslove.domain.ports
{
    public interface IGenericRepository<T>
    {
        // Operaciones básicas CRUD
        Task<IEnumerable<T>> GetAllAsync();
        Task<T> GetByIdAsync(object id);
        Task<int> InsertAsync(T entity);
        Task<bool> UpdateAsync(T entity);
        Task<bool> DeleteAsync(object id);
    }
}