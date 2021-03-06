﻿using EntityFramework.Repository6.Interfaces;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace EntityFramework.Repository6
{
    public abstract class BaseRepository<C, T> :
            IBaseRepository<C, T>
            , IDisposable
        where T : class
        where C : DbContext
    {
        protected C Context { get; set; }
        protected Lazy<List<string>> IgnoreFieldsOnUpdate = new Lazy<List<string>>();
        protected Lazy<List<Expression<Func<T, bool>>>> IgnoreFieldsOnUpdate2 = new Lazy<List<Expression<Func<T, bool>>>>();

        protected BaseRepository(C context)
        {
            Context = context;
        }

        public virtual void AddUpdateIgnoreField(string fieldName)
        {
            IgnoreFieldsOnUpdate.Value.Add(fieldName);
        }

        public virtual void AddUpdateIgnoreField(Expression<Func<T, bool>> fieldName)
        {
            IgnoreFieldsOnUpdate2.Value.Add(fieldName);
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

        public async virtual Task ReloadAsync(T entity)
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

        public virtual IEnumerable<T> Add(IEnumerable<T> entities)
        {
            Context.Set<T>().AddRange(entities);

            return entities;
        }

        public virtual void Update(T entity, int id)
        {
            var entityToUpdate = Context.Set<T>().Find(id);

            if (entityToUpdate == null) throw CreateEntityNotFoundException<ArgumentOutOfRangeException>(id.ToString(), typeof(T).Name);

            Context.Entry(entityToUpdate).CurrentValues.SetValues(entity);

            MarkIgnoreFields(entityToUpdate);
        }

        public virtual void Update(T entity, Expression<Func<T, bool>> predicate)
        {
            var entityToUpdate = Context.Set<T>().Where(predicate).SingleOrDefault();

            if (entityToUpdate == null) throw CreateEntityNotFoundException<ArgumentOutOfRangeException>(predicate.ToString(), typeof(T).Name);

            Context.Entry(entityToUpdate).CurrentValues.SetValues(entity);

            MarkIgnoreFields(entityToUpdate);
        }

        public virtual void Update(Delta<T> delta, params object[] ids)
        {
            var entityToUpdate = Context.Set<T>().Find(ids);

            if (entityToUpdate == null) throw CreateEntityNotFoundException<ArgumentOutOfRangeException>(ids.ToString(), typeof(T).Name);

            foreach (string field in delta.UpdatedFields())
            {
                var fieldToSet = typeof(T).GetProperty(field);
                var dataOrigion = delta.Internal.GetType().GetProperty(field);

                fieldToSet.SetValue(entityToUpdate, dataOrigion.GetValue(delta.Internal));
            }

            MarkIgnoreFields(entityToUpdate);
        }

        public void Update(Delta<T> delta, Expression<Func<T, bool>> predicate)
        {
            var entityToUpdate = Context.Set<T>().Where(predicate).SingleOrDefault();

            if (entityToUpdate == null) throw CreateEntityNotFoundException<ArgumentOutOfRangeException>(predicate.ToString(), typeof(T).Name);

            foreach (string field in delta.UpdatedFields())
            {
                var fieldToSet = typeof(T).GetProperty(field);
                var dataOrigion = delta.Internal.GetType().GetProperty(field);

                fieldToSet.SetValue(entityToUpdate, dataOrigion.GetValue(delta.Internal));
            }

            MarkIgnoreFields(entityToUpdate);
        }

        protected virtual E CreateEntityNotFoundException<E>(string entityId, string entityType)
        {
            return (E)Activator.CreateInstance(typeof(E), string.Format("{0} is not a valid identity key(s) for {1}.", entityId, entityType));
        }

        protected virtual void MarkIgnoreFields(T entityToUpdate)
        {
            if(IgnoreFieldsOnUpdate.IsValueCreated)
            { IgnoreFieldsOnUpdate.Value.ForEach(ignoreField => Context.Entry(entityToUpdate).Property(ignoreField).IsModified = false); }


            if(IgnoreFieldsOnUpdate2.IsValueCreated)
            { IgnoreFieldsOnUpdate2.Value.ForEach(ignoreField => Context.Entry(entityToUpdate).Property(ignoreField).IsModified = false); }
        }

        public async virtual Task<int> SaveAsync()
        {
            return await Context.SaveChangesAsync();
        }

        public virtual int Save()
        {
            return Context.SaveChanges();
        }

        public virtual int Count()
        {
            return Context.Set<T>().Count();
        }

        public async virtual Task<int> CountAsync()
        {
            return await Context.Set<T>().CountAsync();
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
