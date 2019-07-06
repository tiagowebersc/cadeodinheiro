using CadeODinheiro.Core.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CadeODinheiro.Core.Business.Abstract
{
    public interface IExpenseIncomeBusiness : IBusiness<ExpenseIncome>
    {
        void RegistraExpensePagoCartaoCredito(ExpenseIncome entity);
    }
}
