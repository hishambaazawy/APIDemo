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
    public class UserRole
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [JsonProperty("UserRoleId")]
        public int UserRoleId { get; set; }

        [JsonProperty("UserId")]
        public int UserId { get; set; }

        [JsonProperty("User")]
        public virtual User? User { get; set; }


        [JsonProperty("RoleId")]
        public int RoleId { get; set; }

        [JsonProperty("Role")]
        public virtual Role? Role { get; set; }

        [JsonProperty("IsActive")]
        public bool IsActive { get; set; } = true;

    }
}
