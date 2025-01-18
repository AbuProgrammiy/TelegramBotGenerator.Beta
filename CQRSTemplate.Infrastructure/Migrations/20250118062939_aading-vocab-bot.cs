using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CQRSTemplate.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class aadingvocabbot : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Vocabularies_Bots_BotId",
                table: "Vocabularies");

            migrationBuilder.AlterColumn<Guid>(
                name: "BotId",
                table: "Vocabularies",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Vocabularies_Bots_BotId",
                table: "Vocabularies",
                column: "BotId",
                principalTable: "Bots",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Vocabularies_Bots_BotId",
                table: "Vocabularies");

            migrationBuilder.AlterColumn<Guid>(
                name: "BotId",
                table: "Vocabularies",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddForeignKey(
                name: "FK_Vocabularies_Bots_BotId",
                table: "Vocabularies",
                column: "BotId",
                principalTable: "Bots",
                principalColumn: "Id");
        }
    }
}
