using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Infrastructure.Database.Migrations
{
    /// <inheritdoc />
    public partial class SchoolModulesRefactor : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "modules",
                schema: "public",
                table: "schools",
                newName: "modules_legacy");

            migrationBuilder.CreateTable(
                name: "school_modules",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    key = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    public_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<Guid>(type: "uuid", nullable: true),
                    last_modified_by = table.Column<Guid>(type: "uuid", nullable: true),
                    last_modified = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    deleted_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_school_modules", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "school_module_assignments",
                schema: "public",
                columns: table => new
                {
                    module_id = table.Column<int>(type: "integer", nullable: false),
                    school_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_school_module_assignments", x => new { x.module_id, x.school_id });
                    table.ForeignKey(
                        name: "fk_school_module_assignments_school_modules_module_id",
                        column: x => x.module_id,
                        principalSchema: "public",
                        principalTable: "school_modules",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_school_module_assignments_schools_school_id",
                        column: x => x.school_id,
                        principalSchema: "public",
                        principalTable: "schools",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_school_module_assignments_school_id",
                schema: "public",
                table: "school_module_assignments",
                column: "school_id");

            migrationBuilder.CreateIndex(
                name: "ix_school_modules_key",
                schema: "public",
                table: "school_modules",
                column: "key",
                unique: true);

            migrationBuilder.Sql("""
                INSERT INTO public.school_modules
                (id, name, key, description, public_id, created, created_by, last_modified_by, last_modified, is_deleted, deleted_at)
                VALUES
                (1, 'Teachers', 'TEACHERS', 'Manage teachers and teacher records.', 'd57d8e0c-c783-4f55-b78d-650a5f39cf01', TIMESTAMPTZ '2026-05-19 00:00:00+00', NULL, NULL, TIMESTAMPTZ '2026-05-19 00:00:00+00', FALSE, NULL),
                (2, 'Dashboard', 'DASHBOARD', 'View school analytics and high-level metrics.', 'd57d8e0c-c783-4f55-b78d-650a5f39cf02', TIMESTAMPTZ '2026-05-19 00:00:00+00', NULL, NULL, TIMESTAMPTZ '2026-05-19 00:00:00+00', FALSE, NULL),
                (3, 'Settings', 'SETTINGS', 'Configure school-level settings and preferences.', 'd57d8e0c-c783-4f55-b78d-650a5f39cf03', TIMESTAMPTZ '2026-05-19 00:00:00+00', NULL, NULL, TIMESTAMPTZ '2026-05-19 00:00:00+00', FALSE, NULL),
                (4, 'Students', 'STUDENTS', 'Manage student profiles and enrollment data.', 'd57d8e0c-c783-4f55-b78d-650a5f39cf04', TIMESTAMPTZ '2026-05-19 00:00:00+00', NULL, NULL, TIMESTAMPTZ '2026-05-19 00:00:00+00', FALSE, NULL),
                (5, 'Parents', 'PARENTS', 'Manage parent accounts and linked students.', 'd57d8e0c-c783-4f55-b78d-650a5f39cf05', TIMESTAMPTZ '2026-05-19 00:00:00+00', NULL, NULL, TIMESTAMPTZ '2026-05-19 00:00:00+00', FALSE, NULL),
                (6, 'Support', 'SUPPORT', 'Access support tickets and help desk tools.', 'd57d8e0c-c783-4f55-b78d-650a5f39cf06', TIMESTAMPTZ '2026-05-19 00:00:00+00', NULL, NULL, TIMESTAMPTZ '2026-05-19 00:00:00+00', FALSE, NULL),
                (7, 'Suggestion', 'SUGGESTION', 'Submit and review suggestions.', 'd57d8e0c-c783-4f55-b78d-650a5f39cf07', TIMESTAMPTZ '2026-05-19 00:00:00+00', NULL, NULL, TIMESTAMPTZ '2026-05-19 00:00:00+00', FALSE, NULL),
                (8, 'Classroom Management', 'CLASSROOMMANAGEMENT', 'Manage classrooms and class assignments.', 'd57d8e0c-c783-4f55-b78d-650a5f39cf08', TIMESTAMPTZ '2026-05-19 00:00:00+00', NULL, NULL, TIMESTAMPTZ '2026-05-19 00:00:00+00', FALSE, NULL),
                (9, 'Virtual Classroom', 'VIRTUALCLASSROOM', 'Conduct and manage virtual classroom activities.', 'd57d8e0c-c783-4f55-b78d-650a5f39cf09', TIMESTAMPTZ '2026-05-19 00:00:00+00', NULL, NULL, TIMESTAMPTZ '2026-05-19 00:00:00+00', FALSE, NULL),
                (10, 'Assignment', 'ASSIGNMENT', 'Create, assign, and review assignments.', 'd57d8e0c-c783-4f55-b78d-650a5f39cf10', TIMESTAMPTZ '2026-05-19 00:00:00+00', NULL, NULL, TIMESTAMPTZ '2026-05-19 00:00:00+00', FALSE, NULL),
                (11, 'Virtual Meeting', 'VIRTUALMEETING', 'Schedule and manage virtual meetings.', 'd57d8e0c-c783-4f55-b78d-650a5f39cf11', TIMESTAMPTZ '2026-05-19 00:00:00+00', NULL, NULL, TIMESTAMPTZ '2026-05-19 00:00:00+00', FALSE, NULL),
                (12, 'Exams', 'EXAMS', 'Manage exam setup, schedules, and grading.', 'd57d8e0c-c783-4f55-b78d-650a5f39cf12', TIMESTAMPTZ '2026-05-19 00:00:00+00', NULL, NULL, TIMESTAMPTZ '2026-05-19 00:00:00+00', FALSE, NULL),
                (13, 'Lesson Plan', 'LESSONPLAN', 'Create and manage lesson plans.', 'd57d8e0c-c783-4f55-b78d-650a5f39cf13', TIMESTAMPTZ '2026-05-19 00:00:00+00', NULL, NULL, TIMESTAMPTZ '2026-05-19 00:00:00+00', FALSE, NULL),
                (14, 'Admissions', 'ADMISSIONS', 'Manage admission applications and onboarding.', 'd57d8e0c-c783-4f55-b78d-650a5f39cf14', TIMESTAMPTZ '2026-05-19 00:00:00+00', NULL, NULL, TIMESTAMPTZ '2026-05-19 00:00:00+00', FALSE, NULL),
                (15, 'Library', 'LIBRARY', 'Manage digital and physical library resources.', 'd57d8e0c-c783-4f55-b78d-650a5f39cf15', TIMESTAMPTZ '2026-05-19 00:00:00+00', NULL, NULL, TIMESTAMPTZ '2026-05-19 00:00:00+00', FALSE, NULL),
                (16, 'Calendar', 'CALENDAR', 'Manage school events and schedules.', 'd57d8e0c-c783-4f55-b78d-650a5f39cf16', TIMESTAMPTZ '2026-05-19 00:00:00+00', NULL, NULL, TIMESTAMPTZ '2026-05-19 00:00:00+00', FALSE, NULL),
                (17, 'Fees', 'FEES', 'Track fee setup, payments, and balances.', 'd57d8e0c-c783-4f55-b78d-650a5f39cf17', TIMESTAMPTZ '2026-05-19 00:00:00+00', NULL, NULL, TIMESTAMPTZ '2026-05-19 00:00:00+00', FALSE, NULL),
                (18, 'Account Management', 'ACCOUNTMANAGEMENT', 'Manage school accounts and account settings.', 'd57d8e0c-c783-4f55-b78d-650a5f39cf18', TIMESTAMPTZ '2026-05-19 00:00:00+00', NULL, NULL, TIMESTAMPTZ '2026-05-19 00:00:00+00', FALSE, NULL),
                (19, 'Broadcast', 'BROADCAST', 'Send announcements to school users.', 'd57d8e0c-c783-4f55-b78d-650a5f39cf19', TIMESTAMPTZ '2026-05-19 00:00:00+00', NULL, NULL, TIMESTAMPTZ '2026-05-19 00:00:00+00', FALSE, NULL),
                (20, 'Messaging', 'MESSAGING', 'Access internal messaging and communication.', 'd57d8e0c-c783-4f55-b78d-650a5f39cf20', TIMESTAMPTZ '2026-05-19 00:00:00+00', NULL, NULL, TIMESTAMPTZ '2026-05-19 00:00:00+00', FALSE, NULL),
                (21, 'Configuration', 'CONFIGURATION', 'Manage advanced system configurations.', 'd57d8e0c-c783-4f55-b78d-650a5f39cf21', TIMESTAMPTZ '2026-05-19 00:00:00+00', NULL, NULL, TIMESTAMPTZ '2026-05-19 00:00:00+00', FALSE, NULL),
                (22, 'Audit', 'AUDIT', 'Review audit logs and compliance activities.', 'd57d8e0c-c783-4f55-b78d-650a5f39cf22', TIMESTAMPTZ '2026-05-19 00:00:00+00', NULL, NULL, TIMESTAMPTZ '2026-05-19 00:00:00+00', FALSE, NULL);
                """);

            migrationBuilder.Sql("""
                INSERT INTO public.school_module_assignments (module_id, school_id)
                SELECT sm.id, s.id
                FROM public.schools s
                CROSS JOIN LATERAL unnest(string_to_array(COALESCE(s.modules_legacy, ''), ';')) AS module_key
                JOIN public.school_modules sm ON UPPER(TRIM(module_key)) = sm.key;
                """);

            migrationBuilder.DropColumn(
                name: "modules_legacy",
                schema: "public",
                table: "schools");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "modules",
                schema: "public",
                table: "schools",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.Sql("""
                UPDATE public.schools s
                SET modules = COALESCE((
                    SELECT string_agg(sm.key, ';' ORDER BY sm.key)
                    FROM public.school_module_assignments sma
                    JOIN public.school_modules sm ON sm.id = sma.module_id
                    WHERE sma.school_id = s.id
                ), '');
                """);

            migrationBuilder.DropTable(
                name: "school_module_assignments",
                schema: "public");

            migrationBuilder.DropTable(
                name: "school_modules",
                schema: "public");
        }
    }
}
