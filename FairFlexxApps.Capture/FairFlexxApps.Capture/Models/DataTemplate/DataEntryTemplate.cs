namespace FairFlexxApps.Capture.Models.DataTemplate
{
    public class DataEntryTemplate : DataControlTemplate
    {
        public DataEntryTemplate(string id, string value) : base(id, value)
        {
            this.Type = "Entry";
        }
    }
}
