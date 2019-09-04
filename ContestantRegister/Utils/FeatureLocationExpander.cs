using Microsoft.AspNetCore.Mvc.Razor;
using System.Collections.Generic;

namespace ContestantRegister.Utils
{
    public class FeatureLocationExpander : IViewLocationExpander
    {
        public IEnumerable<string> ExpandViewLocations(ViewLocationExpanderContext context, IEnumerable<string> viewLocations)
        {
            //TODO вьюхи перенести в папку с фичами
            //if (context.ControllerName =="Schools")
            //    return new string[] { "/Controllers/{1}/Views/{0}.cshtml", "/Views/Shared/{0}.cshtml" };

            return viewLocations;
        }

        public void PopulateValues(ViewLocationExpanderContext context)
        {
            
        }
    }
}
