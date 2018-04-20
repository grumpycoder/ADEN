using System.Configuration;
using System.Data.Common;
using System.Reflection;
using System.Security.Claims;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace Aden.Web.Helpers
{
    public static class HtmlHelpers
    {
        public static IHtmlString RenderConfigurationValue(this HtmlHelper htmlHelper, string key)
        {
            var value = ConfigurationManager.AppSettings[key];

            return new MvcHtmlString(value);
        }

        public static IHtmlString RenderDataSource(this HtmlHelper htmlHelper)
        {
            var connectionString = ConfigurationManager.ConnectionStrings["AdenContext"].ConnectionString;
            var builder = new DbConnectionStringBuilder { ConnectionString = connectionString };
            return new MvcHtmlString(builder["Data Source"].ToString());
        }

        public static IHtmlString RenderDataName(this HtmlHelper htmlHelper)
        {
            var connectionString = ConfigurationManager.ConnectionStrings["AdenContext"].ConnectionString;
            var builder = new DbConnectionStringBuilder { ConnectionString = connectionString };
            return new MvcHtmlString(builder["initial catalog"].ToString());
        }

        public static IHtmlString RenderApplicationName(this HtmlHelper htmlHelper)
        {
            var appInstance = htmlHelper.ViewContext.HttpContext.ApplicationInstance;

            var memberInfo = appInstance.GetType().BaseType;
            if (memberInfo != null)
            {
                var attr = memberInfo.Assembly.GetAssemblyAttribute<AssemblyTitleAttribute>();

                return new MvcHtmlString(attr.Title ?? "No Application Title");
            }
            return new MvcHtmlString("No Application Title");
        }

        public static IHtmlString RenderApplicationDescription(this HtmlHelper htmlHelper)
        {
            var appInstance = htmlHelper.ViewContext.HttpContext.ApplicationInstance;

            var memberInfo = appInstance.GetType().BaseType;
            if (memberInfo != null)
            {
                var attr = memberInfo.Assembly.GetAssemblyAttribute<AssemblyDescriptionAttribute>();

                return new MvcHtmlString(attr.Description ?? "No Application Description");
            }
            return new MvcHtmlString("No Application Description");
        }

        public static IHtmlString UserNavBarMenuList(this HtmlHelper htmlHelper, string position)
        {
            var identity = ((ClaimsIdentity)HttpContext.Current.User.Identity);
            var sb = new StringBuilder();
            var env = ConfigurationManager.AppSettings["ASPNET_ENV"].ToLower();
            var baseUrl = "https://devaim.alsde.edu/";
            switch (env)
            {
                case "test":
                    {
                        baseUrl = baseUrl.Replace("dev", "test");
                        break;
                    }
                case "stage":
                    {
                        baseUrl = baseUrl.Replace("dev", "stage");
                        break;
                    }
                case "production":
                    {
                        baseUrl = baseUrl.Replace("dev", "");
                        break;
                    }
            }
            sb.AppendFormat("<ul class='nav navbar-nav navbar-{0}'>", position);

            sb.Append(@"<li class='dropdown'>");
            sb.AppendFormat(
                @"<a href='#' class='dropdown-toggle' data-toggle='dropdown' role='button' aria-haspopup='true' aria-expanded='false'><i class='fa fa-user'></i>&nbsp;{0}<span class='caret'></span></a>",
                identity.Name);

            sb.Append("<ul class='dropdown-menu'>");

            sb.AppendFormat("<li><a href='{0}aim/ApplicationInventory.aspx'><i class='fa fa-home'></i> My Applications</a></li>", baseUrl);
            sb.AppendFormat("<li><a href='{0}aim/UserProfile.aspx'><i class='fa fa-book'></i> User Profile</a></li>", baseUrl);
            sb.AppendFormat("<li><a href='{0}aim/EdDirPositions.aspx'><i class='fa fa-university'></i> EdDir Positions</a></li>", baseUrl);
            sb.Append("<li role='separator' class='divider'></li>");

            sb.Append("<li class='dropdown-header'>AIM Groups and Users</li>");
            sb.AppendFormat("<li><a href='{0}aim/admin/RolesAndUsers.aspx'><i class='fa fa-group'></i> Groups and Users</a></li>", baseUrl);
            sb.AppendFormat("<li><a href='{0}aim/admin/UserMaintenance.aspx'><i class='fa fa-user'></i> User Maintenance</a></li>", baseUrl);
            sb.AppendFormat("<li><a href='{0}aim/alsde/AppMembership.aspx'><i class='fa fa-heartbeat'></i> App Members</a></li>", baseUrl);
            sb.Append("<li role='separator' class='divider'></li>");

            sb.Append("<li class='dropdown-header'>AIM Administration</li>");
            sb.AppendFormat("<li><a href='{0}aim/admin/EditMessages.aspx'><i class='fa fa-comment'></i> Messages</a></li>", baseUrl);
            sb.AppendFormat("<li><a href='{0}aim/admin/WebsitesandApplications.aspx'><i class='fa fa-sitemap'></i> Websites and Applications</a></li>", baseUrl);
            sb.AppendFormat("<li><a href='{0}aim/admin/Groups.aspx''><i class='fa fa-cogs'></i> Group/Subgroup Maintenance</a></li>", baseUrl);
            sb.AppendFormat("<li><a href='{0}aim/alsde/LoadGroups.aspx'><i class='fa fa-cogs'></i> Load Groups</a></li>", baseUrl);
            sb.Append("<li role='separator' class='divider'></li>");

            sb.AppendFormat("<li><a href='{0}aim/Index.aspx?Impersonate=0'><i class='fa fa-stop'></i> Stop Impersonating</a></li>", baseUrl);
            sb.Append("<li role='separator' class='divider'></li>");

            sb.Append("<li><a href='/account/signout'><i class='fa fa-sign-out'></i> Logout</a></li>");
            sb.Append("</ul>");

            sb.Append("<li><a href='/account/signout'><i class='fa fa-sign-out'></i> Logout</a></li>");

            sb.Append("</ul>");

            return MvcHtmlString.Create(sb.ToString());
        }


    }



}
