using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CQRSTemplate.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class aadingbotbotId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "BotId",
                table: "Bots",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BotId",
                table: "Bots");
        }
    }
}
