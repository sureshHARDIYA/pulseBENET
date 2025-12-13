using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PulseLMS.Migrations
{
    /// <inheritdoc />
    public partial class QuestionsAndOptions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "questions",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    QuizId = table.Column<Guid>(type: "uuid", nullable: false),
                    Title = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    Type = table.Column<string>(type: "text", nullable: false),
                    SortOrder = table.Column<int>(type: "integer", nullable: false),
                    Points = table.Column<int>(type: "integer", nullable: false),
                    AllowMultipleCorrect = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_questions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_questions_quizzes_QuizId",
                        column: x => x.QuizId,
                        principalSchema: "public",
                        principalTable: "quizzes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "question_options",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    QuestionId = table.Column<Guid>(type: "uuid", nullable: false),
                    SortOrder = table.Column<int>(type: "integer", nullable: false),
                    Text = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    PromptCorrect = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    PromptWrong = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    IsCorrect = table.Column<bool>(type: "boolean", nullable: false),
                    Score = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_question_options", x => x.Id);
                    table.ForeignKey(
                        name: "FK_question_options_questions_QuestionId",
                        column: x => x.QuestionId,
                        principalSchema: "public",
                        principalTable: "questions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_question_options_QuestionId",
                schema: "public",
                table: "question_options",
                column: "QuestionId");

            migrationBuilder.CreateIndex(
                name: "IX_question_options_QuestionId_SortOrder",
                schema: "public",
                table: "question_options",
                columns: new[] { "QuestionId", "SortOrder" });

            migrationBuilder.CreateIndex(
                name: "IX_questions_QuizId",
                schema: "public",
                table: "questions",
                column: "QuizId");

            migrationBuilder.CreateIndex(
                name: "IX_questions_QuizId_SortOrder",
                schema: "public",
                table: "questions",
                columns: new[] { "QuizId", "SortOrder" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "question_options",
                schema: "public");

            migrationBuilder.DropTable(
                name: "questions",
                schema: "public");
        }
    }
}
