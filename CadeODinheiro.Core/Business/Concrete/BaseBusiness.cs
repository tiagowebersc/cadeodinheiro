using Ninject;
using CadeODinheiro.Core.Business.Abstract;
using CadeODinheiro.Core.Entity;
using CadeODinheiro.Core.Repository.Abstract;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity.Infrastructure;

namespace CadeODinheiro.Core.Business.Concrete
{
    public class BaseBusiness<T> : IBusiness<T> where T : BaseEntity
    {
        [Inject]
        public IRepository<T> Repository { get; set; }

        public void AbilitarValidacaoAoSalvar(bool abilitar)
        {
            Repository.AbilitarValidacaoAoSalvar(abilitar);
        }

        public DbEntityEntry DataBaseEntityValue(T entity)
        {
            return Repository.DataBaseEntityValue(entity);
        }

        public virtual IQueryable<T> Get
        {
            get { return Repository.Get; }
        }

        public virtual void Insert(T entity)
        {
            Repository.Insert(entity);
        }

        public virtual void Update(T entity)
        {
            Repository.Update(entity);
        }

        public virtual void Delete(string id)
        {
            Repository.Delete(id);
        }
    }
}
