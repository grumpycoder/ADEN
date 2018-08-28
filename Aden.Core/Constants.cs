using Aden.Core.Helpers;

namespace Aden.Core
{
    public static class Constants
    {
        public static string HelpDeskEmail = AppSettings.Get<string>("HelpDeskEmail");

        public static string ReplyAddress = AppSettings.Get<string>("ReplyAddress");
        public static string CurrentEnvironment => AppSettings.Get<string>("ASPNET_ENV");

        public static string Url
        {
            get
            {
                var env = CurrentEnvironment != "Production" ? CurrentEnvironment : string.Empty;
                return $"https://{env.ToLower()}aden.alsde.edu/assignments";
            }
        }

    }



}
