namespace FairFlexxApps.Capture.Models.DataTemplate
{
    public abstract class DataControlTemplate
    {
        public string Id { get; set; }
        public string Value { get; set; }
        public string Type { get; set; }

        protected DataControlTemplate()
        {
        }

        protected DataControlTemplate(string id)
        {
            Id = id;
        }

        protected DataControlTemplate(string id, string value)
        {
            Id = id;
            Value = value;
        }

    }
}
