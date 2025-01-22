using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NegotiationSystemAPI.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Items",
                columns: table => new
                {
                    ItemId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Items", x => x.ItemId);
                });

            migrationBuilder.CreateTable(
                name: "Parties",
                columns: table => new
                {
                    PartyId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Parties", x => x.PartyId);
                });

            migrationBuilder.CreateTable(
                name: "ItemParty",
                columns: table => new
                {
                    ItemsItemId = table.Column<int>(type: "int", nullable: false),
                    PartiesPartyId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItemParty", x => new { x.ItemsItemId, x.PartiesPartyId });
                    table.ForeignKey(
                        name: "FK_ItemParty_Items_ItemsItemId",
                        column: x => x.ItemsItemId,
                        principalTable: "Items",
                        principalColumn: "ItemId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ItemParty_Parties_PartiesPartyId",
                        column: x => x.PartiesPartyId,
                        principalTable: "Parties",
                        principalColumn: "PartyId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PartyId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.UserId);
                    table.ForeignKey(
                        name: "FK_Users_Parties_PartyId",
                        column: x => x.PartyId,
                        principalTable: "Parties",
                        principalColumn: "PartyId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Proposals",
                columns: table => new
                {
                    ProposalId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ItemId = table.Column<int>(type: "int", nullable: false),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<int>(type: "int", nullable: false),
                    Message = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsCounterProposal = table.Column<bool>(type: "bit", nullable: false),
                    InitialProposalId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Proposals", x => x.ProposalId);
                    table.ForeignKey(
                        name: "FK_Proposals_Items_ItemId",
                        column: x => x.ItemId,
                        principalTable: "Items",
                        principalColumn: "ItemId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Proposals_Proposals_InitialProposalId",
                        column: x => x.InitialProposalId,
                        principalTable: "Proposals",
                        principalColumn: "ProposalId");
                    table.ForeignKey(
                        name: "FK_Proposals_Users_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PaymentRatios",
                columns: table => new
                {
                    PaymentRatioId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProposalId = table.Column<int>(type: "int", nullable: false),
                    PartyId = table.Column<int>(type: "int", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    StatusedBy = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentRatios", x => x.PaymentRatioId);
                    table.ForeignKey(
                        name: "FK_PaymentRatios_Parties_PartyId",
                        column: x => x.PartyId,
                        principalTable: "Parties",
                        principalColumn: "PartyId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PaymentRatios_Proposals_ProposalId",
                        column: x => x.ProposalId,
                        principalTable: "Proposals",
                        principalColumn: "ProposalId");
                    table.ForeignKey(
                        name: "FK_PaymentRatios_Users_StatusedBy",
                        column: x => x.StatusedBy,
                        principalTable: "Users",
                        principalColumn: "UserId");
                });

            migrationBuilder.CreateIndex(
                name: "IX_ItemParty_PartiesPartyId",
                table: "ItemParty",
                column: "PartiesPartyId");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentRatios_PartyId",
                table: "PaymentRatios",
                column: "PartyId");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentRatios_ProposalId",
                table: "PaymentRatios",
                column: "ProposalId");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentRatios_StatusedBy",
                table: "PaymentRatios",
                column: "StatusedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Proposals_CreatedBy",
                table: "Proposals",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Proposals_InitialProposalId",
                table: "Proposals",
                column: "InitialProposalId");

            migrationBuilder.CreateIndex(
                name: "IX_Proposals_ItemId",
                table: "Proposals",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_PartyId",
                table: "Users",
                column: "PartyId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ItemParty");

            migrationBuilder.DropTable(
                name: "PaymentRatios");

            migrationBuilder.DropTable(
                name: "Proposals");

            migrationBuilder.DropTable(
                name: "Items");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Parties");
        }
    }
}
