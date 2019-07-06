using CadeODinheiro.Core.Entity;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CadeODinheiro.Core.Repository.Abstract
{
    public interface IRepository<T> where T : BaseEntity
    {
        void AbilitarValidacaoAoSalvar(bool abilitar);

        DbEntityEntry DataBaseEntityValue(T entity);

        IQueryable<T> Get { get; }

        void Insert(T entity);

        void Update(T entity);

        void Delete(string id);
    }
        
}
