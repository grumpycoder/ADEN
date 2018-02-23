using System;
using System.IO;
using System.Text;
using System.Web.Mvc;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using ADEN.Web.Models;

namespace Aden.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly string  AccessKey = "34C3A440-B8A8-4235-88AE-EDD2FA2991BC";

        public ActionResult Logon(string token)
        {
            var svc = new IdemService.Service();
            var data = svc.GetUserDetail(AccessKey, token);
             
            var personXml = data.ChildNodes[0].FirstChild;

            var person = ConvertNode<Person>(personXml);
            
            return RedirectToAction("FileSpecifications", "Home");
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