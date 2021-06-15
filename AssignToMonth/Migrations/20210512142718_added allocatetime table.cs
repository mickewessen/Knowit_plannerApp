using Microsoft.EntityFrameworkCore.Migrations;

namespace AssignToMonth.Migrations
{
    public partial class addedallocatetimetable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AllocateTime",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AllocatedHours = table.Column<int>(type: "int", nullable: false),
                    CustomerId = table.Column<int>(type: "int", nullable: true),
                    UserId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AllocateTime", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AllocateTime_AssignCustomerToMonths_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "AssignCustomerToMonths",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AllocateTime_AssignedMonths_UserId",
                        column: x => x.UserId,
                        principalTable: "AssignedMonths",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AllocateTime_CustomerId",
                table: "AllocateTime",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_AllocateTime_UserId",
                table: "AllocateTime",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AllocateTime");
        }
    }
}
