using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace PriceTrackerCloud.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Products",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    Category = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Stores",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Website = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Stores", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    PasswordHash = table.Column<string>(type: "text", nullable: false),
                    Role = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProductPrices",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ProductId = table.Column<Guid>(type: "uuid", nullable: false),
                    StoreId = table.Column<Guid>(type: "uuid", nullable: false),
                    Price = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    DateCollected = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductPrices", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductPrices_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProductPrices_Stores_StoreId",
                        column: x => x.StoreId,
                        principalTable: "Stores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Alerts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    ProductId = table.Column<Guid>(type: "uuid", nullable: false),
                    TargetPrice = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Alerts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Alerts_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Alerts_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "Id", "Category", "Description", "Name" },
                values: new object[,]
                {
                    { new Guid("b2b2b2b2-0000-0000-0000-000000000001"), "Videojuegos", "Consola Sony PS5 Standard Edition", "PlayStation 5" },
                    { new Guid("b2b2b2b2-0000-0000-0000-000000000002"), "Smartphones", "Apple iPhone 15 Pro 256GB", "iPhone 15 Pro" },
                    { new Guid("b2b2b2b2-0000-0000-0000-000000000003"), "Portátiles", "Apple MacBook Pro 14\" chip M3", "MacBook Pro 14 M3" }
                });

            migrationBuilder.InsertData(
                table: "Stores",
                columns: new[] { "Id", "Name", "Website" },
                values: new object[,]
                {
                    { new Guid("a1a1a1a1-0000-0000-0000-000000000001"), "Amazon", "https://www.amazon.es" },
                    { new Guid("a1a1a1a1-0000-0000-0000-000000000002"), "PcComponentes", "https://www.pccomponentes.com" },
                    { new Guid("a1a1a1a1-0000-0000-0000-000000000003"), "MediaMarkt", "https://www.mediamarkt.es" },
                    { new Guid("a1a1a1a1-0000-0000-0000-000000000004"), "El Corte Inglés", "https://www.elcorteingles.es" }
                });

            migrationBuilder.InsertData(
                table: "ProductPrices",
                columns: new[] { "Id", "DateCollected", "Price", "ProductId", "StoreId" },
                values: new object[,]
                {
                    { new Guid("c3c3c3c3-0000-0000-0000-000000000001"), new DateTime(2026, 5, 1, 0, 0, 0, 0, DateTimeKind.Utc), 549.99m, new Guid("b2b2b2b2-0000-0000-0000-000000000001"), new Guid("a1a1a1a1-0000-0000-0000-000000000001") },
                    { new Guid("c3c3c3c3-0000-0000-0000-000000000002"), new DateTime(2026, 5, 1, 0, 0, 0, 0, DateTimeKind.Utc), 559.99m, new Guid("b2b2b2b2-0000-0000-0000-000000000001"), new Guid("a1a1a1a1-0000-0000-0000-000000000003") },
                    { new Guid("c3c3c3c3-0000-0000-0000-000000000003"), new DateTime(2026, 5, 6, 0, 0, 0, 0, DateTimeKind.Utc), 539.99m, new Guid("b2b2b2b2-0000-0000-0000-000000000001"), new Guid("a1a1a1a1-0000-0000-0000-000000000001") },
                    { new Guid("c3c3c3c3-0000-0000-0000-000000000004"), new DateTime(2026, 5, 6, 0, 0, 0, 0, DateTimeKind.Utc), 549.99m, new Guid("b2b2b2b2-0000-0000-0000-000000000001"), new Guid("a1a1a1a1-0000-0000-0000-000000000003") },
                    { new Guid("c3c3c3c3-0000-0000-0000-000000000005"), new DateTime(2026, 5, 11, 0, 0, 0, 0, DateTimeKind.Utc), 529.99m, new Guid("b2b2b2b2-0000-0000-0000-000000000001"), new Guid("a1a1a1a1-0000-0000-0000-000000000001") },
                    { new Guid("c3c3c3c3-0000-0000-0000-000000000006"), new DateTime(2026, 5, 11, 0, 0, 0, 0, DateTimeKind.Utc), 549.99m, new Guid("b2b2b2b2-0000-0000-0000-000000000001"), new Guid("a1a1a1a1-0000-0000-0000-000000000003") },
                    { new Guid("c3c3c3c3-0000-0000-0000-000000000007"), new DateTime(2026, 5, 16, 0, 0, 0, 0, DateTimeKind.Utc), 519.99m, new Guid("b2b2b2b2-0000-0000-0000-000000000001"), new Guid("a1a1a1a1-0000-0000-0000-000000000001") },
                    { new Guid("c3c3c3c3-0000-0000-0000-000000000008"), new DateTime(2026, 5, 16, 0, 0, 0, 0, DateTimeKind.Utc), 539.99m, new Guid("b2b2b2b2-0000-0000-0000-000000000001"), new Guid("a1a1a1a1-0000-0000-0000-000000000003") },
                    { new Guid("c3c3c3c3-0000-0000-0000-000000000009"), new DateTime(2026, 5, 21, 0, 0, 0, 0, DateTimeKind.Utc), 509.99m, new Guid("b2b2b2b2-0000-0000-0000-000000000001"), new Guid("a1a1a1a1-0000-0000-0000-000000000001") },
                    { new Guid("c3c3c3c3-0000-0000-0000-000000000010"), new DateTime(2026, 5, 21, 0, 0, 0, 0, DateTimeKind.Utc), 529.99m, new Guid("b2b2b2b2-0000-0000-0000-000000000001"), new Guid("a1a1a1a1-0000-0000-0000-000000000003") },
                    { new Guid("c3c3c3c3-0000-0000-0000-000000000011"), new DateTime(2026, 5, 26, 0, 0, 0, 0, DateTimeKind.Utc), 499.99m, new Guid("b2b2b2b2-0000-0000-0000-000000000001"), new Guid("a1a1a1a1-0000-0000-0000-000000000001") },
                    { new Guid("c3c3c3c3-0000-0000-0000-000000000012"), new DateTime(2026, 5, 26, 0, 0, 0, 0, DateTimeKind.Utc), 519.99m, new Guid("b2b2b2b2-0000-0000-0000-000000000001"), new Guid("a1a1a1a1-0000-0000-0000-000000000003") },
                    { new Guid("c3c3c3c3-0000-0000-0000-000000000013"), new DateTime(2026, 5, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1229.00m, new Guid("b2b2b2b2-0000-0000-0000-000000000002"), new Guid("a1a1a1a1-0000-0000-0000-000000000002") },
                    { new Guid("c3c3c3c3-0000-0000-0000-000000000014"), new DateTime(2026, 5, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1249.00m, new Guid("b2b2b2b2-0000-0000-0000-000000000002"), new Guid("a1a1a1a1-0000-0000-0000-000000000004") },
                    { new Guid("c3c3c3c3-0000-0000-0000-000000000015"), new DateTime(2026, 5, 6, 0, 0, 0, 0, DateTimeKind.Utc), 1219.00m, new Guid("b2b2b2b2-0000-0000-0000-000000000002"), new Guid("a1a1a1a1-0000-0000-0000-000000000002") },
                    { new Guid("c3c3c3c3-0000-0000-0000-000000000016"), new DateTime(2026, 5, 6, 0, 0, 0, 0, DateTimeKind.Utc), 1249.00m, new Guid("b2b2b2b2-0000-0000-0000-000000000002"), new Guid("a1a1a1a1-0000-0000-0000-000000000004") },
                    { new Guid("c3c3c3c3-0000-0000-0000-000000000017"), new DateTime(2026, 5, 11, 0, 0, 0, 0, DateTimeKind.Utc), 1199.00m, new Guid("b2b2b2b2-0000-0000-0000-000000000002"), new Guid("a1a1a1a1-0000-0000-0000-000000000002") },
                    { new Guid("c3c3c3c3-0000-0000-0000-000000000018"), new DateTime(2026, 5, 11, 0, 0, 0, 0, DateTimeKind.Utc), 1229.00m, new Guid("b2b2b2b2-0000-0000-0000-000000000002"), new Guid("a1a1a1a1-0000-0000-0000-000000000004") },
                    { new Guid("c3c3c3c3-0000-0000-0000-000000000019"), new DateTime(2026, 5, 16, 0, 0, 0, 0, DateTimeKind.Utc), 1189.00m, new Guid("b2b2b2b2-0000-0000-0000-000000000002"), new Guid("a1a1a1a1-0000-0000-0000-000000000002") },
                    { new Guid("c3c3c3c3-0000-0000-0000-000000000020"), new DateTime(2026, 5, 16, 0, 0, 0, 0, DateTimeKind.Utc), 1209.00m, new Guid("b2b2b2b2-0000-0000-0000-000000000002"), new Guid("a1a1a1a1-0000-0000-0000-000000000004") },
                    { new Guid("c3c3c3c3-0000-0000-0000-000000000021"), new DateTime(2026, 5, 21, 0, 0, 0, 0, DateTimeKind.Utc), 1179.00m, new Guid("b2b2b2b2-0000-0000-0000-000000000002"), new Guid("a1a1a1a1-0000-0000-0000-000000000002") },
                    { new Guid("c3c3c3c3-0000-0000-0000-000000000022"), new DateTime(2026, 5, 21, 0, 0, 0, 0, DateTimeKind.Utc), 1199.00m, new Guid("b2b2b2b2-0000-0000-0000-000000000002"), new Guid("a1a1a1a1-0000-0000-0000-000000000004") },
                    { new Guid("c3c3c3c3-0000-0000-0000-000000000023"), new DateTime(2026, 5, 26, 0, 0, 0, 0, DateTimeKind.Utc), 1169.00m, new Guid("b2b2b2b2-0000-0000-0000-000000000002"), new Guid("a1a1a1a1-0000-0000-0000-000000000002") },
                    { new Guid("c3c3c3c3-0000-0000-0000-000000000024"), new DateTime(2026, 5, 26, 0, 0, 0, 0, DateTimeKind.Utc), 1189.00m, new Guid("b2b2b2b2-0000-0000-0000-000000000002"), new Guid("a1a1a1a1-0000-0000-0000-000000000004") },
                    { new Guid("c3c3c3c3-0000-0000-0000-000000000025"), new DateTime(2026, 5, 1, 0, 0, 0, 0, DateTimeKind.Utc), 2199.00m, new Guid("b2b2b2b2-0000-0000-0000-000000000003"), new Guid("a1a1a1a1-0000-0000-0000-000000000001") },
                    { new Guid("c3c3c3c3-0000-0000-0000-000000000026"), new DateTime(2026, 5, 1, 0, 0, 0, 0, DateTimeKind.Utc), 2249.00m, new Guid("b2b2b2b2-0000-0000-0000-000000000003"), new Guid("a1a1a1a1-0000-0000-0000-000000000002") },
                    { new Guid("c3c3c3c3-0000-0000-0000-000000000027"), new DateTime(2026, 5, 6, 0, 0, 0, 0, DateTimeKind.Utc), 2149.00m, new Guid("b2b2b2b2-0000-0000-0000-000000000003"), new Guid("a1a1a1a1-0000-0000-0000-000000000001") },
                    { new Guid("c3c3c3c3-0000-0000-0000-000000000028"), new DateTime(2026, 5, 6, 0, 0, 0, 0, DateTimeKind.Utc), 2199.00m, new Guid("b2b2b2b2-0000-0000-0000-000000000003"), new Guid("a1a1a1a1-0000-0000-0000-000000000002") },
                    { new Guid("c3c3c3c3-0000-0000-0000-000000000029"), new DateTime(2026, 5, 11, 0, 0, 0, 0, DateTimeKind.Utc), 2099.00m, new Guid("b2b2b2b2-0000-0000-0000-000000000003"), new Guid("a1a1a1a1-0000-0000-0000-000000000001") },
                    { new Guid("c3c3c3c3-0000-0000-0000-000000000030"), new DateTime(2026, 5, 11, 0, 0, 0, 0, DateTimeKind.Utc), 2149.00m, new Guid("b2b2b2b2-0000-0000-0000-000000000003"), new Guid("a1a1a1a1-0000-0000-0000-000000000002") },
                    { new Guid("c3c3c3c3-0000-0000-0000-000000000031"), new DateTime(2026, 5, 16, 0, 0, 0, 0, DateTimeKind.Utc), 2049.00m, new Guid("b2b2b2b2-0000-0000-0000-000000000003"), new Guid("a1a1a1a1-0000-0000-0000-000000000001") },
                    { new Guid("c3c3c3c3-0000-0000-0000-000000000032"), new DateTime(2026, 5, 16, 0, 0, 0, 0, DateTimeKind.Utc), 2099.00m, new Guid("b2b2b2b2-0000-0000-0000-000000000003"), new Guid("a1a1a1a1-0000-0000-0000-000000000002") },
                    { new Guid("c3c3c3c3-0000-0000-0000-000000000033"), new DateTime(2026, 5, 21, 0, 0, 0, 0, DateTimeKind.Utc), 1999.00m, new Guid("b2b2b2b2-0000-0000-0000-000000000003"), new Guid("a1a1a1a1-0000-0000-0000-000000000001") },
                    { new Guid("c3c3c3c3-0000-0000-0000-000000000034"), new DateTime(2026, 5, 21, 0, 0, 0, 0, DateTimeKind.Utc), 2049.00m, new Guid("b2b2b2b2-0000-0000-0000-000000000003"), new Guid("a1a1a1a1-0000-0000-0000-000000000002") },
                    { new Guid("c3c3c3c3-0000-0000-0000-000000000035"), new DateTime(2026, 5, 26, 0, 0, 0, 0, DateTimeKind.Utc), 1979.00m, new Guid("b2b2b2b2-0000-0000-0000-000000000003"), new Guid("a1a1a1a1-0000-0000-0000-000000000001") },
                    { new Guid("c3c3c3c3-0000-0000-0000-000000000036"), new DateTime(2026, 5, 26, 0, 0, 0, 0, DateTimeKind.Utc), 1999.00m, new Guid("b2b2b2b2-0000-0000-0000-000000000003"), new Guid("a1a1a1a1-0000-0000-0000-000000000002") }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Alerts_ProductId",
                table: "Alerts",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_Alerts_UserId",
                table: "Alerts",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductPrices_ProductId_StoreId_DateCollected",
                table: "ProductPrices",
                columns: new[] { "ProductId", "StoreId", "DateCollected" });

            migrationBuilder.CreateIndex(
                name: "IX_ProductPrices_StoreId",
                table: "ProductPrices",
                column: "StoreId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Alerts");

            migrationBuilder.DropTable(
                name: "ProductPrices");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Products");

            migrationBuilder.DropTable(
                name: "Stores");
        }
    }
}
