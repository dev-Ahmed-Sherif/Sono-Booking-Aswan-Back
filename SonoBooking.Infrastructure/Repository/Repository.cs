using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using SonoBooking.Common.Extensions;
using SonoBooking.Common.Infrastructure.Repository;

namespace SonoBooking.Infrastructure.Repository
{
    public class Repository<T> : IRepository<T> where T : class
    {
       
        protected readonly DbContext DbContext;
        protected DbSet<T> DbSet;

        public Repository(DbContext dbContext)
        {
            DbContext = dbContext;
            DbSet = DbContext.Set<T>();
        }


        public async Task<T> GetAsync(params object[] keys)
        {
            return await DbSet.FindAsync(keys);
        }

        public async Task<T> GetAsync(CancellationToken cancellationToken, params object[] keys)
        {
            return await DbSet.FindAsync(keys, cancellationToken);
        }

        public IQueryable<T> GetAllQueryable(Expression<Func<T, bool>> predicate = null)
        {
            if (predicate != null)
            {
                return DbSet.Where(predicate);
            }
            return DbSet;
        }


        public async Task<T> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate, Func<IQueryable<T>, IOrderedQueryable<T>> orderby = null, Func<IQueryable<T>, IIncludableQueryable<T, object>> include = null, bool disableTracking = true, CancellationToken cancellationToken = default)
        {
            IQueryable<T> query = DbSet;
            if (disableTracking)
            {
                query = query.AsNoTracking();
            }
            if (orderby != null)
            {
                query = orderby(query);
            }
            if (predicate != null)
            {
                query = query.Where(predicate);
            }
            if (include != null)
            {
                query = include(query).AsSplitQuery();
            }
            return await query.FirstOrDefaultAsync(cancellationToken);

        }

        public async Task<T> LastOrDefaultAsync(Expression<Func<T, bool>> predicate, Func<IQueryable<T>, IOrderedQueryable<T>> orderby = null, Func<IQueryable<T>, IIncludableQueryable<T, object>> include = null, bool disableTracking = true, CancellationToken cancellationToken = default)
        {
            IQueryable<T> query = DbSet;
            if (disableTracking)
            {
                query = query.AsNoTracking();
            }
            if (orderby != null)
            {
                query = orderby(query);
            }
            if (predicate != null)
            {
                query = query.Where(predicate);
            }
            if (include != null)
            {
                query = include(query).AsSplitQuery();
            }
            return await query.LastOrDefaultAsync(cancellationToken);

        }

