using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CadeODinheiro.Web.Infrastructure.Utils
{
    public static class OperacaoData
    {
        public static DateTime retornoDataTimeValido(int ano, int mes, int dia)
        {
            DateTime data = new DateTime();
            bool invalido = true;
            while (invalido)
            {
                try
                {
                    data = new DateTime(ano, mes, dia);
                    invalido = false;
                }
                catch (Exception)
                {
                    dia--;
                };
            }
            return data;
        }
    }
}