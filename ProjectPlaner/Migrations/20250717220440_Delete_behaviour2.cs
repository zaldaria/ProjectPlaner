using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjectPlaner.Migrations
{
    /// <inheritdoc />
    public partial class Delete_behaviour2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_clients_AspNetUsers_userId",
                table: "clients");

            migrationBuilder.DropForeignKey(
                name: "FK_projects_AspNetUsers_userId",
                table: "projects");

            migrationBuilder.DropForeignKey(
                name: "FK_projects_clients_clientId",
                table: "projects");

            migrationBuilder.DropForeignKey(
                name: "FK_tasks_AspNetUsers_userId",
                table: "tasks");

            migrationBuilder.DropForeignKey(
                name: "FK_tasks_projects_projectId",
                table: "tasks");

            migrationBuilder.AddForeignKey(
                name: "FK_clients_AspNetUsers_userId",
                table: "clients",
                column: "userId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_projects_AspNetUsers_userId",
                table: "projects",
                column: "userId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_projects_clients_clientId",
                table: "projects",
                column: "clientId",
                principalTable: "clients",
                principalColumn: "clientId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_tasks_AspNetUsers_userId",
                table: "tasks",
                column: "userId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_tasks_projects_projectId",
                table: "tasks",
                column: "projectId",
                principalTable: "projects",
                principalColumn: "projectId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_clients_AspNetUsers_userId",
                table: "clients");

            migrationBuilder.DropForeignKey(
                name: "FK_projects_AspNetUsers_userId",
                table: "projects");

            migrationBuilder.DropForeignKey(
                name: "FK_projects_clients_clientId",
                table: "projects");

            migrationBuilder.DropForeignKey(
                name: "FK_tasks_AspNetUsers_userId",
                table: "tasks");

            migrationBuilder.DropForeignKey(
                name: "FK_tasks_projects_projectId",
                table: "tasks");

            migrationBuilder.AddForeignKey(
                name: "FK_clients_AspNetUsers_userId",
                table: "clients",
                column: "userId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_projects_AspNetUsers_userId",
                table: "projects",
                column: "userId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_projects_clients_clientId",
                table: "projects",
                column: "clientId",
                principalTable: "clients",
                principalColumn: "clientId",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_tasks_AspNetUsers_userId",
                table: "tasks",
                column: "userId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_tasks_projects_projectId",
                table: "tasks",
                column: "projectId",
                principalTable: "projects",
                principalColumn: "projectId",
                onDelete: ReferentialAction.SetNull);
        }
    }
}
