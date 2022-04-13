using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CommentingSystem.Migrations;

public partial class Mig : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "Comments",
            columns: table => new
            {
                CommentId = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                ParentId = table.Column<int>(type: "int", nullable: true),
                FullName = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: false),
                Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                Content = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                DateCreated = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                DateModified = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Comments", x => x.CommentId);
                table.ForeignKey(
                    name: "FK_Comments_Comments_ParentId",
                    column: x => x.ParentId,
                    principalTable: "Comments",
                    principalColumn: "CommentId",
                    onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateIndex(
            name: "IX_Comments_ParentId",
            table: "Comments",
            column: "ParentId");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "Comments");
    }
}
