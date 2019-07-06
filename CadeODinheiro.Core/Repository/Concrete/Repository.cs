using CadeODinheiro.Core.Entity;
using CadeODinheiro.Core.Repository.Abstract;
using CadeODinheiro.Core.Repository.Configuration;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CadeODinheiro.Core.Repository.Concrete
{
    public class Repository<T> : IRepository<T> where T : BaseEntity
    {
        protected ConnectionContext Context;

        public DbEntityEntry DataBaseEntityValue(T entity)
        {
            DbEntityEntry full = Context.Entry(entity);
            return full;
        }

        public void AbilitarValidacaoAoSalvar(bool abilitar){
            Context.Configuration.AutoDetectChangesEnabled = abilitar;
            Context.Configuration.ValidateOnSaveEnabled = abilitar;
        }

        public Repository(ConnectionContext contextParam)
        {
            Context = contextParam;            
        }

        public IQueryable<T> Get
        {
            get { return from c in Context.Set<T>() select c; }
        }

        public void Insert(T entity)
        {
            entity.dRecord = DateTime.Now;
            Context.Entry(entity).State = EntityState.Added;
            Context.SaveChanges();
        }

        public void Update(T entity)
        {
            Context.Entry(entity).State = EntityState.Modified;
            Context.SaveChanges();
        }

        public void Delete(string id)
        {
            var entity = Get.First(c => c.sID == id);
            Context.Entry(entity).State = EntityState.Deleted;
            Context.SaveChanges();
        }
    }
}
