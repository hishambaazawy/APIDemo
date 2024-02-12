﻿// <auto-generated />
using ApiData.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace ApiData.Migrations
{
    [DbContext(typeof(SmartChargingContext))]
    [Migration("20240212035301_M")]
    partial class M
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasDefaultSchema("dbo")
                .HasAnnotation("ProductVersion", "8.0.1")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("ApiData.Entities.Data.ChargeStation", b =>
                {
                    b.Property<int>("ChargeStationId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("ChargeStationId"));

                    b.Property<int>("GroupId")
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("ChargeStationId");

                    b.HasIndex("GroupId");

                    b.ToTable("ChargeStations", "dbo");
                });

            modelBuilder.Entity("ApiData.Entities.Data.Connector", b =>
                {
                    b.Property<int>("ConnectorId")
                        .HasColumnType("int");

                    b.Property<int>("ChargeStationId")
                        .HasColumnType("int");

                    b.Property<int>("MaxCurrentInAmps")
                        .HasColumnType("int");

                    b.HasKey("ConnectorId", "ChargeStationId");

                    b.HasIndex("ChargeStationId");

                    b.ToTable("Connectors", "dbo");
                });

            modelBuilder.Entity("ApiData.Entities.Data.Group", b =>
                {
                    b.Property<int>("GroupId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("GroupId"));

                    b.Property<int>("CapacityInAmps")
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("GroupId");

                    b.ToTable("Groups", "dbo");
                });

            modelBuilder.Entity("ApiData.Entities.Identity.Role", b =>
                {
                    b.Property<int>("RoleId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("RoleId"));

                    b.Property<string>("RoleDescription")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("RoleName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("RoleId");

                    b.ToTable("Roles", "dbo");

                    b.HasData(
                        new
                        {
                            RoleId = 1,
                            RoleDescription = "Default User",
                            RoleName = "User"
                        },
                        new
                        {
                            RoleId = 2,
                            RoleDescription = "Administrator role with full access",
                            RoleName = "Admin"
                        });
                });

            modelBuilder.Entity("ApiData.Entities.Identity.User", b =>
                {
                    b.Property<int>("UserId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("UserId"));

                    b.Property<string>("PasswordHash")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UserEmail")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UserMobile")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("UserId");

                    b.ToTable("Users", "dbo");

                    b.HasData(
                        new
                        {
                            UserId = 1,
                            PasswordHash = "mJ2ZEyG33TtrgDsH/e+mBoteYTGil16w0J9qO4EQMhQ=",
                            UserEmail = "hisham.baazawy@gmail.com",
                            UserMobile = "55781453"
                        });
                });

            modelBuilder.Entity("ApiData.Entities.Identity.UserRole", b =>
                {
                    b.Property<int>("UserRoleId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("UserRoleId"));

                    b.Property<bool>("IsActive")
                        .HasColumnType("bit");

                    b.Property<int>("RoleId")
                        .HasColumnType("int");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.HasKey("UserRoleId");

                    b.HasIndex("RoleId");

                    b.HasIndex("UserId");

                    b.ToTable("UserRoles", "dbo");

                    b.HasData(
                        new
                        {
                            UserRoleId = 1,
                            IsActive = true,
                            RoleId = 1,
                            UserId = 1
                        },
                        new
                        {
                            UserRoleId = 2,
                            IsActive = true,
                            RoleId = 2,
                            UserId = 1
                        });
                });

            modelBuilder.Entity("ApiData.Entities.Data.ChargeStation", b =>
                {
                    b.HasOne("ApiData.Entities.Data.Group", "Group")
                        .WithMany("ChargeStations")
                        .HasForeignKey("GroupId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Group");
                });

            modelBuilder.Entity("ApiData.Entities.Data.Connector", b =>
                {
                    b.HasOne("ApiData.Entities.Data.ChargeStation", "ChargeStation")
                        .WithMany("Connectors")
                        .HasForeignKey("ChargeStationId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("ChargeStation");
                });

            modelBuilder.Entity("ApiData.Entities.Identity.UserRole", b =>
                {
                    b.HasOne("ApiData.Entities.Identity.Role", "Role")
                        .WithMany("UserRoles")
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("ApiData.Entities.Identity.User", "User")
                        .WithMany("UserRoles")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Role");

                    b.Navigation("User");
                });

            modelBuilder.Entity("ApiData.Entities.Data.ChargeStation", b =>
                {
                    b.Navigation("Connectors");
                });

            modelBuilder.Entity("ApiData.Entities.Data.Group", b =>
                {
                    b.Navigation("ChargeStations");
                });

            modelBuilder.Entity("ApiData.Entities.Identity.Role", b =>
                {
                    b.Navigation("UserRoles");
                });

            modelBuilder.Entity("ApiData.Entities.Identity.User", b =>
                {
                    b.Navigation("UserRoles");
                });
#pragma warning restore 612, 618
        }
    }
}
