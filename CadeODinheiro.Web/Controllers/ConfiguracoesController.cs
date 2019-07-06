using CadeODinheiro.Web.Infrastructure.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CadeODinheiro.Core.DTO;
using Ninject;
using CadeODinheiro.Core.Business.Abstract;
using CadeODinheiro.Core.Entity;
using CadeODinheiro.Core.DI;

namespace CadeODinheiro.Web.Controllers
{
    public class ConfiguracoesController : Controller
    {
        protected IKernel kernel;

        //
        // GET: /Configuracoes/
        [Inject]
        public IParameterBusiness parameterBusiness { get; set; }

        public ConfiguracoesController()
        {
            kernel = new StandardKernel(new NinjectModuleCustom());
            kernel.Inject(this);
        }

        [Auth()]
        public ActionResult Index()
        {
            var paramsModel = new ParamsModel()
            {
                listParams = parameterBusiness.Get.ToList().OrderBy(p => p.sParametro).ToList<Parameter>()
            };
            return View(paramsModel);
        }

        [Auth()]
        public ActionResult GravaParametro(Parameter jsonParam)
        {
            try
            {
                var par = parameterBusiness.Get.FirstOrDefault(p => p.sID == jsonParam.sID);
                if (par == null)
                    return Json(new { Sucesso = false, Mensagem = "Parametro nao Encontrado", Titulo = "Erro" });

                par.sValor = jsonParam.sValor;
                parameterBusiness.Update(par);
            }
            catch (Exception e)
            {
                return Json(new { Sucesso = false, Mensagem = e.Message, Titulo = "Erro" });
            }
            return Json(new { Sucesso = true, Mensagem = "Parâmetro gravado com sucesso!", Titulo = "OK" });
        }


    }
}
