using System.Web;
using System.Web.Mvc;

namespace TPAPIs_equipo_12B
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
