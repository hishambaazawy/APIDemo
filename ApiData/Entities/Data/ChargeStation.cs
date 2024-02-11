using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ApiData.Entities.Data
{
    [Serializable]
    public class ChargeStation
    {
        public ChargeStation() {Connectors=new HashSet<Connector>();}
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [JsonProperty("ChargeStationId")]
        public int ChargeStationId { get; set; }

        [JsonProperty("Name")]
        public string Name { get; set; }

        [JsonProperty("GroupId")]
        public int GroupId { get; set; }
        [JsonProperty("Group")]
        public virtual Group? Group { get; set; }
        [JsonProperty("Connectors")]
        public virtual ICollection<Connector> Connectors { get; set; }
    }

}
