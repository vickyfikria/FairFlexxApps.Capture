using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace FairFlexxApps.Capture.Models
{
    public class NewContactModel
    {
        public string Title { get; set; }
        public ObservableCollection<string> LanguageButtons { get; set; }
    }
}
