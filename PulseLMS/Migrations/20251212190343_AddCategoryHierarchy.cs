using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PulseLMS.Migrations
{
    /// <inheritdoc />
    public partial class AddCategoryHierarchy : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ParentId",
                schema: "public",
                table: "categories",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_categories_ParentId",
                schema: "public",
                table: "categories",
                column: "ParentId");

            migrationBuilder.AddForeignKey(
                name: "FK_categories_categories_ParentId",
                schema: "public",
                table: "categories",
                column: "ParentId",
                principalSchema: "public",
                principalTable: "categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_categories_categories_ParentId",
                schema: "public",
                table: "categories");

            migrationBuilder.DropIndex(
                name: "IX_categories_ParentId",
                schema: "public",
                table: "categories");

            migrationBuilder.DropColumn(
                name: "ParentId",
                schema: "public",
                table: "categories");
        }
    }
}
