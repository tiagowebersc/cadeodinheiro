
using Ninject;
using CadeODinheiro.Core.Business.Abstract;
using CadeODinheiro.Core.DI;
using CadeODinheiro.Core.Entity;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;

namespace CadeODinheiro.Web.App_Start
{
    public class DataConfig
    {
        protected IKernel kernel;

        #region Injects
        [Inject]
        public IParameterBusiness parameterBusiness { get; set; }
        
        
        #endregion

        public DataConfig()
        {
            kernel = new StandardKernel(new NinjectModuleCustom());
            kernel.Inject(this);
        }

        public void PopularBancoDados()
        {
            //var listaParams = new List<Parameter>();
            //listaParams.Add(new Parameter
            //{
            //    sParametro = "URL_WebServiceERPUsuario",
            //    sValor = "http://tbos.cloudapp.net:9090", 
            //    sDescricao = "URL do webservice utilizado para verificação do usuário"
            //});
            //listaParams.Add(new Parameter
            //{
            //    sParametro = "URL_WebServiceERPPatrimonio",
            //    sValor = "http://tbos.cloudapp.net:9090",
            //    sDescricao = "URL do webservice utilizado para carga/atualização dos patrimônios"
            //});
            //listaParams.Add(new Parameter
            //{
            //    sParametro = "URL_WebServiceERPUser",
            //    sValor = "senior",
            //    sDescricao = "Usuário para conexão com webservice utilizado"
            //});
            //listaParams.Add(new Parameter
            //{
            //    sParametro = "URL_WebServiceERPPass",
            //    sValor = "senior",
            //    sDescricao = "Senha para conexão com webservice utilizado"
            //});
            
            //foreach (var p in listaParams)
            //{
            //    if (!parameterBusiness.ParameterExist(p.sParametro))
            //    {
            //        parameterBusiness.Insert(p);
            //    }
            //}
        }
    }
}