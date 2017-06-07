using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace PocDatabase
{
    public class PocRepository<TUnitOfWork, TEntity>
    {
        private const string ID_PROPERTY_NAME = "id";
        private readonly PropertyInfo IdProperty;

        private PocFile<TUnitOfWork> _pocFile;
        private List<TEntity> _objects;

        public PocRepository(PocFile<TUnitOfWork> pocDb)
        {
            this._pocFile = pocDb;
            this._objects = this._pocFile.GetCollection<TEntity>();
            this.IdProperty = this.GetIdProperty();
            ValidateEntityWithoutId();
        }

        public IEnumerable<TEntity> GetAll()
        {
            return this.Get(null, null);
        }

        public IEnumerable<TEntity> Get(
            Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            string includeProperties = "")
        {
            var query = _objects.AsQueryable();

            if (filter != null)
                query = query.Where(filter);

            if (orderBy != null)
                query = orderBy(query);

            return query.ToList();
        }

        public IQueryable<TEntity> AsQueryable()
        {
            var query = _objects.AsQueryable();
            return query;
        }

        public void Insert(TEntity entity)
        {
            if (GetIdValue(entity) == Guid.Empty)
                IdProperty.SetValue(entity, Guid.NewGuid());

            if (this._objects.Exists(f => GetIdValue(entity) == GetIdValue(f)))
                throw new DuplicateIdException();

            this._objects.Add(entity);
        }

        public TEntity GetById(Guid id)
        {
            return this.Get(f => GetIdValue(f) == id).FirstOrDefault();
        }

        public void Delete(TEntity entityToDelete)
        {
            Delete(GetIdValue(entityToDelete));
        }

        public void Delete(Guid id)
        {
            var entity = this.GetById(id);
            this._objects.Remove(entity);
        }

        public void Update(TEntity entityToUpdate)
        {
            var entityFound = this.GetById(GetIdValue(entityToUpdate));
            var pos = this._objects.IndexOf(entityFound);
            this._objects.Remove(entityFound);
            this._objects.Insert(pos, entityToUpdate);
        }

        private PropertyInfo GetIdProperty()
        {
            var idProperty = typeof(TEntity)
                .GetProperties()
                .Where(f => f.Name.ToLower() == ID_PROPERTY_NAME && f.PropertyType == typeof(Guid))
                .FirstOrDefault();
            return idProperty;
        }

        private Guid GetIdValue(TEntity entity)
        {
            if (IdProperty != null)
                return (Guid)IdProperty.GetValue(entity);
            return Guid.Empty;
        }

        private void ValidateEntityWithoutId()
        {
            if (IdProperty == null)
                throw new PropertyIdNotExistsException(typeof(TEntity));
        }
    }
}