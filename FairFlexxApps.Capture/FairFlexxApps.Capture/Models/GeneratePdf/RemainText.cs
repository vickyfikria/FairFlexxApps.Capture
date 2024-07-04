using FairFlexxApps.Capture.Enums.Templates;
using FairFlexxApps.Capture.Models.Templates.Controls;

namespace FairFlexxApps.Capture.Models.GeneratePdf
{
    public class RemainText
    {
        public string Text { get; set; }
        public string Value { get; set; }

        public InputType Type { get; set; }

        public bool IsHaveTitle { get; set; }

        public float Index { get; set; }

        public Input InputChildren { get; set; }
    }
}
