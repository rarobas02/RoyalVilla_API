using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RoyalVilla_API.Migrations
{
    /// <inheritdoc />
    public partial class AddVillaAmenitiesRelationToVilla : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_VillaAmenities_Villas_VillaId",
                table: "VillaAmenities");

            migrationBuilder.AlterColumn<int>(
                name: "VillaId",
                table: "VillaAmenities",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_VillaAmenities_Villas_VillaId",
                table: "VillaAmenities",
                column: "VillaId",
                principalTable: "Villas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_VillaAmenities_Villas_VillaId",
                table: "VillaAmenities");

            migrationBuilder.AlterColumn<int>(
                name: "VillaId",
                table: "VillaAmenities",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_VillaAmenities_Villas_VillaId",
                table: "VillaAmenities",
                column: "VillaId",
                principalTable: "Villas",
                principalColumn: "Id");
        }
    }
}
