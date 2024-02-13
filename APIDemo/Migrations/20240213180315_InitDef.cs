using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace APIDemo.Migrations
{
    /// <inheritdoc />
    public partial class InitDef : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedBy2",
                schema: "dbo",
                table: "Connectors");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CreatedBy2",
                schema: "dbo",
                table: "Connectors",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
