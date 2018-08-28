using System;
using Newtonsoft.Json;

namespace InnovationMonitor.Models
{
    public class TempHumidityModel : BaseModel
    {
        /// <value>The temperature outside.</value>
        [JsonProperty("externalTemp")]
        public string ExternalTemp { get; set; }

        /// <value>The temperature of the beer.</value>
        [JsonProperty("internalTemp")]
        public string InternalTemp { get; set; }

        /// <value>The humidity.</value>
        [JsonProperty("humidity")]
        public string Humidity { get; set; }

        /// <value>Time reading was taken.</value>
        [JsonProperty("timeOfReading")]
        public string TimeOfReading { get; set; }
    }
}
