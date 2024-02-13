using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiData.Shared.Model
{
    public class ConnectorModel
    {
        [JsonProperty("MaxCurrentInAmps")]
        public int MaxCurrentInAmps { get; set; }

        [JsonProperty("Reference")]
        public string? Reference { get; set; }

        [JsonProperty("ChargeStationId")]
        public int ChargeStationId { get; set; }
    }
}
