using System.Collections.ObjectModel;
using System.Xml.Serialization;
using FairFlexxApps.Capture.Models.Templates;
using Page = FairFlexxApps.Capture.Models.Templates.Page;
namespace FairFlexxApps.Capture.Models
{
    [XmlRoot("template")]
    public class Template
    {
        [XmlElement("languages")]
        public Language Language { get; set; }

        [XmlElement("event")]
        public Event Event { get; set; }

        [XmlElement("topmenu")]
        public TopMenu TopMenu { get; set; }

        [XmlElement("version")]
        public double Version { get; set; }

        [XmlElement("page")]
        public ObservableCollection<Page> Pages { get; set; } = new ObservableCollection<Page>();

    }
}
