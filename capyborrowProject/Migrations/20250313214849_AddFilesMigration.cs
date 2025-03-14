using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace capyborrowProject.Migrations
{
    /// <inheritdoc />
    public partial class AddFilesMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AssignmentFiles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FileUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FileName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AssignmentId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssignmentFiles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AssignmentFiles_Assignments_AssignmentId",
                        column: x => x.AssignmentId,
                        principalTable: "Assignments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SubmissionFiles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FileUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FileName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StudentId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    AssignmentId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubmissionFiles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SubmissionFiles_StudentAssignments_StudentId_AssignmentId",
                        columns: x => new { x.StudentId, x.AssignmentId },
                        principalTable: "StudentAssignments",
                        principalColumns: new[] { "StudentId", "AssignmentId" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AssignmentFiles_AssignmentId",
                table: "AssignmentFiles",
                column: "AssignmentId");

            migrationBuilder.CreateIndex(
                name: "IX_SubmissionFiles_StudentId_AssignmentId",
                table: "SubmissionFiles",
                columns: new[] { "StudentId", "AssignmentId" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AssignmentFiles");

            migrationBuilder.DropTable(
                name: "SubmissionFiles");
        }
    }
}
