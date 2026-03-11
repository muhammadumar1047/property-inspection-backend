using Microsoft.EntityFrameworkCore;
using PropertyInspection.Core.Interfaces.Repositories;
using PropertyInspection.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace PropertyInspection.Infrastructure.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        private readonly AppDbContext _context;
        protected readonly DbSet<T> _dbSet;

        public GenericRepository(AppDbContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }

        //public async Task<T?> GetByIdAsync(Guid id) =>
        //    await _dbSet.FindAsync(id);

        public async Task<T?> GetByIdAsync(
            Guid id,
            Func<IQueryable<T>, IQueryable<T>>? include = null)
        {
            IQueryable<T> query = _context.Set<T>();

            if (include != null)
                query = include(query);

            return await query.FirstOrDefaultAsync(e => EF.Property<Guid>(e, "Id") == id);
        }

        //public async Task<IEnumerable<T>> GetAllAsync() =>
        //    await _dbSet.AsNoTracking().ToListAsync();

        //public async Task<List<T>> GetAllAsync(Expression<Func<T, bool>>? predicate = null,
        //    Func<IQueryable<T>, IQueryable<T>>? include = null)
        //{
        //    IQueryable<T> query = _context.Set<T>();

        //    if (predicate != null)
        //        query = query.Where(predicate);

        //    if (include != null)
        //        query = include(query);

        //    return await query.ToListAsync();
        //}

        public async Task<List<T>> GetAsync(
            Expression<Func<T, bool>>? predicate = null,
            Func<IQueryable<T>, IQueryable<T>>? include = null,
            Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
            int? skip = null,
            int? take = null
        )
        {
            IQueryable<T> query = _context.Set<T>();

            if (predicate != null)
                query = query.Where(predicate);

            if (include != null)
                query = include(query);

            if (orderBy != null)
                query = orderBy(query);

            if (skip.HasValue)
                query = query.Skip(skip.Value);

            if (take.HasValue)
                query = query.Take(take.Value);

            return await query.ToListAsync();
        }


        public async Task AddAsync(T entity) =>
            await _dbSet.AddAsync(entity);

        public async Task UpdateAsync(T entity)
        {
            _dbSet.Update(entity);
            await Task.CompletedTask;
        }

        public void Remove(T entity) => _dbSet.Remove(entity);

        public async Task DeleteAsync(Guid id, Guid deletedBy)
        {

            var entity = await _dbSet.FindAsync(id);
            if (entity != null)
            {
                // Assuming the entity has IsDeleted, DeletedAt, and DeletedBy properties
                var isDeletedProp = entity.GetType().GetProperty("IsDeleted");
                var deletedAtProp = entity.GetType().GetProperty("DeletedAt");
                var deletedByProp = entity.GetType().GetProperty("DeletedBy");
                if (isDeletedProp != null && deletedAtProp != null && deletedByProp != null)
                {
                    isDeletedProp.SetValue(entity, true);
                    deletedAtProp.SetValue(entity, DateTime.UtcNow);
                    deletedByProp.SetValue(entity, deletedBy);
                    _dbSet.Update(entity);
                }
            }
            await Task.CompletedTask;
        }
        public async Task<int> CountAsync(Expression<Func<T, bool>>? predicate = null)
        {
            IQueryable<T> query = _context.Set<T>();
            if (predicate != null)
                query = query.Where(predicate);
            return await query.CountAsync();
        }

        public async Task<(List<T> Items, int TotalCount)> GetPagedAsync(
            int pageNumber,
            int pageSize,
            Expression<Func<T, bool>>? predicate = null,
            Func<IQueryable<T>, IQueryable<T>>? include = null,
            Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null)
        {
            var normalizedPage = pageNumber < 1 ? 1 : pageNumber;
            var normalizedPageSize = pageSize < 1 ? 10 : pageSize;
            var skip = (normalizedPage - 1) * normalizedPageSize;

            IQueryable<T> query = _context.Set<T>();
            if (predicate != null)
                query = query.Where(predicate);

            var totalCount = await query.CountAsync();

            if (include != null)
                query = include(query);

            if (orderBy != null)
                query = orderBy(query);

            var items = await query
                .Skip(skip)
                .Take(normalizedPageSize)
                .ToListAsync();

            return (items, totalCount);
        }

        public async Task<T?> FirstOrDefaultAsync(
         Expression<Func<T, bool>> predicate,
         Func<IQueryable<T>, IQueryable<T>>? include = null)
        {
            IQueryable<T> query = _context.Set<T>();

            if (include != null)
                query = include(query);

            return await query.FirstOrDefaultAsync(predicate);
        }

        public async Task<bool> DeleteWhereAsync(Expression<Func<T, bool>> predicate)
        {
            var entity = await _dbSet.FirstOrDefaultAsync(predicate);
            if (entity == null)
                return false;

            _dbSet.Remove(entity);
            return true;
        }

        public IQueryable<T> GetQueryable(
            Expression<Func<T, bool>>? filter = null,
            Func<IQueryable<T>, IQueryable<T>>? include = null)
        {
            IQueryable<T> query = _context.Set<T>();

            if (include != null)
                query = include(query);

            if (filter != null)
                query = query.Where(filter);

            return query;
        }
    }
}
