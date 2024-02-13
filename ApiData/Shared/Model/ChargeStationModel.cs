using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiData.Shared.Model
{
    public class ChargeStationModel
    {
        [JsonProperty("Name")]
        public string Name { get; set; }

        [JsonProperty("GroupId")]
        public int GroupId { get; set; }
       
        public ConnectorModel? Connector { get; set; }
    }
   
}
