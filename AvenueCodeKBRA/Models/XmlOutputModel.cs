using System.Xml.Serialization;

namespace AvenueCodeKBRA.Models;

[XmlRoot("PublishedItem")]
public class XmlOutputModel
{
    [XmlElement("Title")]
    public string Title { get; set; } = string.Empty;

    [XmlElement("Countries")]
    public string Countries { get; set; } = string.Empty;

    [XmlElement("PublishedDate")]
    public DateTime PublishedDate { get; set; }

    [XmlElement("ContactInformation")]
    public ContactInformationXml ContactInformation { get; set; } = new();
}

public class ContactInformationXml
{
    [XmlElement("PersonGroup")]
    public PersonGroup PersonGroup { get; set; } = new();
}

public class PersonGroup
{
    [XmlAttribute("sequence")]
    public int Sequence { get; set; } = 1;

    [XmlElement("Name")]
    public string Name { get; set; } = string.Empty;

    [XmlElement("PersonGroupMember")]
    public PersonGroupMember PersonGroupMember { get; set; } = new();
}

public class PersonGroupMember
{
    [XmlElement("Person")]
    public List<Person> Person { get; set; } = new();
}

public class Person
{
    [XmlElement("FamilyName")]
    public string FamilyName { get; set; } = string.Empty;

    [XmlElement("GivenName")]
    public string GivenName { get; set; } = string.Empty;

    [XmlElement("DisplayName")]
    public string DisplayName { get; set; } = string.Empty;

    [XmlElement("JobTitle")]
    public string JobTitle { get; set; } = string.Empty;

    [XmlElement("ContactInfo")]
    public ContactInfo ContactInfo { get; set; } = new();
}

public class ContactInfo
{
    [XmlElement("Phone")]
    public List<Phone> Phone { get; set; } = new();
    
    [XmlElement("Email")]
    public List<Email> Email { get; set; } = new();
}

public class Phone
{
    [XmlElement("Number")]
    public string Number { get; set; } = string.Empty;
}

public class Email
{
    [XmlElement("Address")]
    public string Address { get; set; } = string.Empty;
}
