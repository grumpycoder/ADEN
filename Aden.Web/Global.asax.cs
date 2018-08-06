﻿using Aden.Web.App_Start;
using Aden.Web.Controllers;
using Aden.Web.Helpers;
using AutoMapper;
using System;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace Aden.Web
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            DevExtremeBundleConfig.RegisterBundles(BundleTable.Bundles);

            Mapper.AssertConfigurationIsValid();
        }

        protected void Application_Error(object sender, EventArgs e)
        {
            var ex = Server.GetLastError();
            if (ex == null)
                return;

            Session["Error"] = ex.ToBetterString();

            int httpStatus;
            string errorControllerAction;

            WebHelpers.GetHttpStatus(ex, out httpStatus);
            switch (httpStatus)
            {
                case 404:
                    errorControllerAction = "NotFound";
                    break;
                default:
                    //Helpers.LogWebError(Constants.ProductName, Constants.LayerName, ex);
                    errorControllerAction = "Index";
                    break;
            }

            var httpContext = ((MvcApplication)sender).Context;
            httpContext.ClearError();
            httpContext.Response.Clear();
            httpContext.Response.StatusCode = httpStatus;
            httpContext.Response.TrySkipIisCustomErrors = true;

            var routeData = new RouteData();
            routeData.Values["controller"] = "Error";
            routeData.Values["action"] = errorControllerAction;

            var controller = new ErrorController();
            ((IController)controller)
                .Execute(new RequestContext(new HttpContextWrapper(httpContext), routeData));
        }
    }
}
