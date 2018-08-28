using System;
using Newtonsoft.Json;

namespace InnovationMonitor.Models
{
    public class BaseModel
    {
        public BaseModel()
        {
            Id = Guid.NewGuid().ToString();
        }

        [JsonProperty("id")]
        public string Id { get; set; }
    }
}
