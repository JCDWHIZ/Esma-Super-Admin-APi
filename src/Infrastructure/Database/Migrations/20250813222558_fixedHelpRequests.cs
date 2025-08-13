using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Database.Migrations
{
    /// <inheritdoc />
    public partial class fixedHelpRequests : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "school_id",
                schema: "public",
                table: "help_requests",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "tenant_help_request_id",
                schema: "public",
                table: "help_requests",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "user_name",
                schema: "public",
                table: "help_requests",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "user_profile_pic",
                schema: "public",
                table: "help_requests",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "school_id",
                schema: "public",
                table: "help_requests");

            migrationBuilder.DropColumn(
                name: "tenant_help_request_id",
                schema: "public",
                table: "help_requests");

            migrationBuilder.DropColumn(
                name: "user_name",
                schema: "public",
                table: "help_requests");

            migrationBuilder.DropColumn(
                name: "user_profile_pic",
                schema: "public",
                table: "help_requests");
        }
    }
}
