using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ApiData.Entities.Identity
{
    [Serializable]
    public class User
    {
        public User()
        {
            UserRoles = new HashSet<UserRole>();
        }
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
       
        [JsonProperty("UserId")]
        public int UserId { get; set; }

        [JsonProperty("UserEmail")]
        public string UserEmail { get; set; }

        [JsonProperty("UserMobile")]
        public string UserMobile { get; set; }

        [JsonProperty("PasswordHash")]
        public string PasswordHash { get; set; }

        [JsonProperty("UserRoles")]
        public virtual ICollection<UserRole> UserRoles { get; set; }
    }

}
