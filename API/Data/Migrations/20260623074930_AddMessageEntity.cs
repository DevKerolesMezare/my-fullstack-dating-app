using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddMessageEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "RecipientDeleted",
                table: "Messages",
                newName: "SenderDeleted");

            migrationBuilder.RenameColumn(
                name: "DateReadd",
                table: "Messages",
                newName: "DateRead");

            migrationBuilder.AddColumn<bool>(
                name: "RecipentDeleted",
                table: "Messages",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RecipentDeleted",
                table: "Messages");

            migrationBuilder.RenameColumn(
                name: "SenderDeleted",
                table: "Messages",
                newName: "RecipientDeleted");

            migrationBuilder.RenameColumn(
                name: "DateRead",
                table: "Messages",
                newName: "DateReadd");
        }
    }
}
