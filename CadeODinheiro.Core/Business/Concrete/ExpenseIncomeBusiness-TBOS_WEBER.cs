using CadeODinheiro.Core.Business.Abstract;
using CadeODinheiro.Core.Entity;
using Ninject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CadeODinheiro.Core.Business.Concrete
{
    public class ExpenseIncomeBusiness : BaseBusiness<ExpenseIncome>, IExpenseIncomeBusiness
    {
        [Inject]
        public IAccountBusiness accountBusiness { get; set; }

        public override void Insert(ExpenseIncome entity)
        {
            if (entity.iNumeroOcorrencia == 0) entity.iNumeroOcorrencia = 1;
            if (entity.iTotalOcorrencia == 0) entity.iTotalOcorrencia = 1;

            Account account = accountBusiness.Get.FirstOrDefault(a => a.sID == entity.sAccountID);
            bool bSubtrair = false;
            if (entity.CategoryType == Entity.Enum.CategoryType.Despesa) bSubtrair = true;
            if (entity.CategoryType == Entity.Enum.CategoryType.Transferencia && entity.bTransferOrigem) bSubtrair = true;
            if (entity.CategoryType == Entity.Enum.CategoryType.PagamentoCartaoCredito) bSubtrair = true;
            if (bSubtrair)
                account.dSaldo -= entity.dValor;
            else
                account.dSaldo += entity.dValor;
            accountBusiness.Update(account);

            base.Insert(entity);
        }

        public void RegistraExpensePagoCartaoCredito(ExpenseIncome entity)
        {
            Account account = accountBusiness.Get.FirstOrDefault(a => a.sID == entity.sAccountID);
            double dValor = entity.dValor;
            if (entity.CategoryType == Entity.Enum.CategoryType.Receita) dValor = dValor * (-1);
            account.dSaldo += dValor;
            accountBusiness.Update(account);

            entity.bPagoCartaoCredito = true;
            Update(entity);
        }

        public override void Delete(string id)
        {
            ExpenseIncome expenseIncome = Get.FirstOrDefault(e => e.sID == id);
            Account account = accountBusiness.Get.FirstOrDefault(a => a.sID == expenseIncome.sAccountID);
            bool pagamentoCCRealizado = false;
            if (account.AccountType == Entity.Enum.AccountType.CartaoDeCredito && expenseIncome.bPagoCartaoCredito) pagamentoCCRealizado = true;

            if (!pagamentoCCRealizado)
            {
                bool bSomar = false;
                if (expenseIncome.CategoryType == Entity.Enum.CategoryType.Despesa) bSomar = true;
                if (expenseIncome.CategoryType == Entity.Enum.CategoryType.Transferencia && expenseIncome.bTransferOrigem) bSomar = true;
                if (expenseIncome.CategoryType == Entity.Enum.CategoryType.PagamentoCartaoCredito) bSomar = true;
                if (bSomar)
                    account.dSaldo += expenseIncome.dValor;
                else
                    account.dSaldo -= expenseIncome.dValor;
                accountBusiness.Update(account);

                base.Delete(id);
            }
            else
            {
                throw new InvalidOperationException("Lançamento marcado como pago, excluir primeiro o lançamento de pagamento do cartão de crédito!");
            }
        }

        public override void Update(ExpenseIncome entity)
        {
            bool bAtualizarDados = false;
            bool bAlterouValor = false;
            System.Data.Entity.Infrastructure.DbEntityEntry FullDataEntity = DataBaseEntityValue(entity);
            if (!FullDataEntity.OriginalValues["CategoryType"].Equals(FullDataEntity.CurrentValues["CategoryType"])) bAtualizarDados = true;
            if (!FullDataEntity.OriginalValues["sAccountID"].Equals(FullDataEntity.CurrentValues["sAccountID"])) bAtualizarDados = true;
            if (!FullDataEntity.OriginalValues["dValor"].ToString().Equals(FullDataEntity.CurrentValues["dValor"].ToString()))
            {
                bAtualizarDados = true;
                bAlterouValor = true;
            }
            if (bAtualizarDados)
            {
                if (entity.bPagoCartaoCredito)
                {
                    throw new InvalidOperationException("Lançamento marcado como pago, não pode ser alterado o valor, excluir primeiro o lançamento de pagamento do cartão de crédito!");
                }
                else
                {
                    if (entity.CategoryType == Entity.Enum.CategoryType.PagamentoCartaoCredito && bAlterouValor)
                    {
                        throw new InvalidOperationException("Lançamento de pagamento de cartão de crédito não podem alterar valor!");
                    }
                    else
                    {
                        #region Simulando o Delete para reverter o valor do lançamento original
                        string sAccountDeleteID = ((string)FullDataEntity.OriginalValues["sAccountID"]);
                        Account accountDelete = accountBusiness.Get.FirstOrDefault(a => a.sID == sAccountDeleteID);
                        if (accountDelete == null) throw new InvalidOperationException("Problema na identificação da conta original!");
                        Entity.Enum.CategoryType tipoDelete = ((Entity.Enum.CategoryType)FullDataEntity.OriginalValues["CategoryType"]);
                        double valorDelete = ((double)FullDataEntity.OriginalValues["dValor"]);
                        bool bSomar = false;
                        if (tipoDelete == Entity.Enum.CategoryType.Despesa) bSomar = true;
                        if (tipoDelete == Entity.Enum.CategoryType.Transferencia && ((bool)FullDataEntity.OriginalValues["bTransferOrigem"])) bSomar = true;
                        if (tipoDelete == Entity.Enum.CategoryType.PagamentoCartaoCredito) bSomar = true;
                        if (bSomar)
                            accountDelete.dSaldo += valorDelete;
                        else
                            accountDelete.dSaldo -= valorDelete;
                        accountBusiness.Update(accountDelete);
                        #endregion
                        #region Simulando o Insert para atualizar o valor do lançamento alterado
                        string sAccountInsertID = ((string)FullDataEntity.CurrentValues["sAccountID"]);
                        Account accountInsert = accountBusiness.Get.FirstOrDefault(a => a.sID == sAccountInsertID);
                        if (accountInsert == null) throw new InvalidOperationException("Problema na identificação da conta destino!");
                        Entity.Enum.CategoryType tipoInsert = ((Entity.Enum.CategoryType)FullDataEntity.CurrentValues["CategoryType"]);
                        double valorInsert = ((double)FullDataEntity.CurrentValues["dValor"]);
                        bool bSubtrair = false;
                        if (tipoInsert == Entity.Enum.CategoryType.Despesa) bSubtrair = true;
                        if (tipoInsert == Entity.Enum.CategoryType.Transferencia && ((bool)FullDataEntity.CurrentValues["bTransferOrigem"])) bSubtrair = true;
                        if (tipoInsert == Entity.Enum.CategoryType.PagamentoCartaoCredito) bSubtrair = true;
                        if (bSubtrair)
                            accountInsert.dSaldo -= valorInsert;
                        else
                            accountInsert.dSaldo += valorInsert;
                        accountBusiness.Update(accountInsert);
                        #endregion
                    }
                }
            }

            base.Update(entity);
        }
    }
}
