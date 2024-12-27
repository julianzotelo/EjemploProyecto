using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Historial_C.Data.Migrations
{
    public partial class DniUnique : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Personas_Dni",
                table: "Personas",
                column: "Dni",
                unique: true,
                filter: "[Dni] IS NOT NULL");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Personas_Dni",
                table: "Personas");
        }
    }
}
