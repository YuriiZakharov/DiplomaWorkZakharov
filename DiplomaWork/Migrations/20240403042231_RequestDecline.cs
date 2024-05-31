using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DiplomaWork.Migrations
{
    /// <inheritdoc />
    public partial class RequestDecline : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DeclineText",
                table: "Requests",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "IsDeclined",
                table: "Requests",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DeclineText",
                table: "Requests");

            migrationBuilder.DropColumn(
                name: "IsDeclined",
                table: "Requests");
        }
    }
}
