using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MediaStreamer.DataAccess.NetStandard.Migrations
{
    public partial class AddedPlayerStates : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PlayerStates",
                columns: table => new
                {
                    StateID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StateTime = table.Column<DateTime>(type: "DATETIME", nullable: false),
                    VolumeLevel = table.Column<decimal>(type: "NUMERIC(38,17)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlayerStates", x => x.StateID);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PlayerStates");
        }
    }
}
