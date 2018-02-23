using System.Collections.Generic;
using System.Xml.Serialization;

namespace ADEN.Web.Models
{
    public class ApplicationUserIds
    {
        public ApplicationUser ApplicationUser { get; set; }
    }

    public class ApplicationUser
    {
        [XmlAttribute(AttributeName = "Name")]
        public string Name { get; set; }
        [XmlAttribute(AttributeName = "Id")]
        public string Id { get; set; }
    }

    public class AimGroup
    {
        public string GroupViewKey { get; set; }
        public string Description { get; set; }
        public List<Site> Sites { get; set; }
    }

    public class Site
    {
        public string SystemCode { get; set; }
        public string SiteCode { get; set; }
        public string SystemName { get; set; }
        public string SiteName { get; set; }
        [XmlAttribute("ID")]
        public string Id { get; set; }
    }

    public class Email
    {
        [XmlAttribute("type")]
        public string Type { get; set; }
        [XmlAttribute("value")]
        public string Address { get; set; }
    }

    public class PhoneNumber
    {
        [XmlAttribute("type")]
        public string Type { get; set; }
        [XmlAttribute("value")]
        public string Number { get; set; }
    }

    public class Person
    {
        public ApplicationUserIds ApplicationUserIds { get; set; }
        [XmlArray(ElementName = "AimGroups")]
        [XmlArrayItem(ElementName = "AimGroup")]
        public List<AimGroup> AimGroups { get; set; }
        [XmlArray(ElementName = "EmailList")]
        [XmlArrayItem(ElementName = "Email")]
        public List<Email> Emails { get; set; }
        [XmlArray(ElementName = "PhoneNumberList")]
        [XmlArrayItem(ElementName = "PhoneNumber")]
        public List<PhoneNumber> PhoneNumbers { get; set; }
        [XmlArray(ElementName = "Sites")]
        [XmlArrayItem(ElementName = "Site")]
        public List<Site> Sites { get; set; }
        [XmlAttribute("id")]
        public string Id { get; set; }
        [XmlAttribute("firstName")]
        public string FirstName { get; set; }
        [XmlAttribute("middleName")]
        public string MiddleName { get; set; }
        [XmlAttribute("lastName")]
        public string LastName { get; set; }
        [XmlAttribute]
        public string Dob { get; set; }
        [XmlAttribute]
        public string IdentityId { get; set; }
        [XmlAttribute]
        public string IdentityGuid { get; set; }
    }
}