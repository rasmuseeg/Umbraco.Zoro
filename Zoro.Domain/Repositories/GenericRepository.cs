using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Core;
using Umbraco.Core.Persistence;

namespace UmbracoComments.Core.Repositories
{
    public abstract class GenericRepository<TKey, TEntity>
    {
        public DatabaseContext context;

        public GenericRepository(DatabaseContext context)
        {
            this.context = context;
        }

        public virtual IEnumerable<TEntity> Get()
        {
            var sql = new Sql().Select("*")
                .From<TEntity>(this.context.SqlSyntax);

            return this.context.Database.Query<TEntity>(sql);
        }

        public virtual IEnumerable<TEntity> Where(Expression<Func<TEntity, bool>> predicate)
        {
            var sql = new Sql().Select("*")
                .From<TEntity>(this.context.SqlSyntax)
                .Where(predicate, this.context.SqlSyntax);

            return this.context.Database.Query<TEntity>(sql);
        }

        public virtual TEntity GetById(TKey id)
        {
            return this.context.Database.SingleOrDefault<TEntity>(id);
        }

        public virtual void Insert(TEntity entity)
        {
            this.context.Database.Insert(entity);
        }

        public virtual void Update(TEntity entity)
        {
            this.context.Database.Update(entity);
        }

        public virtual void Delete(TEntity entity)
        {
            this.context.Database.Delete(entity);
        }

        public virtual bool Exist(TKey primaryKey)
        {
            return this.context.Database.Exists<TEntity>(primaryKey);
        }

        public virtual Page<TEntity> GetPaged(long currentPage, long pageSize, Expression<Func<TEntity, bool>> predicate)
        {
            var sql = new Sql().Where(predicate, this.context.SqlSyntax);
            var paged = this.context.Database.Page<TEntity>(currentPage, pageSize, sql);
            return paged;
        }
    }
}
