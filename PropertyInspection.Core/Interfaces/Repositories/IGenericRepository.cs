using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace PropertyInspection.Core.Interfaces.Repositories
{
    public interface IGenericRepository<T> where T : class
    {
        Task<T?> GetByIdAsync(
           Guid id,
           Func<IQueryable<T>, IQueryable<T>>? include = null
        );

        Task<List<T>> GetAsync(
            Expression<Func<T, bool>>? predicate = null,
            Func<IQueryable<T>, IQueryable<T>>? include = null,
            Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
            int? skip = null,
            int? take = null
        );
        Task AddAsync(T entity);
        Task UpdateAsync(T entity);
        void Remove(T entity);
        Task DeleteAsync(Guid id, Guid deletedBy);
        Task<bool> DeleteWhereAsync(Expression<Func<T, bool>> predicate);
        Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate, Func<IQueryable<T>, IQueryable<T>>? include = null);
        Task<int> CountAsync(Expression<Func<T, bool>>? predicate = null);
        Task<(List<T> Items, int TotalCount)> GetPagedAsync(
            int pageNumber,
            int pageSize,
            Expression<Func<T, bool>>? predicate = null,
            Func<IQueryable<T>, IQueryable<T>>? include = null,
            Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null
        );

        IQueryable<T> GetQueryable(
           Expression<Func<T, bool>>? filter = null,
           Func<IQueryable<T>, IQueryable<T>>? include = null
        );

    }
}
