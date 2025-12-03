using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EfcDmPersistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Guests",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Email = table.Column<string>(type: "TEXT", maxLength: 80, nullable: false),
                    FirstName = table.Column<string>(type: "TEXT", maxLength: 25, nullable: false),
                    LastName = table.Column<string>(type: "TEXT", maxLength: 25, nullable: false),
                    ProfilePictureUrlAddress = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Guests", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "VeaEvents",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Title_Value = table.Column<string>(type: "TEXT", maxLength: 75, nullable: false),
                    Description_Value = table.Column<string>(type: "TEXT", maxLength: 250, nullable: false),
                    TimeRange_StartTime = table.Column<DateTime>(type: "TEXT", nullable: true),
                    TimeRange_EndTime = table.Column<DateTime>(type: "TEXT", nullable: true),
                    Status = table.Column<int>(type: "INTEGER", nullable: false),
                    Visibility = table.Column<int>(type: "INTEGER", nullable: false),
                    LocationMaxCapacity = table.Column<int>(type: "INTEGER", nullable: false),
                    MaxGuestsNo_Value = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VeaEvents", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EventParticipants",
                columns: table => new
                {
                    GuestId = table.Column<Guid>(type: "TEXT", nullable: false),
                    EventId = table.Column<Guid>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventParticipants", x => new { x.EventId, x.GuestId });
                    table.ForeignKey(
                        name: "FK_EventParticipants_VeaEvents_EventId",
                        column: x => x.EventId,
                        principalTable: "VeaEvents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Invitations",
                columns: table => new
                {
                    InvitationId = table.Column<Guid>(type: "TEXT", nullable: false),
                    GuestId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Status = table.Column<int>(type: "INTEGER", nullable: false),
                    EventId = table.Column<Guid>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Invitations", x => x.InvitationId);
                    table.ForeignKey(
                        name: "FK_Invitations_VeaEvents_EventId",
                        column: x => x.EventId,
                        principalTable: "VeaEvents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Guests_Email",
                table: "Guests",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Invitations_EventId_GuestId",
                table: "Invitations",
                columns: new[] { "EventId", "GuestId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EventParticipants");

            migrationBuilder.DropTable(
                name: "Guests");

            migrationBuilder.DropTable(
                name: "Invitations");

            migrationBuilder.DropTable(
                name: "VeaEvents");
        }
    }
}
