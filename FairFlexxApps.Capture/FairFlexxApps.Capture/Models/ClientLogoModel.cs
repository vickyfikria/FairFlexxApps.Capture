using FairFlexxApps.Capture.Enums;
using Newtonsoft.Json;

namespace FairFlexxApps.Capture.Models
{
    public class ClientLogoModel
    {
        [JsonProperty("logoUrl")]
        public string ClientLogoUrl { get; set; }

        [JsonProperty("positionX")]
        public HorizontalPositions ClientLogoPositionX { get; set; }

        [JsonProperty("positionY")]
        public VerticalPostions ClientLogoPositionY { get; set; }
    }
}
