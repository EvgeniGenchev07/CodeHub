using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataLayer.Migrations
{
    /// <inheritdoc />
    public partial class updatev5 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Lectors_Courses_CourseId",
                table: "Lectors");

            migrationBuilder.DropIndex(
                name: "IX_Lectors_CourseId",
                table: "Lectors");

            migrationBuilder.DropColumn(
                name: "CourseId",
                table: "Lectors");

            migrationBuilder.AddColumn<int>(
                name: "LectorId",
                table: "Courses",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Courses_LectorId",
                table: "Courses",
                column: "LectorId");

            migrationBuilder.AddForeignKey(
                name: "FK_Courses_Lectors_LectorId",
                table: "Courses",
                column: "LectorId",
                principalTable: "Lectors",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Courses_Lectors_LectorId",
                table: "Courses");

            migrationBuilder.DropIndex(
                name: "IX_Courses_LectorId",
                table: "Courses");

            migrationBuilder.DropColumn(
                name: "LectorId",
                table: "Courses");

            migrationBuilder.AddColumn<int>(
                name: "CourseId",
                table: "Lectors",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Lectors_CourseId",
                table: "Lectors",
                column: "CourseId");

            migrationBuilder.AddForeignKey(
                name: "FK_Lectors_Courses_CourseId",
                table: "Lectors",
                column: "CourseId",
                principalTable: "Courses",
                principalColumn: "Id");
        }
    }
}
