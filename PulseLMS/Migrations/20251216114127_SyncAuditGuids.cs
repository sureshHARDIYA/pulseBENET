using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PulseLMS.Migrations
{
    /// <inheritdoc />
    public partial class SyncAuditGuids : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Convert audit columns from text -> uuid with safe casts
            migrationBuilder.Sql("""
            DO $$
            BEGIN
              IF EXISTS (
                SELECT 1 FROM information_schema.columns
                WHERE table_schema = 'public' AND table_name = 'quizzes' AND column_name = 'CreatedBy' AND data_type <> 'uuid'
              ) OR EXISTS (
                SELECT 1 FROM information_schema.columns
                WHERE table_schema = 'public' AND table_name = 'quizzes' AND column_name = 'UpdatedBy' AND data_type <> 'uuid'
              ) THEN
                ALTER TABLE public.quizzes
                  ALTER COLUMN "CreatedBy" TYPE uuid USING NULLIF("CreatedBy"::text, '')::uuid,
                  ALTER COLUMN "UpdatedBy" TYPE uuid USING NULLIF("UpdatedBy"::text, '')::uuid;
              END IF;
            END $$;
            """);

            migrationBuilder.Sql("""
            DO $$
            BEGIN
              IF EXISTS (
                SELECT 1 FROM information_schema.columns
                WHERE table_schema = 'public' AND table_name = 'categories' AND column_name = 'CreatedBy' AND data_type <> 'uuid'
              ) OR EXISTS (
                SELECT 1 FROM information_schema.columns
                WHERE table_schema = 'public' AND table_name = 'categories' AND column_name = 'UpdatedBy' AND data_type <> 'uuid'
              ) THEN
                ALTER TABLE public.categories
                  ALTER COLUMN "CreatedBy" TYPE uuid USING NULLIF("CreatedBy"::text, '')::uuid,
                  ALTER COLUMN "UpdatedBy" TYPE uuid USING NULLIF("UpdatedBy"::text, '')::uuid;
              END IF;
            END $$;
            """);

            migrationBuilder.Sql("""
            DO $$
            BEGIN
              IF EXISTS (
                SELECT 1 FROM information_schema.columns
                WHERE table_schema = 'public' AND table_name = 'questions' AND column_name = 'CreatedBy' AND data_type <> 'uuid'
              ) OR EXISTS (
                SELECT 1 FROM information_schema.columns
                WHERE table_schema = 'public' AND table_name = 'questions' AND column_name = 'UpdatedBy' AND data_type <> 'uuid'
              ) THEN
                ALTER TABLE public.questions
                  ALTER COLUMN "CreatedBy" TYPE uuid USING NULLIF("CreatedBy"::text, '')::uuid,
                  ALTER COLUMN "UpdatedBy" TYPE uuid USING NULLIF("UpdatedBy"::text, '')::uuid;
              END IF;
            END $$;
            """);

            migrationBuilder.Sql("""
            DO $$
            BEGIN
              IF EXISTS (
                SELECT 1 FROM information_schema.columns
                WHERE table_schema = 'public' AND table_name = 'question_options' AND column_name = 'CreatedBy' AND data_type <> 'uuid'
              ) OR EXISTS (
                SELECT 1 FROM information_schema.columns
                WHERE table_schema = 'public' AND table_name = 'question_options' AND column_name = 'UpdatedBy' AND data_type <> 'uuid'
              ) THEN
                ALTER TABLE public.question_options
                  ALTER COLUMN "CreatedBy" TYPE uuid USING NULLIF("CreatedBy"::text, '')::uuid,
                  ALTER COLUMN "UpdatedBy" TYPE uuid USING NULLIF("UpdatedBy"::text, '')::uuid;
              END IF;
            END $$;
            """);

            migrationBuilder.Sql("""
            DO $$
            BEGIN
              IF EXISTS (
                SELECT 1 FROM information_schema.columns
                WHERE table_schema = 'public' AND table_name = 'quiz_categories' AND column_name = 'CreatedBy' AND data_type <> 'uuid'
              ) OR EXISTS (
                SELECT 1 FROM information_schema.columns
                WHERE table_schema = 'public' AND table_name = 'quiz_categories' AND column_name = 'UpdatedBy' AND data_type <> 'uuid'
              ) THEN
                ALTER TABLE public.quiz_categories
                  ALTER COLUMN "CreatedBy" TYPE uuid USING NULLIF("CreatedBy"::text, '')::uuid,
                  ALTER COLUMN "UpdatedBy" TYPE uuid USING NULLIF("UpdatedBy"::text, '')::uuid;
              END IF;
            END $$;
            """);

            migrationBuilder.CreateTable(
                name: "quiz_attempts",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    QuizId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: true),
                    Mode = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    StartedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    SubmittedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    LastActivityAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    QuizVersion = table.Column<int>(type: "integer", nullable: false),
                    TotalQuestions = table.Column<int>(type: "integer", nullable: false),
                    AnsweredQuestions = table.Column<int>(type: "integer", nullable: false),
                    MaxScore = table.Column<int>(type: "integer", nullable: false),
                    Score = table.Column<int>(type: "integer", nullable: false),
                    TimeLimitSeconds = table.Column<int>(type: "integer", nullable: true),
                    ExpiresAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_quiz_attempts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_quiz_attempts_quizzes_QuizId",
                        column: x => x.QuizId,
                        principalSchema: "public",
                        principalTable: "quizzes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_quiz_attempts_QuizId_UserId_Status",
                schema: "public",
                table: "quiz_attempts",
                columns: new[] { "QuizId", "UserId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_quiz_attempts_UserId",
                schema: "public",
                table: "quiz_attempts",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "quiz_attempts",
                schema: "public");

            migrationBuilder.AlterColumn<string>(
                name: "UpdatedBy",
                schema: "public",
                table: "quizzes",
                type: "text",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CreatedBy",
                schema: "public",
                table: "quizzes",
                type: "text",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "UpdatedBy",
                schema: "public",
                table: "quiz_categories",
                type: "text",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CreatedBy",
                schema: "public",
                table: "quiz_categories",
                type: "text",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "UpdatedBy",
                schema: "public",
                table: "questions",
                type: "text",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CreatedBy",
                schema: "public",
                table: "questions",
                type: "text",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "UpdatedBy",
                schema: "public",
                table: "question_options",
                type: "text",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CreatedBy",
                schema: "public",
                table: "question_options",
                type: "text",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "UpdatedBy",
                schema: "public",
                table: "categories",
                type: "text",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CreatedBy",
                schema: "public",
                table: "categories",
                type: "text",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);
        }
    }
}
