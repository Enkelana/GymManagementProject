using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GymManagementProject.Data.Migrations
{
    /// <inheritdoc />
    public partial class NewColumnMember : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            
            migrationBuilder.AddColumn<string>(
                name: "RegistrationCard",
                table: "Members",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RegistrationCard",
                table: "Members");
         
        }
    }
}
