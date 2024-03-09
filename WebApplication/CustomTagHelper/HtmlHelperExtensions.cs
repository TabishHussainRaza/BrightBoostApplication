using Microsoft.AspNetCore.Mvc.Rendering;

namespace WebApplication.CustomTagHelper
{
    public static class HtmlHelperExtensions
    {
        public static string RouteIf(this IHtmlHelper htmlHelper, string routeName, string className)
        {
            var currentRoute = htmlHelper.ViewContext.RouteData.Values["controller"].ToString();

            return (currentRoute == routeName) ? className : string.Empty;
        }
    }

}