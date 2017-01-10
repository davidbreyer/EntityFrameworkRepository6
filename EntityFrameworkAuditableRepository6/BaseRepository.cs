using EntityFramework.Auditing;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace EntityFrameworkAuditableRepository6.Base
{
    public interface IBaseRepository<C, T> : IDisposable
            where T : class
            where C : AuditDbContext
    {
        IQueryable<T> GetAll();
        IQueryable<T> FindBy(Expression<Func<T, bool>> predicate);
        IQueryable<T> FindByReadOnly(Expression<Func<T, bool>> predicate);
        T Add(T entity);
        void Delete(T entity);
        void Delete(IEnumerable<T> entities);
        void Reload(T entity);
        void ReloadAsync(T entity);
        void Update(T entity, int id);
        void Update(T entity, Expression<Func<T, bool>> predicate);
        void Update(Delta<T> delta, params object[] ids);
        Task<int> SaveAsync();
        int Save();
        T Find(int id);
        T Find(params object[] ids);
        Task<T> FindAsync(int id);
        Task<T> FindAsync(params object[] ids);
        bool Exists(Expression<Func<T, bool>> predicate);
        void AddUpdateIgnoreField(string fieldName);
    }

    public abstract class BaseRepository<C, T> : IBaseRepository<C, T>,
            IDisposable
        where T : class
        where C : AuditDbContext
    {
        public C Context { get; set; }
        public Lazy<List<string>> IgnoreFieldsOnUpdate = new Lazy<List<string>>();

        public void AddUpdateIgnoreField(string fieldName)
        {
            IgnoreFieldsOnUpdate.Value.Add(fieldName);
        }

        public virtual T Find(int id)
        {
            T entity = Context.Set<T>().Find(id);

            return entity;
        }

        public virtual T Find(params object[] ids)
        {
            T entity = Context.Set<T>().Find(ids);

            return entity;
        }
        
        public virtual async Task<T> FindAsync(int id)
        {
            var query = Context.Set<T>().FindAsync(id);
            return await query;
        }

        public virtual async Task<T> FindAsync(params object[] ids)
        {
            var query = Context.Set<T>().FindAsync(ids);
            return await query;
        }

        public virtual IQueryable<T> FindBy(Expression<Func<T, bool>> predicate)
        {
            IQueryable<T> query = Context.Set<T>().Where(predicate);
            return query;
        }

        public virtual bool Exists(Expression<Func<T, bool>> predicate)
        {
            return Context.Set<T>().AsNoTracking().Any(predicate);
        }

        public virtual void Delete(T entity)
        {
            Context.Set<T>().Remove(entity);
        }

        public virtual void Delete(IEnumerable<T> entities)
        {
            foreach (T entity in entities)
            {
                Delete(entity);
            }
        }

        public virtual void Reload(T entity)
        {
            Context.Entry<T>(entity).Reload();
        }

        public async virtual void ReloadAsync(T entity)
        {
            await Context.Entry<T>(entity).ReloadAsync();
        }

        public virtual IQueryable<T> FindByReadOnly(Expression<Func<T, bool>> predicate)
        {
            IQueryable<T> query = Context.Set<T>().AsNoTracking().Where(predicate);
            return query;
        }

        public virtual IQueryable<T> GetAll()
        {
            IQueryable<T> query = Context.Set<T>().AsNoTracking();

            return query;
        }

        public virtual T Add(T entity)
        {
            Context.Set<T>().Add(entity);

            return entity;
        }

        public virtual void Update(T entity, int id)
        {
            var entityToUpdate = Context.Set<T>().Find(id);

            if (entityToUpdate == null) throw new Exception(string.Format("{0} is not a valid identity key for {1}.", id, typeof(T).Name));

            Context.Entry(entityToUpdate).CurrentValues.SetValues(entity);

            MarkIgnoreFields(entityToUpdate);
        }
        
        public virtual void Update(T entity, Expression<Func<T, bool>> predicate)
        {
            var entityToUpdate = Context.Set<T>().Where(predicate).SingleOrDefault();

            if (entityToUpdate == null) throw new Exception(string.Format("{0} is not a valid identity key for {1}.", predicate.ToString(), typeof(T).Name));

            Context.Entry(entityToUpdate).CurrentValues.SetValues(entity);

            MarkIgnoreFields(entityToUpdate);
        }

        public virtual void Update(Delta<T> delta, params object[] ids)
        {
            var entityToUpdate = Context.Set<T>().Find(ids);

            if (entityToUpdate == null) throw new Exception(string.Format("{0} is not a valid identity keys for {1}.", ids.ToString(), typeof(T).Name));

            foreach (string field in delta.UpdatedFields())
            {
                var fieldToSet = typeof(T).GetProperty(field);
                var dataOrigion = delta.Internal.GetType().GetProperty(field);

                fieldToSet.SetValue(entityToUpdate, dataOrigion.GetValue(delta.Internal));
            }

            MarkIgnoreFields(entityToUpdate);
        }

        protected virtual void MarkIgnoreFields(T entityToUpdate)
        {
            if(IgnoreFieldsOnUpdate.IsValueCreated)
            {
                foreach(var item in IgnoreFieldsOnUpdate.Value)
                {
                    Context.Entry(entityToUpdate).Property(item).IsModified = false;
                }
            }
            
        }

        public async virtual Task<int> SaveAsync()
        {
            return await Context.SaveChangesAsync();
        }

        public virtual int Save()
        {
            return Context.SaveChanges();
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    if (Context != null)
                    {
                        Context.Dispose();
                        Context = null;
                    }
                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }
        #endregion

    }
}
