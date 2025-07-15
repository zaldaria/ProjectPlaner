using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjectPlaner.Migrations
{
    /// <inheritdoc />
    public partial class Third : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "userId",
                table: "clients",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_clients_userId",
                table: "clients",
                column: "userId");

            migrationBuilder.AddForeignKey(
                name: "FK_clients_AspNetUsers_userId",
                table: "clients",
                column: "userId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_clients_AspNetUsers_userId",
                table: "clients");

            migrationBuilder.DropIndex(
                name: "IX_clients_userId",
                table: "clients");

            migrationBuilder.DropColumn(
                name: "userId",
                table: "clients");
        }
    }
}
