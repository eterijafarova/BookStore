using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookShop.Migrations
{
    /// <inheritdoc />
    public partial class UpdateContext : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameTable(
                name: "UserRole",
                newName: "UserRoles");

            migrationBuilder.RenameIndex(
                name: "IX_UserRole_userId",
                table: "UserRoles",
                newName: "IX_UserRoles_userId");

            migrationBuilder.RenameIndex(
                name: "IX_UserRole_roleId",
                table: "UserRoles",
                newName: "IX_UserRoles_roleId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameTable(
                name: "UserRoles",
                newName: "UserRole");

            migrationBuilder.RenameIndex(
                name: "IX_UserRoles_userId",
                table: "UserRole",
                newName: "IX_UserRole_userId");

            migrationBuilder.RenameIndex(
                name: "IX_UserRoles_roleId",
                table: "UserRole",
                newName: "IX_UserRole_roleId");
        }
    }
}
