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
        public int MaxCurrentInAmps { get; set; }

        [Key]
        [JsonProperty("ChargeStationId")]
        public int ChargeStationId { get; set; }
        [JsonProperty("ChargeStation")]
        public virtual ChargeStation? ChargeStation { get; set; }
    }
}
