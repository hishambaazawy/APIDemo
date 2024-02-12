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
    public class Group
    {
        public Group() {ChargeStations = new HashSet<ChargeStation>();}
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [JsonProperty("GroupId")]
        public int GroupId { get; set; }
        [JsonProperty("Name")]
        public string Name { get; set; }
        [JsonProperty("CapacityInAmps")]
        public int CapacityInAmps { get; set; }
        [JsonProperty("ChargeStations")]
        public virtual ICollection<ChargeStation> ChargeStations { get; set; }
    }
}
