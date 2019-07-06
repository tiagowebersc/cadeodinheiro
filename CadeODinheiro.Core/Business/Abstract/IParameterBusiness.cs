using CadeODinheiro.Core.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CadeODinheiro.Core.Business.Abstract
{
    public interface IParameterBusiness : IBusiness<Parameter>
    {
        bool ParameterExist(string nomeParam);

        Parameter GetParameter(string nomeParam);
    }
}
