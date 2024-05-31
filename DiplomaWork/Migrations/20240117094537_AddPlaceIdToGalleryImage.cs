using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DiplomaWork.Migrations
{
    /// <inheritdoc />
    public partial class AddPlaceIdToGalleryImage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GalleryImage_Places_PlaceId",
                table: "GalleryImage");

            migrationBuilder.DropPrimaryKey(
                name: "PK_GalleryImage",
                table: "GalleryImage");

            migrationBuilder.RenameTable(
                name: "GalleryImage",
                newName: "GalleryImages");

            migrationBuilder.RenameIndex(
                name: "IX_GalleryImage_PlaceId",
                table: "GalleryImages",
                newName: "IX_GalleryImages_PlaceId");

            migrationBuilder.AlterColumn<int>(
                name: "PlaceId",
                table: "GalleryImages",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_GalleryImages",
                table: "GalleryImages",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_GalleryImages_Places_PlaceId",
                table: "GalleryImages",
                column: "PlaceId",
                principalTable: "Places",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GalleryImages_Places_PlaceId",
                table: "GalleryImages");

            migrationBuilder.DropPrimaryKey(
                name: "PK_GalleryImages",
                table: "GalleryImages");

            migrationBuilder.RenameTable(
                name: "GalleryImages",
                newName: "GalleryImage");

            migrationBuilder.RenameIndex(
                name: "IX_GalleryImages_PlaceId",
                table: "GalleryImage",
                newName: "IX_GalleryImage_PlaceId");

            migrationBuilder.AlterColumn<int>(
                name: "PlaceId",
                table: "GalleryImage",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddPrimaryKey(
                name: "PK_GalleryImage",
                table: "GalleryImage",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_GalleryImage_Places_PlaceId",
                table: "GalleryImage",
                column: "PlaceId",
                principalTable: "Places",
                principalColumn: "Id");
        }
    }
}
