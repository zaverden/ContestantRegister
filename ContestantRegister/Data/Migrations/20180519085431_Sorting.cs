using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace ContestantRegister.Data.Migrations
{
    public partial class Sorting : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ContestCompClassParticipants");

            migrationBuilder.DropTable(
                name: "ContestCompClasses");

            migrationBuilder.DropColumn(
                name: "ComputerName",
                table: "ContestRegistrations");

            migrationBuilder.AddColumn<string>(
                name: "CompClassesData",
                table: "ContestAreas",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CompClassesData",
                table: "ContestAreas");

            migrationBuilder.AddColumn<string>(
                name: "ComputerName",
                table: "ContestRegistrations",
                maxLength: 50,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ContestCompClasses",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    CompClassId = table.Column<int>(nullable: false),
                    ContestId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContestCompClasses", x => x.Id);
                    table.UniqueConstraint("AK_ContestCompClasses_ContestId_CompClassId", x => new { x.ContestId, x.CompClassId });
                    table.ForeignKey(
                        name: "FK_ContestCompClasses_CompClasses_CompClassId",
                        column: x => x.CompClassId,
                        principalTable: "CompClasses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ContestCompClasses_Contests_ContestId",
                        column: x => x.ContestId,
                        principalTable: "Contests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ContestCompClassParticipants",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    CompNumber = table.Column<int>(nullable: false),
                    ContestCompClassId = table.Column<int>(nullable: false),
                    ParticipantId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContestCompClassParticipants", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ContestCompClassParticipants_ContestCompClasses_ContestCompClassId",
                        column: x => x.ContestCompClassId,
                        principalTable: "ContestCompClasses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ContestCompClassParticipants_AspNetUsers_ParticipantId",
                        column: x => x.ParticipantId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ContestCompClasses_CompClassId",
                table: "ContestCompClasses",
                column: "CompClassId");

            migrationBuilder.CreateIndex(
                name: "IX_ContestCompClassParticipants_ContestCompClassId",
                table: "ContestCompClassParticipants",
                column: "ContestCompClassId");

            migrationBuilder.CreateIndex(
                name: "IX_ContestCompClassParticipants_ParticipantId",
                table: "ContestCompClassParticipants",
                column: "ParticipantId");
        }
    }
}
