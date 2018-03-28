using System;
using System.Web.Mvc;

namespace Aden.Web.Helpers
{
    public static class Helper
    {

        public static string ToFriendlyString(this Boolean b)
        {
            return b ? "Yes" : "No";
        }

        //public static string BoolLabel(this HtmlHelper, string target, string text)
        //{
        //    return string.Format()
        //}

        public static MvcHtmlString YesNo(this HtmlHelper htmlHelper, bool yesNo)
        {
            var text = yesNo ? "Yes" : "No";
            return new MvcHtmlString(text);
        }

    }
}
