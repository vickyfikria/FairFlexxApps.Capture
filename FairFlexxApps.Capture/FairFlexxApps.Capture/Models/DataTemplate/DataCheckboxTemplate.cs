namespace FairFlexxApps.Capture.Models.DataTemplate
{
    public class DataCheckboxTemplate : DataControlTemplate
    {
        public DataCheckboxTemplate()
        {
        }

        public DataCheckboxTemplate(string id, string value,bool isChecked) : base(id,value)
        {
            this.IsChecked = isChecked;
            this.Type = "CheckBox";
        }

        public bool IsChecked { get; set; }

    }
}
