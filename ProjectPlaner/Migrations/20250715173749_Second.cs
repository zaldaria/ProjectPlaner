using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjectPlaner.Migrations
{
    /// <inheritdoc />
    public partial class Second : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_tasks_AspNetUsers_userId",
                table: "tasks");

            migrationBuilder.AlterColumn<string>(
                name: "userId",
                table: "tasks",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddColumn<string>(
                name: "userId",
                table: "projects",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_projects_userId",
                table: "projects",
                column: "userId");

            migrationBuilder.AddForeignKey(
                name: "FK_projects_AspNetUsers_userId",
                table: "projects",
                column: "userId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_tasks_AspNetUsers_userId",
                table: "tasks",
                column: "userId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_projects_AspNetUsers_userId",
                table: "projects");

            migrationBuilder.DropForeignKey(
                name: "FK_tasks_AspNetUsers_userId",
                table: "tasks");

            migrationBuilder.DropIndex(
                name: "IX_projects_userId",
                table: "projects");

            migrationBuilder.DropColumn(
                name: "userId",
                table: "projects");

            migrationBuilder.AlterColumn<string>(
                name: "userId",
                table: "tasks",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_tasks_AspNetUsers_userId",
                table: "tasks",
                column: "userId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
