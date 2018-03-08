using System.Collections.Generic;
using System.Xml.Serialization;

namespace Aden.Core
{
    public class Person
    {
        public string Id { get; set; }
        public string IdentityId { get; set; }
        public string IdentityGuid { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string Dob { get; set; }

        public ApplicationUserIds ApplicationUserIds { get; set; }
        public List<Group> Groups { get; set; }
        public List<Email> Emails { get; set; }
        public List<PhoneNumber> PhoneNumbers { get; set; }
        public List<Site> Sites { get; set; }

        public Person()
        {
            Groups = new List<Group>();
        }
    }



    public class ApplicationUserIds
    {
        public ApplicationUser ApplicationUser { get; set; }
    }

    public class ApplicationUser
    {
    }

    public class Group
    {
        public string GroupViewKey { get; set; }
        public string Description { get; set; }
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


}
