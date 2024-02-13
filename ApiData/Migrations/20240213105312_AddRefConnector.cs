using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ApiData.Migrations
{
    /// <inheritdoc />
    public partial class AddRefConnector : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Reference",
                schema: "dbo",
                table: "Connectors",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Reference",
                schema: "dbo",
                table: "Connectors");
        }
    }
}
