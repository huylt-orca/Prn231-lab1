using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ODataBookStore.Migrations
{
    public partial class InitialDB : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Presss",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Category = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Presss", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Username = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Role = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Books",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ISBN = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Author = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Location_City = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Location_Street = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PressId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Books", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Books_Presss_PressId",
                        column: x => x.PressId,
                        principalTable: "Presss",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Presss",
                columns: new[] { "Id", "Category", "Name" },
                values: new object[,]
                {
                    { 1, 0, "Beverages" },
                    { 2, 0, "Condiments" },
                    { 3, 0, "Confections" },
                    { 4, 0, "Dairy Products" },
                    { 5, 0, "Grains/Cereals" },
                    { 6, 0, "Meat/Cereals" },
                    { 7, 0, "Produce" },
                    { 8, 0, "Seafood" }
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "Name", "Password", "Role", "Username" },
                values: new object[,]
                {
                    { 1, "Admin", "x3Xnt1ft5jDNCqERO9ECZhqziCnKUqZCKreChi8mhkY=", 0, "Admin" },
                    { 2, "User", "x3Xnt1ft5jDNCqERO9ECZhqziCnKUqZCKreChi8mhkY=", 1, "User" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Books_PressId",
                table: "Books",
                column: "PressId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Books");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Presss");
        }
    }
}
