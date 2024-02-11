using ApiData.Entities.Data;
using ApiData.Entities.Identity;
using ApiData.Entities.SeedEnumeration;
using ApiData.Shared;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiData.Entities
{
    public class SmartChargingContext : DbContext
    {
        private readonly string _connectionString;
        public SmartChargingContext() : base() { }
        public SmartChargingContext(string connectionString)
        {
            _connectionString = connectionString;
        }
        public SmartChargingContext(DbContextOptions<SmartChargingContext> options) : base(options) { }
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<Group>  Groups { get; set; }
        public DbSet<ChargeStation>  ChargeStations { get; set; }
        public DbSet<Connector>  Connectors { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                if (!string.IsNullOrEmpty(_connectionString))
                    optionsBuilder.UseSqlServer(_connectionString);
                else

                    optionsBuilder.UseSqlServer("");
                 optionsBuilder.UseSqlServer(DefaultConfig.ConnectionString);

            }
        }
        public void Seed(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Role>().HasData(RoleEnum.GetAll().ToArray());
            User AdminUser = new User()
            {
                UserEmail = "hisham.baazawy@gmail.com",
                PasswordHash = Shared.Utility.GenerateHash("wYX%0<|HK09"),
                UserId = 1,
                UserMobile="55781453",
            };
            modelBuilder.Entity<User>().HasData(AdminUser);
            List<UserRole> userRoles = new List<UserRole>() { new UserRole() { UserRoleId = 1, UserId = 1, RoleId = 1 }, new UserRole() { UserRoleId = 2, UserId = 1, RoleId = 2 } };
            modelBuilder.Entity<UserRole>().HasData(userRoles.ToArray());

        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
             modelBuilder.HasDefaultSchema("dbo");
            Seed(modelBuilder);
        }

    }
}
