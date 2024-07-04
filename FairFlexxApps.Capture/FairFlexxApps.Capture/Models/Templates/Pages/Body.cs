using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Xml.Serialization;

namespace FairFlexxApps.Capture.Models.Templates.Pages
{
    [XmlRoot("body")]
    public class Body
    {
        [XmlElement("row")]
        public ObservableCollection<Row> Rows { get; set; } = new ObservableCollection<Row>();
    }
}
