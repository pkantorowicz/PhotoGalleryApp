using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Gallery.Data.Models;
using Gallery.Data.Repositories;
using Gallery.Infrastructure.EF;

namespace Gallery.Infrastructure.Repositories
{
    public class EntityBaseRepository<T> : IEntityBaseRepository<T>, ISqlRepository
            where T : class, IEntityBase, new()
    {
        private readonly GalleryContext _context;

        public EntityBaseRepository(GalleryContext context)
        {
            _context = context;
        }

        public virtual IEnumerable<T> GetAll()        
            => _context.Set<T>().AsEnumerable();   

        public virtual async Task<IEnumerable<T>> GetAllAsync()       
            => await _context.Set<T>().ToListAsync();

        public virtual IQueryable<T> QueryAll()
            => _context.Set<T>().AsQueryable();

        public virtual IEnumerable<T> AllIncluding(params Expression<Func<T, object>>[] includeProperties)
        {
            IQueryable<T> query = _context.Set<T>();
            query = includeProperties.Aggregate(query, (current, includeProperty) 
                => current.Include(includeProperty));
            return query.AsEnumerable();
        }

        public virtual async Task<IEnumerable<T>> AllIncludingAsync(params Expression<Func<T, object>>[] includeProperties)
        {
            IQueryable<T> query = _context.Set<T>();
            query = includeProperties.Aggregate(query, (current, includeProperty) 
                => current.Include(includeProperty));
            return await query.ToListAsync();
        }

        public T GetSingle(int id)       
            => _context.Set<T>().FirstOrDefault(x => x.Id == id);       

        public T GetSingle(Expression<Func<T, bool>> predicate)       
            => _context.Set<T>().FirstOrDefault(predicate);        

        public T GetSingle(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includeProperties)
        {
            IQueryable<T> query = _context.Set<T>();
            query = includeProperties.Aggregate(query, (current, includeProperty)
                => current.Include(includeProperty));

            return query.Where(predicate).FirstOrDefault();
        }

        public async Task<T> GetSingleAsync(int id)
            => await _context.Set<T>().FirstOrDefaultAsync(e => e.Id == id);  

        public async Task<T> GetSingleAsync(Expression<Func<T, bool>> predicate)      
            => await _context.Set<T>().FirstOrDefaultAsync(predicate);       

        public async Task<T> GetSingleAsync(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includeProperties)
        {
            IQueryable<T> query = _context.Set<T>();
            query = includeProperties.Aggregate(query, (current, includeProperty)
                => current.Include(includeProperty));

            return await query.Where(predicate).FirstOrDefaultAsync();
        }

        public virtual IEnumerable<T> FindBy(Expression<Func<T, bool>> predicate)        
            => _context.Set<T>().Where(predicate);       

        public virtual async Task<IEnumerable<T>> FindByAsync(Expression<Func<T, bool>> predicate)       
            => await _context.Set<T>().Where(predicate).ToListAsync();       

        public virtual async Task AddAsync(T entity)
        {
            EntityEntry dbEntityEntry = _context.Entry<T>(entity);
            await  _context.Set<T>().AddAsync(entity);
        }

        public virtual void Edit(T entity)
        {
            EntityEntry dbEntityEntry = _context.Entry<T>(entity);
            dbEntityEntry.State = EntityState.Modified;
        }
        public virtual void Delete(T entity)
        {
            EntityEntry dbEntityEntry = _context.Entry<T>(entity);
            dbEntityEntry.State = EntityState.Deleted;
        }

        public virtual async Task CommitAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
