using CadeODinheiro.Core.Business.Abstract;
using CadeODinheiro.Core.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CadeODinheiro.Core.Business.Concrete
{
    public class ParameterBusiness : BaseBusiness<Parameter>, IParameterBusiness
    {
        public override void Insert(Parameter nomeParam)
        {
            if (Get.Any(p => p.sParametro.ToUpper() == nomeParam.sParametro.ToUpper()))
                throw new InvalidOperationException("Já existe parâmetro com este nome cadastrado");
            base.Insert(nomeParam);
        }

        public bool ParameterExist(string nomeParam)
        {
            return Get.Any(p => p.sParametro.ToUpper() == nomeParam.ToUpper());
        }

        public Parameter GetParameter(string nomeParam)
        {
            var parametro = Get.FirstOrDefault(p => p.sParametro.ToUpper() == nomeParam.ToUpper());
            if (parametro == null)
                throw new NullReferenceException("Parametro: " + nomeParam + " não cadastrado.");
            return parametro;
        }
    }
}
