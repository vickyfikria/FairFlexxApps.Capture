namespace FairFlexxApps.Capture.Models.DataTemplate
{
    public class DataRadioButtonTemplate : DataControlTemplate
    {
        public string Name { get; set; }
        public bool IsChecked { get; set; }
        
        public DataRadioButtonTemplate(string id, string value, bool isChecked, string name) : base(id, value)
        {
            this.Name = name;
            this.IsChecked = isChecked;
            this.Type = "RadioButton";
        }
    }
}
