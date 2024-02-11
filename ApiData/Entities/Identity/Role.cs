using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiData.Entities.Identity
{
    [Serializable]
    public class Role
    {
        public Role()
        {
            UserRoles = new HashSet<UserRole>();
        }
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [JsonProperty("RoleId")]
        public int RoleId { get; set; }
       
        [JsonProperty("RoleName")]
        public string RoleName { get; set; }
       
        [JsonProperty("RoleDescription")]
        public string RoleDescription { get; set; }
      
        [JsonProperty("UserRoles")]
        public virtual ICollection<UserRole> UserRoles { get; set; }
      
    }
}
