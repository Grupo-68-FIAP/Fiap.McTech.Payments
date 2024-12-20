using Fiap.McTech.Payments.Domain.Entities;
using Fiap.McTech.Payments.Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Fiap.McTech.Payments.Infra.Repositories
{
    public class RepositoryBase<TEntity> : IRepositoryBase<TEntity> where TEntity : EntityBase
    {
        private bool disposed = false;

        protected DbContext _db;
        protected DbSet<TEntity> _dbSet;

        public RepositoryBase(DbContext context)
        {
            _db = context;
            _dbSet = _db.Set<TEntity>();
        }

        public TEntity Add(TEntity obj)
        {
            _dbSet.Add(obj);
            _db.SaveChanges();
            return obj;
        }

        public async Task<TEntity> AddAsync(TEntity obj)
        {
            await _dbSet.AddAsync(obj);
            await _db.SaveChangesAsync();
            return obj;
        }

        public async Task<List<TEntity>> AddRangeAsync(List<TEntity> obj)
        {
            await _dbSet.AddRangeAsync(obj);
            await _db.SaveChangesAsync();
            return obj;
        }

        public TEntity GetById(Guid id)
        {
            return _dbSet.Find(id);
        }

        public virtual async Task<TEntity> GetByIdAsync(Guid id)
        {
            return await _dbSet.FindAsync(id);
        }

        public virtual async Task<IEnumerable<TEntity>> GetAll()
        {
            return await _dbSet.ToListAsync();
        }

        public void Update(TEntity obj)
        {
            _db.Entry(obj).State = EntityState.Modified;
        }

        public virtual async Task UpdateAsync(TEntity obj)
        {
            _db.Entry(obj).State = EntityState.Modified;
            await _db.SaveChangesAsync();
        }

        public void Remove(TEntity obj)
        {
            _dbSet.Remove(obj);
            _db.SaveChanges();
        }

        public async Task RemoveAsync(TEntity obj)
        {
            _dbSet.Remove(obj);
            await _db.SaveChangesAsync();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
                return;

            if (disposing)
            {
                _db.Dispose();
            }
            disposed = true;
        }

        ~RepositoryBase()
        {
            Dispose(false);
        }
    }
}
