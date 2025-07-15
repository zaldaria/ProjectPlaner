using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjectPlaner.Migrations
{
    /// <inheritdoc />
    public partial class Fifth : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "taskMark");

            migrationBuilder.DropTable(
                name: "marks");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "marks",
                columns: table => new
                {
                    markId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    color = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_marks", x => x.markId);
                });

            migrationBuilder.CreateTable(
                name: "taskMark",
                columns: table => new
                {
                    markId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    taskId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_taskMark", x => new { x.markId, x.taskId });
                    table.ForeignKey(
                        name: "FK_taskMark_marks_markId",
                        column: x => x.markId,
                        principalTable: "marks",
                        principalColumn: "markId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_taskMark_tasks_taskId",
                        column: x => x.taskId,
                        principalTable: "tasks",
                        principalColumn: "taskId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_taskMark_taskId",
                table: "taskMark",
                column: "taskId");
        }
    }
}
