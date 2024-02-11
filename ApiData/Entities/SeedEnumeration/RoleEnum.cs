using ApiData.Entities.Identity;
using ApiData.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiData.Entities.SeedEnumeration
{
    public static class RoleEnum
    {
        public static Role User { get; set; } = new Role() { RoleId = 1, RoleName = BaseRoleEnum.User, RoleDescription = "Default User" };
        public static Role Admin { get; set; } = new Role() { RoleId = 2, RoleName = BaseRoleEnum.Admin, RoleDescription = "Administrator role with full access" };
        public static List<Role> GetAll()
        {
            return new List<Role>() {  User , Admin };
        }
    }
}
