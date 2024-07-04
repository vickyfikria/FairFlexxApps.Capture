using System.Collections.ObjectModel;

namespace FairFlexxApps.Capture.Models.DataTemplate
{
    public class DataBindableRadioGroupTemplate : DataControlTemplate
    {
        public string Name { get; set; }
        public ObservableCollection<DataRadioButtonTemplate> Radios { get; set; }

        public DataBindableRadioGroupTemplate(string id, ObservableCollection<DataRadioButtonTemplate> radios, string name) : base(id)
        {
            this.Name = name;
            this.Radios = radios;
            this.Type = "BindableRadioGroup";
        }

    }
}
