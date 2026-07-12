using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EFRepository.Migrations
{
    /// <inheritdoc />
    public partial class RenameNumberOfWeekFromLesson : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "NumberOfWeer",
                table: "Lessons",
                newName: "NumberOfWeek");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "NumberOfWeek",
                table: "Lessons",
                newName: "NumberOfWeer");
        }
    }
}
