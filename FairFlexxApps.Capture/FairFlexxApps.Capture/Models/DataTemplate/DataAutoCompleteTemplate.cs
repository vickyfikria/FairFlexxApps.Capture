namespace FairFlexxApps.Capture.Models.DataTemplate
{
    public class DataAutoCompleteTemplate : DataControlTemplate
    {
        public DataAutoCompleteTemplate(string id, string value) : base(id, value) {
            this.Type = "AutoComplete";
        }

    }
}
