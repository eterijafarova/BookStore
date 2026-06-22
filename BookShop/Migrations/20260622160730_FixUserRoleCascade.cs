using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookShop.Migrations
{
    /// <inheritdoc />
    public partial class FixUserRoleCascade : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserRoles_Role",
                table: "UserRoles");

            migrationBuilder.DropForeignKey(
                name: "FK_UserRoles_User",
                table: "UserRoles");

            migrationBuilder.DropPrimaryKey(
                name: "PK__UserRole__CD3149CCDE4D7241",
                table: "UserRoles");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserRoles",
                table: "UserRoles",
                column: "userRoleId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserRoles_Role",
                table: "UserRoles",
                column: "roleId",
                principalTable: "Roles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserRoles_User",
                table: "UserRoles",
                column: "userId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserRoles_Role",
                table: "UserRoles");

            migrationBuilder.DropForeignKey(
                name: "FK_UserRoles_User",
                table: "UserRoles");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserRoles",
                table: "UserRoles");

            migrationBuilder.AddPrimaryKey(
                name: "PK__UserRole__CD3149CCDE4D7241",
                table: "UserRoles",
                column: "userRoleId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserRoles_Role",
                table: "UserRoles",
                column: "roleId",
                principalTable: "Roles",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_UserRoles_User",
                table: "UserRoles",
                column: "userId",
                principalTable: "Users",
                principalColumn: "Id");
        }
    }
}
