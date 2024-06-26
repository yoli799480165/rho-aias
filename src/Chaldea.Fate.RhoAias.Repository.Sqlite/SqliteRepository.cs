﻿using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

namespace Chaldea.Fate.RhoAias.Repository.Sqlite
{
    internal class SqliteRepository<TEntity> : IRepository<TEntity> where TEntity : class
    {
        private readonly IDbContextFactory<RhoAiasDbContext> _contextFactory;

        public SqliteRepository(IDbContextFactory<RhoAiasDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate)
        {
            await using var context = _contextFactory.CreateDbContext();
            return await context.Set<TEntity>().AnyAsync(predicate);
        }

        public async Task<int> CountAsync()
        {
            await using var context = _contextFactory.CreateDbContext();
            return await context.Set<TEntity>().CountAsync();
        }

        public async Task<TEntity> InsertAsync(TEntity entity)
        {
            await using var context = _contextFactory.CreateDbContext();
            var r = await context.Set<TEntity>().AddAsync(entity);
            await context.SaveChangesAsync();
            return r.Entity;
        }

        public async Task InsertManyAsync(IEnumerable<TEntity> entities)
        {
            await using var context = _contextFactory.CreateDbContext();
            await context.Set<TEntity>().AddRangeAsync(entities);
            await context.SaveChangesAsync();
        }

        public async Task<TEntity> UpdateAsync(TEntity entity)
        {
            await using var context = _contextFactory.CreateDbContext();
            var r = context.Set<TEntity>().Update(entity);
            await context.SaveChangesAsync();
            return r.Entity;
        }

        public async Task UpdateManyAsync(IEnumerable<TEntity> entities)
        {
            await using var context = _contextFactory.CreateDbContext();
            context.Set<TEntity>().UpdateRange(entities);
            await context.SaveChangesAsync();
        }

        public async Task DeleteAsync(TEntity entity)
        {
            await using var context = _contextFactory.CreateDbContext();
            context.Set<TEntity>().Remove(entity);
            await context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Expression<Func<TEntity, bool>> predicate)
        {
            await using var context = _contextFactory.CreateDbContext();
            // NOTE: ExecuteDelete does not need a SaveChanges
            await context.Set<TEntity>().Where(predicate).ExecuteDeleteAsync();
        }

        public async Task DeleteManyAsync(IEnumerable<TEntity> entities)
        {
            await using var context = _contextFactory.CreateDbContext();
            context.Set<TEntity>().RemoveRange(entities);
            await context.SaveChangesAsync();
        }

        public async Task<TEntity?> GetAsync(Expression<Func<TEntity, bool>> predicate, params Expression<Func<TEntity, object>>[] includes)
        {
            await using var context = _contextFactory.CreateDbContext();
            var query = context.Set<TEntity>().AsQueryable();
            if (includes is { Length: > 0 })
            {
                foreach (var propertySelector in includes)
                {
                    query = query.Include(propertySelector);
                }
            }

            return await query.Where(predicate).FirstOrDefaultAsync();
        }

        public async Task<List<TEntity>> GetListAsync(params Expression<Func<TEntity, object>>[] includes)
        {
            await using var context = _contextFactory.CreateDbContext();
            var query = context.Set<TEntity>().AsQueryable();
            if (includes is { Length: > 0 })
            {
                foreach (var propertySelector in includes)
                {
                    query = query.Include(propertySelector);
                }
            }

            return await query.ToListAsync();
        }

        public async Task<List<TEntity>> GetListAsync(Expression<Func<TEntity, bool>> predicate, params Expression<Func<TEntity, object>>[] includes)
        {
            await using var context = _contextFactory.CreateDbContext();
            var query = context.Set<TEntity>().AsQueryable();
            if (includes is { Length: > 0 })
            {
                foreach (var propertySelector in includes)
                {
                    query = query.Include(propertySelector);
                }
            }

            return await query.Where(predicate).ToListAsync();
        }
    }
}