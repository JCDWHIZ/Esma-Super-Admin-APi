using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Infrastructure.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddSisAndLmsModules : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "lms_modules",
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
                    table.PrimaryKey("pk_lms_modules", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "sis_modules",
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
                    table.PrimaryKey("pk_sis_modules", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "lms_module_assignments",
                schema: "public",
                columns: table => new
                {
                    module_id = table.Column<int>(type: "integer", nullable: false),
                    school_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_lms_module_assignments", x => new { x.module_id, x.school_id });
                    table.ForeignKey(
                        name: "fk_lms_module_assignments_lms_modules_module_id",
                        column: x => x.module_id,
                        principalSchema: "public",
                        principalTable: "lms_modules",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_lms_module_assignments_schools_school_id",
                        column: x => x.school_id,
                        principalSchema: "public",
                        principalTable: "schools",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "sis_module_assignments",
                schema: "public",
                columns: table => new
                {
                    module_id = table.Column<int>(type: "integer", nullable: false),
                    school_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_sis_module_assignments", x => new { x.module_id, x.school_id });
                    table.ForeignKey(
                        name: "fk_sis_module_assignments_schools_school_id",
                        column: x => x.school_id,
                        principalSchema: "public",
                        principalTable: "schools",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_sis_module_assignments_sis_modules_module_id",
                        column: x => x.module_id,
                        principalSchema: "public",
                        principalTable: "sis_modules",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_lms_module_assignments_school_id",
                schema: "public",
                table: "lms_module_assignments",
                column: "school_id");

            migrationBuilder.CreateIndex(
                name: "ix_lms_modules_key",
                schema: "public",
                table: "lms_modules",
                column: "key",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_sis_module_assignments_school_id",
                schema: "public",
                table: "sis_module_assignments",
                column: "school_id");

            migrationBuilder.CreateIndex(
                name: "ix_sis_modules_key",
                schema: "public",
                table: "sis_modules",
                column: "key",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "lms_module_assignments",
                schema: "public");

            migrationBuilder.DropTable(
                name: "sis_module_assignments",
                schema: "public");

            migrationBuilder.DropTable(
                name: "lms_modules",
                schema: "public");

            migrationBuilder.DropTable(
                name: "sis_modules",
                schema: "public");
        }
    }
}
