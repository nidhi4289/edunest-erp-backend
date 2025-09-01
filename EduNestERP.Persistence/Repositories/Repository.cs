using Npgsql;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EduNestERP.Persistence.Repositories
{
    public class Repository<T> : IRepository<T>
    {

        protected readonly ITenantDataSourceProvider _dataSource;
        public Repository(ITenantDataSourceProvider dataSource) => _dataSource = dataSource;

        public virtual async Task<IEnumerable<T>> GetAllAsync()
        {
            throw new NotImplementedException("Override GetAllAsync in your repository for entity-specific logic.");
        }

        public virtual async Task<T?> GetByIdAsync(Guid id)
        {
            throw new NotImplementedException("Override GetByIdAsync in your repository for entity-specific logic.");
        }

        public virtual async Task<bool?> AddAsync(T entity)
        {
            throw new NotImplementedException();
        }

        public virtual Task<bool?> UpdateAsync(T entity)
        {
            throw new NotImplementedException();
        }
    }
}