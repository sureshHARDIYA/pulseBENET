using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PulseLMS.Migrations
{
    /// <inheritdoc />
    public partial class AuditLog : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "CreatedAt",
                schema: "public",
                table: "quizzes",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                schema: "public",
                table: "quizzes",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "UpdatedAt",
                schema: "public",
                table: "quizzes",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy",
                schema: "public",
                table: "quizzes",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "CreatedAt",
                schema: "public",
                table: "quiz_categories",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                schema: "public",
                table: "quiz_categories",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "UpdatedAt",
                schema: "public",
                table: "quiz_categories",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy",
                schema: "public",
                table: "quiz_categories",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "CreatedAt",
                schema: "public",
                table: "questions",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                schema: "public",
                table: "questions",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "UpdatedAt",
                schema: "public",
                table: "questions",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy",
                schema: "public",
                table: "questions",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "CreatedAt",
                schema: "public",
                table: "question_options",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                schema: "public",
                table: "question_options",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "UpdatedAt",
                schema: "public",
                table: "question_options",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy",
                schema: "public",
                table: "question_options",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "CreatedAt",
                schema: "public",
                table: "categories",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                schema: "public",
                table: "categories",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "UpdatedAt",
                schema: "public",
                table: "categories",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy",
                schema: "public",
                table: "categories",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedAt",
                schema: "public",
                table: "quizzes");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                schema: "public",
                table: "quizzes");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                schema: "public",
                table: "quizzes");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                schema: "public",
                table: "quizzes");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                schema: "public",
                table: "quiz_categories");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                schema: "public",
                table: "quiz_categories");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                schema: "public",
                table: "quiz_categories");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                schema: "public",
                table: "quiz_categories");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                schema: "public",
                table: "questions");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                schema: "public",
                table: "questions");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                schema: "public",
                table: "questions");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                schema: "public",
                table: "questions");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                schema: "public",
                table: "question_options");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                schema: "public",
                table: "question_options");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                schema: "public",
                table: "question_options");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                schema: "public",
                table: "question_options");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                schema: "public",
                table: "categories");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                schema: "public",
                table: "categories");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                schema: "public",
                table: "categories");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                schema: "public",
                table: "categories");
        }
    }
}
