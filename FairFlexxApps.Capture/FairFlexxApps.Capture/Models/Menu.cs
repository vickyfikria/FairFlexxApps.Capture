using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace RestDemo.Model
{
    [XmlRoot("menu")]
    public class Menu
    {
        [XmlElement("item")]
        public ObservableCollection<XmlPizzaDetails> item { get; set; }
    }
}
