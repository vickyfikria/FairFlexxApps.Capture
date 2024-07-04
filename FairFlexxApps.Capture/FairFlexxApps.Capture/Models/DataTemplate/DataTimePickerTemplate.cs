using System;
using System.Collections.Generic;
using System.Text;

namespace FairFlexxApps.Capture.Models.DataTemplate
{
    public class DataTimePickerTemplate : DataControlTemplate
    {
        public DataTimePickerTemplate(string id, string value) : base(id, value)
        {
            this.Type = "TimePicker";
        }
    }
}
