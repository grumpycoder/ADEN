﻿using Aden.Core.Helpers;

namespace Aden.Core
{
    public static class Constants
    {
        public static string Development = AppSettings.Get<string>("Developers");

        public static string GlobalAdministrators = AppSettings.Get<string>("GlobalAdministratorsGroupName");

        public const string FileSpecificationAdministratorGroup = "AdenAppSpecificationAdministrators";

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

        public static int CommandTimeout => AppSettings.Get<int>("CommandTimeout");
    }



}
