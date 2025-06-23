using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataLayer.Migrations
{
    /// <inheritdoc />
    public partial class update_v6 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Exercises_Lessons_LessonId",
                table: "Exercises");

            migrationBuilder.DropIndex(
                name: "IX_Exercises_LessonId",
                table: "Exercises");

            migrationBuilder.DropColumn(
                name: "LessonId",
                table: "Exercises");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "LessonId",
                table: "Exercises",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Exercises_LessonId",
                table: "Exercises",
                column: "LessonId");

            migrationBuilder.AddForeignKey(
                name: "FK_Exercises_Lessons_LessonId",
                table: "Exercises",
                column: "LessonId",
                principalTable: "Lessons",
                principalColumn: "Id");
        }
    }
}
