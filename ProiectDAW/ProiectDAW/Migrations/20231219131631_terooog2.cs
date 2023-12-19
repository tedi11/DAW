using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProiectDAW.Migrations
{
    /// <inheritdoc />
    public partial class terooog2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AppUserId",
                table: "Projects",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "UserID",
                table: "Comments",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.CreateIndex(
                name: "IX_Projects_AppUserId",
                table: "Projects",
                column: "AppUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Projects_AppUser_AppUserId",
                table: "Projects",
                column: "AppUserId",
                principalTable: "AppUser",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Projects_AppUser_AppUserId",
                table: "Projects");

            migrationBuilder.DropIndex(
                name: "IX_Projects_AppUserId",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "AppUserId",
                table: "Projects");

            migrationBuilder.AlterColumn<int>(
                name: "UserID",
                table: "Comments",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);
        }
    }
}
