using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Xml.Serialization;
using FairFlexxApps.Capture.Models.Templates.Controls;

namespace FairFlexxApps.Capture.Models.Templates.Pages
{
    [XmlRoot(ElementName = "row")]
    public class Row
    {
        [XmlAttribute(AttributeName = "column")]
        public int Column { get; set; } // NOT USE FOR XML FILE - BUT NOT  REMOVE PLS

        [XmlElement(ElementName = "text")]
        public ObservableCollection<Text> Texts { get; set; }

        [XmlElement(ElementName = "label")]
        public ObservableCollection<Label> Labels { get; set; }

        [XmlElement(ElementName = "input")]
        public ObservableCollection<Input> Inputs { get; set; }

        [XmlElement(ElementName = "button")]
        public ObservableCollection<Button> Buttons { get; set; }
    }
}
