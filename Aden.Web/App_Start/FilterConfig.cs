﻿using Alsde.Mvc.Logging.Attributes;
using System.Web.Mvc;

namespace Aden.Web
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
            filters.Add(new TrackPerformanceAttribute(Constants.ApplicationName,
                Constants.LayerName));
        }
    }
}
