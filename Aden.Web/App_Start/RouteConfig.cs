using System.Web.Mvc;
using System.Web.Routing;

namespace Aden.Web
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute("HomeActions", "{action}", new { controller = "Home" });

            routes.MapRoute(
                name: "ReportActionsYear",
                url: "reports/{id}/{datayear}",
                defaults: new { controller = "Home", action = "Reports", id = UrlParameter.Optional, datayear = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "ReportActions",
                url: "reports/{id}",
                defaults: new { controller = "Home", action = "Reports", id = UrlParameter.Optional }
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
                name: "AssignmentActions",
                url: "assignments/{id}",
                defaults: new { controller = "Home", action = "Assignments", id = UrlParameter.Optional }
            );

            //routes.MapRoute(
            //    name: "WorkItemHistoryActions",
            //    url: "workitemhistory/{datayear}/{id}",
            //    defaults: new { controller = "Home", action = "WorkItemHistory", id = UrlParameter.Optional }
            //);

            routes.MapRoute(
                name: "WorkItemHistoryActions",
                url: "workitemhistory/{reportId}",
                defaults: new { controller = "Home", action = "WorkItemHistory", reportId = UrlParameter.Optional }
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
                defaults: new { controller = "Home", action = "FileSpecifications", id = UrlParameter.Optional }
            );
        }
    }
}