        public async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate = null, IEnumerable<SortModel> orderByCriteria = null, Func<IQueryable<T>, IIncludableQueryable<T, object>> include = null, bool disableTracking = true, CancellationToken cancellationToken = default)
        {
            IQueryable<T> query = DbSet;
            if (disableTracking)
            {
                query = query.AsNoTracking();
            }
            if (predicate != null)
            {
                query = query.Where(predicate);
            }
            if (orderByCriteria != null)
            {
                query = query.OrderBy(orderByCriteria);
            }
            if (include != null)
            {
                query = include(query).AsSplitQuery();
            }
            return await query.ToListAsync(cancellationToken);
        }

        public async Task<(int Count, IEnumerable<T> Result)> FindPagedAsync(Expression<Func<T, bool>> predicate = null, int pageNumber = 0, int pageSize = 0, IEnumerable<SortModel> orderByCriteria = null, Func<IQueryable<T>, IIncludableQueryable<T, object>> include = null, bool disableTracking = true, CancellationToken cancellationToken = default)
        {
            IQueryable<T> query = DbSet;
            if (disableTracking)
            {
                query = query.AsNoTracking();
            }
            if (predicate != null)
            {
                query = query.Where(predicate);
            }
            if (orderByCriteria != null)
            {
                var field = orderByCriteria.First().PairAsSqlExpression;
                query = query.OrderBy(field);
            }
            if (include != null)
            {
                query = include(query).AsSplitQuery();
            }
            int count = await query.CountAsync(cancellationToken);
            return (count, await query.Skip(pageNumber).Take(pageSize).ToListAsync(cancellationToken));
        }

        public async Task<IEnumerable<T>> GetAllAsync(IEnumerable<SortModel> orderByCriteria = null, Func<IQueryable<T>, IIncludableQueryable<T, object>> include = null, bool disableTracking = true, CancellationToken cancellationToken = default)
        {
            IQueryable<T> query = DbSet;
            if (disableTracking)
            {
                query = query.AsNoTracking();
            }
            if (orderByCriteria != null)
            {
                query = query.OrderBy(orderByCriteria);
            }
            if (include != null)
            {
                query = include(query).AsSplitQuery();
            }
            return await query.ToListAsync(cancellationToken);
        }


        public async Task<ICollection<TType>> GetSelectAsync<TType>(Expression<Func<T, bool>> where, Expression<Func<T, TType>> select, CancellationToken cancellationToken = default) where TType : class
        {
            return await DbSet.Where(where).Select(select).ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<TType>> FindSelectAsync<TType>(Expression<Func<T, TType>> select, Expression<Func<T, bool>> predicate = null, IEnumerable<SortModel> orderByCriteria = null, Func<IQueryable<T>, IIncludableQueryable<T, object>> include = null, bool disableTracking = true, CancellationToken cancellationToken = default) where TType : class
        {
            IQueryable<T> query = DbSet;
            if (disableTracking)
            {
                query = query.AsNoTracking();
            }
            if (predicate != null)
            {
                query = query.Where(predicate);
            }
            if (orderByCriteria != null)
            {
                query = query.OrderBy(orderByCriteria);
            }
            if (include != null)
            {
                query = include(query).AsSplitQuery();
            }
            return await query.Select(select).ToListAsync(cancellationToken);
        }

        public async Task<(int Count, IEnumerable<TType> Result)> FindPagedSelectAsync<TType>(Expression<Func<T, TType>> select, Expression<Func<T, bool>> predicate = null, int pageNumber = 0, int pageSize = 0, IEnumerable<SortModel> orderByCriteria = null, Func<IQueryable<T>, IIncludableQueryable<T, object>> include = null, bool disableTracking = true, CancellationToken cancellationToken = default) where TType : class
        {
            IQueryable<T> query = DbSet;
            if (disableTracking)
            {
                query = query.AsNoTracking();
            }
            if (predicate != null)
            {
                query = query.Where(predicate);
            }
            if (include != null)
            {
                query = include(query).AsSplitQuery();
            }
            if (orderByCriteria != null)
            {
                var field = orderByCriteria.First().PairAsSqlExpression;
                query = query.OrderBy(field);
            }
            var count = await query.CountAsync(cancellationToken);
            return (count, await query.Skip(pageNumber).Take(pageSize).Select(select).ToListAsync(cancellationToken));
        }

        public IList<TReturn> FindGrouped<TResult, TKey, TGroup, TReturn>(
            List<Expression<Func<T, bool>>> predicates,
            Expression<Func<T, TResult>> firstSelector,
            Expression<Func<TResult, TKey>> orderSelector,
            Func<TResult, TGroup> groupSelector,
            Func<IGrouping<TGroup, TResult>, TReturn> selector, bool isDesc = false,
            Func<IQueryable<T>, IIncludableQueryable<T, object>> include = null, int pageNumber = 0, int pageSize = int.MaxValue)
        {
            IQueryable<T> query = DbSet;
            if (include != null)
            {
                query = include(query).AsSplitQuery();
            }

            var result = predicates
                .Aggregate(query, (current, predicate) => current.Where(predicate))
                .Select(firstSelector);
            return isDesc ? result.OrderByDescending(orderSelector).GroupBy(groupSelector).Select(selector).Skip(pageNumber).Take(pageSize).ToList() : result.OrderBy(orderSelector).GroupBy(groupSelector).Select(selector).Skip(pageNumber).Take(pageSize).ToList();
        }

        public IEnumerable<TB> ExecuteStored<TB>(string sql) where TB : class
        {
            var result = DbContext.Set<TB>().FromSqlRaw(sql);
            return result;
        }

        public async Task<int> ExecWithStoreProcedure(string query, CancellationToken cancellationToken = default)
        {
            return await DbContext.Database.ExecuteSqlRawAsync(query, cancellationToken);
        }

        public long GetNextSequenceValue(string sequenceName)
        {
            var value = DbContext.GetNextSequenceValue(sequenceName);
            return value;
        }


        public async Task<int> Count(Expression<Func<T, bool>> predicate = null, CancellationToken cancellationToken = default) => predicate == null ? await DbSet.CountAsync(cancellationToken) : await DbSet.CountAsync(predicate, cancellationToken);

        public async Task<TB> Max<TB>(Expression<Func<T, TB>> selector, Expression<Func<T, bool>> predicate = null, CancellationToken cancellationToken = default)
        {
            if (predicate == null)
                return await DbSet.MaxAsync(selector, cancellationToken);
            return await DbSet.Where(predicate).MaxAsync(selector, cancellationToken);
        }

        public async Task<bool> Any(Expression<Func<T, bool>> predicate = null, CancellationToken cancellationToken = default) => predicate == null ? await DbSet.AnyAsync(cancellationToken) : await DbSet.AnyAsync(predicate, cancellationToken);

        public IQueryable<T> Union(params IQueryable<T>[] queries)
        {
            if (queries == null || queries.Length == 0)
            {
                throw new ArgumentException("At least one queryable must be provided", nameof(queries));
            }

            IQueryable<T> result = queries[0];
            for (int i = 1; i < queries.Length; i++)
            {
                result = result.Union(queries[i]);
            }

            return result;
        }

        public T Add(T newEntity)
        {
            return DbSet.Add(newEntity).Entity;
        }

        public async Task<T> AddAsync(T newEntity, CancellationToken cancellationToken = default)
        {
            var result = await DbSet.AddAsync(newEntity, cancellationToken);
            return result.Entity;
        }

        public void AddRange(IEnumerable<T> entities)
        {
            DbSet.AddRange(entities);
        }

        public async Task AddRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default)
        {
            await DbSet.AddRangeAsync(entities, cancellationToken);
        }

        public void Update(T originalEntity, T newEntity)
        {
            DbContext.Entry(originalEntity).CurrentValues.SetValues(newEntity);
        }

        public async Task UpdateAsync(object id, T newEntity, CancellationToken cancellationToken = default)
        {
            var originalEntity = await DbSet.FindAsync([id], cancellationToken);
            if (originalEntity != null)
            {
                DbContext.Entry(originalEntity).CurrentValues.SetValues(newEntity);
            }

        }

        public void UpdateRange(IEnumerable<T> newEntities)
        {
            DbContext.UpdateRange(newEntities);
        }

        public void Remove(T entity)
        {
            DbSet.Remove(entity);
        }

        public void RemoveLogical(T entity)
        {
            var type = entity.GetType();
            var property = type.GetProperty("IsDeleted");
            if (property != null) property.SetValue(entity, true);
            var id = type.GetProperty("Id")?.GetValue(entity);
            var original = DbSet.Find(id);
            Update(original, entity);
        }

        public async void Remove(Expression<Func<T, bool>> predicate)
        {
            var objects = await DbSet.FindAsync(predicate);
            if (objects != null)
            {
                DbSet.RemoveRange(objects);
            }
            
        }

        public void RemoveRange(IEnumerable<T> entities, CancellationToken cancellationToken = default)
        {
            DbSet.RemoveRange(entities);
        }

        public async Task<T> GetLast(CancellationToken cancellationToken = default)
        {
            // Ensure that the entity type `T` has a `CreatedAt` property.
            // If `T` does not have a `CreatedAt` property, this method will throw an exception.
            var entityType = typeof(T);
            var createdAtProperty = entityType.GetProperty("CreatedAt");
            if (createdAtProperty == null)
            {
                throw new InvalidOperationException($"The entity type '{entityType.Name}' does not contain a property named 'CreatedAt'.");
            }

            return await DbSet.OrderByDescending(x => EF.Property<DateTime>(x, "CreatedAt")).FirstOrDefaultAsync(cancellationToken);
        }
    }
}

