using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace capyborrowProject.Migrations
{
    /// <inheritdoc />
    public partial class NewMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsReturned",
                table: "StudentAssignments");

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Lessons",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "IsAutomaticallyClosed",
                table: "Assignments",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "Lessons");

            migrationBuilder.DropColumn(
                name: "IsAutomaticallyClosed",
                table: "Assignments");

            migrationBuilder.AddColumn<bool>(
                name: "IsReturned",
                table: "StudentAssignments",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
