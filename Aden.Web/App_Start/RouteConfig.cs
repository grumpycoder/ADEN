using System.Web.Mvc;
using System.Web.Routing;

namespace Aden.Web
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.LowercaseUrls = true;

            routes.MapRoute("HomeActions", "{action}", new { controller = "Home" });

            routes.MapRoute(
                name: "ReportActionsByYear",
                url: "reports/{datayear}",
                defaults: new { controller = "Home", action = "Reports", datayear = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "ReportActions",
                url: "reports/{datayear}/{filenumber}",
                defaults: new { controller = "Home", action = "Reports", datayear = UrlParameter.Optional, filenumber = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "DocumentActions",
                url: "document/{id}",
                defaults: new { controller = "Home", action = "Document" }
            );

            routes.MapRoute(
                name: "DownloadActions",
                url: "download/{id}",
                defaults: new { controller = "Home", action = "Download" }
            );

            routes.MapRoute(
                name: "WorkHistoryActions",
                url: "workhistory/{reportId}",
                defaults: new { controller = "Home", action = "WorkHistory", reportId = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "EditFileSpecificationsActions",
                url: "editfilespecification/{id}",
                defaults: new { controller = "Home", action = "EditFileSpecification", Id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "SaveFileSpecificationsActions",
                url: "savespecification",
                defaults: new { controller = "Home", action = "SaveSpecification" }
            );

            routes.MapRoute(
                name: "EditWorkItemsActions",
                url: "editworkitem/{id}",
                defaults: new { controller = "Home", action = "EditWorkItem", Id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Submissions", id = UrlParameter.Optional }
            );
        }
    }
}
