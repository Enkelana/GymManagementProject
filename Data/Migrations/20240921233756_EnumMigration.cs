using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GymManagementProject.Data.Migrations
{
    /// <inheritdoc />
    public partial class EnumMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Time",
                table: "Subscriptions",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Time",
                table: "Subscriptions");
        }
    }
}
