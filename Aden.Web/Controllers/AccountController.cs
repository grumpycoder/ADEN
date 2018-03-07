using System;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System.Xml;
using System.Xml.Serialization;
using ADEN.Web.Models;

namespace Aden.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly string AccessKey = "34C3A440-B8A8-4235-88AE-EDD2FA2991BC";

        public ActionResult LoginCallback(string token)
        {
            //TODO: Create user object from webservice call and create auth ticket and claims
            var svc = new IdemService.Service();
            var data = svc.GetUserDetail(AccessKey, token);

            var personXml = data.ChildNodes[0].FirstChild;

            var person = ConvertNode<Person>(personXml);

            var authTicket = new FormsAuthenticationTicket(
                2,
                person.Emails.FirstOrDefault().Address,
                DateTime.Now,
                DateTime.Now.AddMinutes(FormsAuthentication.Timeout.TotalMinutes),
                false,
                "some token that will be used to access the web service and that you have fetched"
            );
            var authCookie = new HttpCookie(
                FormsAuthentication.FormsCookieName,
                FormsAuthentication.Encrypt(authTicket)
            )
            {
                HttpOnly = true
            };
            Response.AppendCookie(authCookie);
            return RedirectToAction("Submissions", "Home");
            //return null;
        }

        private static T ConvertNode<T>(XmlNode node) where T : class
        {
            MemoryStream stm = new MemoryStream();

            StreamWriter stw = new StreamWriter(stm);
            stw.Write(node.OuterXml);
            stw.Flush();

            stm.Position = 0;

            XmlSerializer ser = new XmlSerializer(typeof(T));
            XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
            ns.Add("", "");


            T result = (ser.Deserialize(stm) as T);
            return result;


        }

    }
}