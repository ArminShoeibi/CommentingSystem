using Microsoft.EntityFrameworkCore.Migrations;

namespace CommentingSystem.Migrations;

public partial class init_LikeEntity : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "Likes",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                Ip = table.Column<string>(type: "nvarchar(16)", maxLength: 16, nullable: false),
                CommentId = table.Column<int>(type: "int", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Likes", x => x.Id);
                table.ForeignKey(
                    name: "FK_Likes_Comments_CommentId",
                    column: x => x.CommentId,
                    principalTable: "Comments",
                    principalColumn: "CommentId",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex(
            name: "IX_Likes_CommentId",
            table: "Likes",
            column: "CommentId");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "Likes");
    }
}
