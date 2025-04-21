using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace capyborrowProject.Migrations
{
    /// <inheritdoc />
    public partial class AddIsSubmittableMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsSubmittable",
                table: "Assignments",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsSubmittable",
                table: "Assignments");
        }
    }
}
