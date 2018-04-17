using System.Configuration;
using System.Data.Common;
using System.Reflection;
using System.Web;
using System.Web.Mvc;

namespace Aden.Web.Helpers
{
    public static class HtmlHelpers
    {
        public static IHtmlString RenderDataSource(this HtmlHelper htmlHelper)
        {
            var connectionString = ConfigurationManager.ConnectionStrings["AdenContext"].ConnectionString;
            var builder = new DbConnectionStringBuilder();
            builder.ConnectionString = connectionString;
            return new MvcHtmlString(builder["Data Source"].ToString());
        }

        public static IHtmlString RenderDataName(this HtmlHelper htmlHelper)
        {
            var connectionString = ConfigurationManager.ConnectionStrings["AdenContext"].ConnectionString;
            var builder = new DbConnectionStringBuilder();
            builder.ConnectionString = connectionString;
            return new MvcHtmlString(builder["initial catalog"].ToString());
        }


        public static IHtmlString RenderApplicationName(this HtmlHelper htmlHelper)
        {
            var title = string.Empty;
            var appInstance = htmlHelper.ViewContext.HttpContext.ApplicationInstance;

            var attr = appInstance.GetType().BaseType.Assembly.GetAssemblyAttribute<AssemblyTitleAttribute>(); 

            //Assembly currentAssembly = GetEntryAssembly();
            //var attr = currentAssembly.GetAssemblyAttribute<AssemblyTitleAttribute>();

            //Assembly currentAssembly = Assembly.GetEntryAssembly();
            //if (currentAssembly == null)
            //    currentAssembly = Assembly.GetCallingAssembly();

            //object[] attrs = currentAssembly.GetCustomAttributes(typeof(AssemblyTitleAttribute), true);

            //title = attrs[0].ToString();
            return new MvcHtmlString(attr.Title);

            //Assembly currentAssembly = Assembly.GetEntryAssembly();
            //var attributes = currentAssembly.CustomAttributes.GetType().GetTypeInfo().Assembly.GetCustomAttributes(typeof(AssemblyTitleAttribute));
            //var assemblyTitleAttribute = attributes.SingleOrDefault() as AssemblyTitleAttribute;


            //var appInstance = htmlHelper.ViewContext.HttpContext.ApplicationInstance;
            ////given that you should then be able to do 
            //var assemblyVersion = appInstance.GetType().BaseType.Assembly.GetName().Name;
            ////note the use of the BaseType - see note below
            //return new MvcHtmlString(assemblyVersion.ToString());

            //var title = string.Empty;
            //Assembly currentAssembly = Assembly.GetEntryAssembly();
            //if (currentAssembly == null)
            //    currentAssembly = Assembly.GetCallingAssembly();

            //var targetValue = currentAssembly.GetCustomAttributes(
            //    typeof(AssemblyTitleAttribute), inherit: true);
            //if (targetValue.Length != 0)
            //    title = ((AssemblyTitleAttribute)targetValue[0]).Title;

            //var name = currentAssembly.GetName().Name;
            //return new MvcHtmlString(title);
        }
    }



}
