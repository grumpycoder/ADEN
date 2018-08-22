using Aden.Web.Helpers;

namespace Aden.Web
{
    public static class Constants
    {
        public const string ApplicationName = "Aden";
        public const string LayerName = "WebApp";
        public const string WebApiLayerName = "WebApi";

        public const string SchoolKey = "SCH";
        public const string LeaKey = "LEA";
        public const string StateKey = "SEA";
        public static string AimBaseUrl
        {
            get
            {
                var env = Environment != "Production" ? Environment : string.Empty;
                return $"https://{env}aim.alsde.edu/";
            }
        }

        //TODO: Refactor magic string and match to config
        public const string GlobalAdministratorGroup = "AdenAppGlobalAdministrators";
        public static string LogoutUrl = "aim.alsde.edu/aim/applicationinventory.aspx?logout=";

        public static string TpaAccessKey => AppSettings.Get<string>("TPA_AccessKey");
        public static string DatabaseContextName => "AdenContext";
        public static string Environment => AppSettings.Get<string>("ASPNET_ENV");
        public static string AimApplicationViewKey => AppSettings.Get<string>("ALSDE_AIM_ApplicationViewKey");
        public static string WebServiceUrl => AppSettings.Get<string>("WebServiceUrl");

    }



}
