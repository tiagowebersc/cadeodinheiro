using CadeODinheiro.Core.Business.Abstract;
using CadeODinheiro.Core.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CadeODinheiro.Core.Business.Concrete
{
    public class AccountBusiness : BaseBusiness<Account>, IAccountBusiness
    {
        public override void Insert(Account entity)
        {
            entity.dSaldo = 0;
            entity.iDiaFechamentoCC = 1;
            base.Insert(entity);
        }

        public override void Update(Account entity)
        {
            base.Update(entity);
        }
    }
}
