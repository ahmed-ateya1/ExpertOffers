﻿using ExpertOffers.Core.RepositoriesContract;
using ExpertOffers.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ExpertOffers.Infrastructure.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        private readonly ApplicationDbContext _db;
        private readonly DbSet<T> _dbSet;

        public GenericRepository(ApplicationDbContext db)
        {
            _db = db;
            _dbSet = _db.Set<T>();
        }

        public async Task AddRangeAsync(IEnumerable<T> model)
        {
            await _dbSet.AddRangeAsync(model);
            await SaveAsync();
        }

        public async Task<bool> AnyAsync(Expression<Func<T, bool>> filter)
        {
            return await _dbSet.AnyAsync(filter);
        }

        public async Task<int> CountAsync(Expression<Func<T, bool>> filter)
        {
            return await _dbSet.CountAsync(filter);
        }

        public async Task<T> CreateAsync(T model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            await _dbSet.AddAsync(model);
            await SaveAsync();
            return model;
        }

        public async Task<bool> DeleteAsync(T model)
        {
            if (model == null)
                return false;

            _dbSet.Remove(model);
            await SaveAsync();
            return true;
        }

        public void Detach(T entity)
        {
            _db.Entry(entity).State = EntityState.Detached;
        }

        public async Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>>? filter = null, string includeProperties = "", Expression<Func<T, object>>? orderBy = null, int? pageIndex = null , int? pageSize = null)
        {
            IQueryable<T> query = _dbSet;

            if (filter != null)
                query = query.Where(filter);

            if (!string.IsNullOrEmpty(includeProperties))
            {
                foreach (var includeProperty in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(includeProperty);
                }
            }

            if (orderBy != null)
            {
                query = query.OrderBy(orderBy);
            }
            if (pageIndex != null && pageSize != null)
            {
                await query.Skip((int)((pageIndex - 1) * pageSize)).Take((int)pageSize).ToListAsync();
            }
            return query.ToList();
        }


        public async Task<T> GetByAsync(Expression<Func<T, bool>>? filter = null, bool isTracked = true, string includeProperties = "")
        {
            IQueryable<T> query = _dbSet;

            if (!isTracked)
                query = query.AsNoTracking();

            if (filter != null)
                query = query.Where(filter);

            if (!string.IsNullOrEmpty(includeProperties))
            {
                foreach (var includeProperty in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(includeProperty);
                }
            }

            return await query.FirstOrDefaultAsync(filter);
        }

        public Task RemoveRangeAsync(IEnumerable<T> model)
        {
            _dbSet.RemoveRange(model);

            return SaveAsync();
        }

        public async Task SaveAsync()
        {
            await _db.SaveChangesAsync();
        }

        public async Task<T> UpdateAsync(T model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            _dbSet.Update(model);
            await SaveAsync();
            return model;
        }
    }
}
