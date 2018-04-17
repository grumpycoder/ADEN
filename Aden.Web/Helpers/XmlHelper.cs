using System;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

namespace Aden.Web.Helpers
{

    /// <summary>
    ///   Extensions for supporting xml serialization by <see cref = "XmlSerializer" />
    /// </summary>
    public static class XmlSerializerExtensions
    {
        public static T ParseToClass<T>(this string xml) where T : class
        {
            try
            {
                if (!string.IsNullOrEmpty(xml) && !string.IsNullOrWhiteSpace(xml))
                {
                    XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));
                    using (StringReader stringReader = new StringReader(xml))
                    {
                        return (T)xmlSerializer.Deserialize(stringReader);
                    }
                }
            }
            catch (Exception ex)
            {
                //Log error
                //throw new Exception(string.Format("Failed to deserialize to class {0}", typeof(T)), ex);
            }
            return null;
        }

    }

    public static class AssemblyExtensions {

        public static T GetAssemblyAttribute<T>(this System.Reflection.Assembly ass) where T : Attribute
        {
            object[] attributes = ass.GetCustomAttributes(typeof(T), false);
            if (attributes == null || attributes.Length == 0)
                return null;
            return attributes.OfType<T>().SingleOrDefault();
        }

    }

}