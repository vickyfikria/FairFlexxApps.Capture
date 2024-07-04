
using System.Xml.Serialization;

namespace RestDemo.Model
{
    [XmlRoot("item")]
    public class XmlPizzaDetails
    {
        public string id { get; set; }
        public string item { get; set; }
        public string name { get; set; }
        public string cost { get; set; }
        public string description { get; set; }
    }
}
