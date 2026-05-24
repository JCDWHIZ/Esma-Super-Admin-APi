using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddSchoolShortCode : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "short_code",
                schema: "public",
                table: "schools",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.Sql("""
                UPDATE public.schools
                SET short_code = UPPER(CONCAT('SCH', id::text))
                WHERE short_code IS NULL OR BTRIM(short_code) = '';
                """);

            migrationBuilder.AlterColumn<string>(
                name: "short_code",
                schema: "public",
                table: "schools",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "ix_schools_short_code",
                schema: "public",
                table: "schools",
                column: "short_code",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_schools_short_code",
                schema: "public",
                table: "schools");

            migrationBuilder.DropColumn(
                name: "short_code",
                schema: "public",
                table: "schools");
        }
    }
}
