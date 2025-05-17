using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookShop.Migrations
{
    /// <inheritdoc />
    public partial class AddededBookCascadeDelete : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "BookId1",
                table: "OrderItems",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_OrderItems_BookId1",
                table: "OrderItems",
                column: "BookId1");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderItems_Books_BookId1",
                table: "OrderItems",
                column: "BookId1",
                principalTable: "Books",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderItems_Books_BookId1",
                table: "OrderItems");

            migrationBuilder.DropIndex(
                name: "IX_OrderItems_BookId1",
                table: "OrderItems");

            migrationBuilder.DropColumn(
                name: "BookId1",
                table: "OrderItems");
        }
    }
}
