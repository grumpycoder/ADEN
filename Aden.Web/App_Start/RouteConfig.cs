using System.Web.Mvc;
using System.Web.Routing;

namespace Aden.Web
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapMvcAttributeRoutes();

            routes.LowercaseUrls = true;

            routes.MapRoute(
                name: "Unauthorized",
                url: "unauthorized",
                defaults: new { controller = "Account", action = "Unauthorized" }
            );

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
                url: "workhistory/{submissionId}",
                defaults: new { controller = "Home", action = "WorkHistory", reportId = UrlParameter.Optional }
            );

            routes.MapRoute(
              name: "WorkReassignmentActions",
              url: "reassign/{workItemId}",
              defaults: new { controller = "Home", action = "Reassign", workItemId = UrlParameter.Optional }
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
                name: "EditGroupMembershipActions",
                url: "editgroupmembership/{id}/{groupName}",
                defaults: new { controller = "Home", action = "EditGroupMembership", Id = UrlParameter.Optional, GroupName = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "UploadErrorReportActions",
                url: "errorreport/{id}",
                defaults: new { controller = "Home", action = "ErrorReport", Id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "UploadReportActions",
                url: "uploadreport/{id}",
                defaults: new { controller = "Home", action = "UploadReport", Id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "SubmissionAuditActions",
                url: "audit/{id}",
                defaults: new { controller = "Home", action = "Audit", Id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "SubmissionReopenAuditActions",
                url: "reopenaudit/{id}",
                defaults: new { controller = "Home", action = "ReopenAudit", Id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "WorkItemImageActions",
                url: "workitemimages/{workItemId}",
                defaults: new { controller = "Home", action = "WorkItemImages", reportId = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Submissions", id = UrlParameter.Optional }
            );
        }
    }
}
