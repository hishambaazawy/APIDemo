using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiData.Shared.Model
{
    public class GroupModel
    {
        [JsonProperty("GroupId")]
        public int GroupId { get; set; }
        [JsonProperty("Name")]
        public string Name { get; set; }
        [JsonProperty("CapacityInAmps")]
        public int CapacityInAmps { get; set; }
    }
}
