using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PulseLMS.Migrations
{
    /// <inheritdoc />
    public partial class RemoveQuizLevel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Level",
                schema: "public",
                table: "quizzes");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Level",
                schema: "public",
                table: "quizzes",
                type: "text",
                nullable: false,
                defaultValue: "");
        }
    }
}
