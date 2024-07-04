using System.Collections.ObjectModel;

namespace FairFlexxApps.Capture.Models.DataTemplate
{
    public class DataBoxDataTemplate
    {
        public string Id { get; set; }
        public ObservableCollection<DataControlTemplate> Controls { get; set; }
    }
}
