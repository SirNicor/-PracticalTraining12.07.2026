using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EFRepository.Migrations
{
    /// <inheritdoc />
    public partial class RemoveDateEndFromLesson : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DateEnd",
                table: "Lessons");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<TimeOnly>(
                name: "DateEnd",
                table: "Lessons",
                type: "time without time zone",
                nullable: false,
                defaultValue: new TimeOnly(0, 0, 0));
        }
    }
}
