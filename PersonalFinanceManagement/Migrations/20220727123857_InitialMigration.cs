using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PesonalFinanceManagement.Migrations
{
    public partial class InitialMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "categories",
                columns: table => new
                {
                    Code = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    ParentCode = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_categories", x => x.Code);
                });

            migrationBuilder.CreateTable(
                name: "transactions",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    BeneficiaryName = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    Date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Direction = table.Column<string>(type: "character varying(1)", maxLength: 1, nullable: false),
                    Amount = table.Column<double>(type: "double precision", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    Currency = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false),
                    Mcc = table.Column<string>(type: "text", nullable: true),
                    Kind = table.Column<string>(type: "text", nullable: false),
                    Catcode = table.Column<string>(type: "text", nullable: true),
                    CategoryCode = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_transactions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_transactions_categories_CategoryCode",
                        column: x => x.CategoryCode,
                        principalTable: "categories",
                        principalColumn: "Code");
                });

            migrationBuilder.CreateTable(
                name: "splitTransactions",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    Catcode = table.Column<string>(type: "text", nullable: false),
                    Amount = table.Column<double>(type: "double precision", nullable: false),
                    TransactionId = table.Column<string>(type: "text", nullable: true),
                    CategoryCode = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_splitTransactions", x => new { x.Id, x.Catcode });
                    table.ForeignKey(
                        name: "FK_splitTransactions_categories_CategoryCode",
                        column: x => x.CategoryCode,
                        principalTable: "categories",
                        principalColumn: "Code");
                    table.ForeignKey(
                        name: "FK_splitTransactions_transactions_TransactionId",
                        column: x => x.TransactionId,
                        principalTable: "transactions",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_splitTransactions_CategoryCode",
                table: "splitTransactions",
                column: "CategoryCode");

            migrationBuilder.CreateIndex(
                name: "IX_splitTransactions_TransactionId",
                table: "splitTransactions",
                column: "TransactionId");

            migrationBuilder.CreateIndex(
                name: "IX_transactions_CategoryCode",
                table: "transactions",
                column: "CategoryCode");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "splitTransactions");

            migrationBuilder.DropTable(
                name: "transactions");

            migrationBuilder.DropTable(
                name: "categories");
        }
    }
}
