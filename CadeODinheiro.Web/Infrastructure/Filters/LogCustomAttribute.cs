using Ninject;
using CadeODinheiro.Core.Business.Abstract;
using CadeODinheiro.Core.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CadeODinheiro.Web.Infrastructure.Filters
{
    public class LogCustomAttribute : ActionFilterAttribute
    {
        [Inject]
        public ILogBusiness logBusiness { get; set; }


        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var log = new Log
            {
                sOperacao = "[Antes Action]",
                sAction = filterContext.ActionDescriptor.ActionName,
                sController = filterContext.ActionDescriptor.ControllerDescriptor.ControllerName,
                sSession = filterContext.HttpContext.Session.SessionID,
                sAgent = filterContext.HttpContext.Request.UserAgent,
                sIP = filterContext.HttpContext.Request.UserHostAddress
            };
            logBusiness.Insert(log);
            base.OnActionExecuting(filterContext);
        }

        public override void OnResultExecuted(ResultExecutedContext filterContext)
        {
            var log = new Log
            {
                sOperacao = "[Após Action]",
                sAction = filterContext.Controller.ControllerContext.RouteData.Values["action"].ToString(),
                sController = filterContext.Controller.ControllerContext.RouteData.Values["controller"].ToString(),
                sSession = filterContext.HttpContext.Session.SessionID,
                sAgent = filterContext.HttpContext.Request.UserAgent,
                sIP = filterContext.HttpContext.Request.UserHostAddress
            };
            logBusiness.Insert(log);
            base.OnResultExecuted(filterContext);
        }

        public override void OnResultExecuting(ResultExecutingContext filterContext)
        {
            var log = new Log
            {
                sOperacao = "[Antes View]",
                sAction = filterContext.Controller.ControllerContext.RouteData.Values["action"].ToString(),
                sController = filterContext.Controller.ControllerContext.RouteData.Values["controller"].ToString(),
                sSession = filterContext.HttpContext.Session.SessionID,
                sAgent = filterContext.HttpContext.Request.UserAgent,
                sIP = filterContext.HttpContext.Request.UserHostAddress
            };
            logBusiness.Insert(log);
            base.OnResultExecuting(filterContext);
        }

        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            var log = new Log
            {
                sOperacao = "[Após View]",
                sAction = filterContext.Controller.ControllerContext.RouteData.Values["action"].ToString(),
                sController = filterContext.Controller.ControllerContext.RouteData.Values["controller"].ToString(),
                sSession = filterContext.HttpContext.Session.SessionID,
                sAgent = filterContext.HttpContext.Request.UserAgent,
                sIP = filterContext.HttpContext.Request.UserHostAddress
            };
            logBusiness.Insert(log);
            base.OnActionExecuted(filterContext);
        }
    }
}