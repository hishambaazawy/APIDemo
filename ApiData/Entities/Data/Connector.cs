using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiData.Entities.Data
{
    [Serializable]
    public class Connector
    {
        [Key]
        [JsonProperty("ConnectorId")]
        public int ConnectorId { get; init; }
        [Key]
        [JsonProperty("ChargeStationId")]
        public int ChargeStationId { get; set; }
        
        [JsonProperty("MaxCurrentInAmps")]
        public int MaxCurrentInAmps { get; set; }

        [JsonProperty("Reference")]
        public string? Reference { get; set; }

        [JsonProperty("CreationDate")]
        public DateTime CreationDate { get; set; } = DateTime.Now;

        [JsonProperty("CreatedBy")]
        public string CreatedBy { get; set; }

        [JsonProperty("ChargeStation")]
        public virtual ChargeStation? ChargeStation { get; set; }
    }
}
