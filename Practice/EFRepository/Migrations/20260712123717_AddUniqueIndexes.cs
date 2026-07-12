using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EFRepository.Migrations
{
    /// <inheritdoc />
    public partial class AddUniqueIndexes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Teachers_FIO",
                table: "Teachers",
                column: "FIO",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Subjects_NameSubjects",
                table: "Subjects",
                column: "NameSubjects",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Groups_NameGroup",
                table: "Groups",
                column: "NameGroup",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Classrooms_NumberClassroom",
                table: "Classrooms",
                column: "NumberClassroom",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Teachers_FIO",
                table: "Teachers");

            migrationBuilder.DropIndex(
                name: "IX_Subjects_NameSubjects",
                table: "Subjects");

            migrationBuilder.DropIndex(
                name: "IX_Groups_NameGroup",
                table: "Groups");

            migrationBuilder.DropIndex(
                name: "IX_Classrooms_NumberClassroom",
                table: "Classrooms");
        }
    }
}
