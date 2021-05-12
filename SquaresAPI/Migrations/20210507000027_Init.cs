using Microsoft.EntityFrameworkCore.Migrations;

namespace SquaresAPI.Migrations
{
    public partial class Init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PointsLists",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SquaresNeedToBeUpdated = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PointsLists", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PointsLists_Points",
                columns: table => new
                {
                    PointsListId = table.Column<int>(type: "int", nullable: false),
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    X = table.Column<int>(type: "int", nullable: false),
                    Y = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PointsLists_Points", x => new { x.PointsListId, x.Id });
                    table.ForeignKey(
                        name: "FK_PointsLists_Points_PointsLists_PointsListId",
                        column: x => x.PointsListId,
                        principalTable: "PointsLists",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Square",
                columns: table => new
                {
                    PointsListId = table.Column<int>(type: "int", nullable: false),
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Square", x => new { x.PointsListId, x.Id });
                    table.ForeignKey(
                        name: "FK_Square_PointsLists_PointsListId",
                        column: x => x.PointsListId,
                        principalTable: "PointsLists",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Square_Points",
                columns: table => new
                {
                    SquarePointsListId = table.Column<int>(type: "int", nullable: false),
                    SquareId = table.Column<int>(type: "int", nullable: false),
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    X = table.Column<int>(type: "int", nullable: false),
                    Y = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Square_Points", x => new { x.SquarePointsListId, x.SquareId, x.Id });
                    table.ForeignKey(
                        name: "FK_Square_Points_Square_SquarePointsListId_SquareId",
                        columns: x => new { x.SquarePointsListId, x.SquareId },
                        principalTable: "Square",
                        principalColumns: new[] { "PointsListId", "Id" },
                        onDelete: ReferentialAction.Cascade);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PointsLists_Points");

            migrationBuilder.DropTable(
                name: "Square_Points");

            migrationBuilder.DropTable(
                name: "Square");

            migrationBuilder.DropTable(
                name: "PointsLists");
        }
    }
}
