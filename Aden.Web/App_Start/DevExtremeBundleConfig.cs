using System.Web.Optimization;

namespace Aden.Web.App_Start
{
    public class DevExtremeBundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            var scriptBundle = new ScriptBundle("~/js/DevExtremeBundle");
            var styleBundle = new StyleBundle("~/css/DevExtremeBundle");
            // CLDR scripts
            scriptBundle
                .Include("~/js/cldr.js")
                .Include("~/js/cldr/event.js")
                .Include("~/js/cldr/supplemental.js")
                .Include("~/js/cldr/unresolved.js");
            // Globalize 1.x
            scriptBundle
                .Include("~/js/globalize.js")
                .Include("~/js/globalize/message.js")
                .Include("~/js/globalize/number.js")
                .Include("~/js/globalize/currency.js")
                .Include("~/js/globalize/date.js");
            // NOTE: jQuery may already be included in the default script bundle. Check the BundleConfig.cs file​​​
            // scriptBundle
            //    .Include("~/Scripts/jquery-1.10.2.js");
            // JSZip for client-side exporting
            // scriptBundle
            //    .Include("~/Scripts/jszip.js");
            // DevExtreme + extensions
            scriptBundle
                .Include("~/js/dx.all.js")
                .Include("~/js/aspnet/dx.aspnet.data.js")
                .Include("~/js/aspnet/dx.aspnet.mvc.js");
            // VectorMap data
            // scriptBundle
            //    .Include("~/Scripts/vectormap-data/africa.js")
            //    .Include("~/Scripts/vectormap-data/canada.js")
            //    .Include("~/Scripts/vectormap-data/eurasia.js")
            //    .Include("~/Scripts/vectormap-data/europe.js")
            //    .Include("~/Scripts/vectormap-data/usa.js")
            //    .Include("~/Scripts/vectormap-data/world.js");
            // DevExtreme themes              
            styleBundle
                .Include("~/css/dx.common.css")
                .Include("~/css/dx.light.css");
            bundles.Add(scriptBundle);
            bundles.Add(styleBundle);
#if !DEBUG
            BundleTable.EnableOptimizations = true;
#endif
        }
    }
}
