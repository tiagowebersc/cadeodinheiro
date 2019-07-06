using CadeODinheiro.Web.Infrastructure.DependencyResolver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CadeODinheiro.Web.Infrastructure.FilterProvider
{
    public class FilterProviderCustom : FilterAttributeFilterProvider
    {
        public override IEnumerable<Filter> GetFilters(ControllerContext controllerContext, ActionDescriptor actionDescriptor)
        {
            var filters = base.GetFilters(controllerContext, actionDescriptor);

            var dependencyResolver = (NinjectDependencyResolver)System.Web.Mvc.DependencyResolver.Current;

            if (dependencyResolver != null)
            {
                foreach (var filter in filters)
                {
                    dependencyResolver.kernel.Inject(filter.Instance);
                }
            }

            return filters;
        }
    }
}