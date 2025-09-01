namespace EduNestERP.Persistence.Repositories;

public interface IRepository<T>
{
      Task<IEnumerable<T>> GetAllAsync();
      Task<T?> GetByIdAsync(Guid id);
      
      Task<bool?> AddAsync(T entity);
      
      Task<bool?> UpdateAsync(T entity);
}