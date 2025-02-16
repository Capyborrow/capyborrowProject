using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace capyborrowProject.Migrations
{
    /// <inheritdoc />
    public partial class AddedAttendanceMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Attendance",
                table: "Lessons",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Attendance",
                table: "Lessons");
        }
    }
}
