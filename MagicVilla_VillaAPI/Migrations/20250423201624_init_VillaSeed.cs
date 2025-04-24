using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace MagicVilla_VillaAPI.Migrations
{
    /// <inheritdoc />
    public partial class init_VillaSeed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Villas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Details = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Rate = table.Column<double>(type: "float", nullable: false),
                    SqFt = table.Column<int>(type: "int", nullable: false),
                    Occupancy = table.Column<int>(type: "int", nullable: false),
                    ImgUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Amenity = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Villas", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Villas",
                columns: new[] { "Id", "Amenity", "CreatedDate", "Details", "ImgUrl", "Name", "Occupancy", "Rate", "SqFt", "UpdatedDate" },
                values: new object[,]
                {
                    { 1, "Pool, Sauna, Wi-Fi", new DateTime(2023, 1, 15, 10, 30, 0, 0, DateTimeKind.Unspecified), "A luxury villa with a stunning ocean view and private beach access.", "https://example.com/images/villa1.jpg", "Ocean Breeze Villa", 6, 350.0, 2200, new DateTime(2024, 4, 20, 14, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 2, "Fireplace, Hot Tub, Hiking Trails", new DateTime(2022, 11, 12, 8, 45, 0, 0, DateTimeKind.Unspecified), "Cozy villa nestled in the mountains, perfect for a winter getaway.", "https://example.com/images/villa2.jpg", "Mountain Escape", 4, 270.0, 1800, new DateTime(2024, 3, 30, 9, 15, 0, 0, DateTimeKind.Unspecified) },
                    { 3, "Wi-Fi, Rooftop Access, City View", new DateTime(2023, 5, 1, 12, 0, 0, 0, DateTimeKind.Unspecified), "A stylish villa in the heart of the city, close to all attractions.", "https://example.com/images/villa3.jpg", "Urban Luxe", 3, 300.0, 1500, new DateTime(2024, 1, 25, 13, 30, 0, 0, DateTimeKind.Unspecified) },
                    { 4, "Boat Dock, BBQ Grill, Deck", new DateTime(2023, 8, 10, 9, 0, 0, 0, DateTimeKind.Unspecified), "Serene villa by the lake, ideal for peaceful weekends.", "https://example.com/images/villa4.jpg", "Lakeside Retreat", 4, 200.0, 1600, new DateTime(2024, 2, 18, 10, 45, 0, 0, DateTimeKind.Unspecified) },
                    { 5, "Pool, Outdoor Shower, Wi-Fi", new DateTime(2022, 9, 22, 11, 20, 0, 0, DateTimeKind.Unspecified), "Modern villa with stunning desert views and luxurious interiors.", "https://example.com/images/villa5.jpg", "Desert Dream", 5, 280.0, 1900, new DateTime(2024, 4, 1, 12, 15, 0, 0, DateTimeKind.Unspecified) },
                    { 6, "Garden, Fireplace, Bicycles", new DateTime(2023, 3, 3, 15, 40, 0, 0, DateTimeKind.Unspecified), "Rustic villa surrounded by nature and rolling hills.", "https://example.com/images/villa6.jpg", "Countryside Charm", 4, 180.0, 1700, new DateTime(2024, 4, 12, 16, 25, 0, 0, DateTimeKind.Unspecified) },
                    { 7, "Hammocks, Wi-Fi, Guided Tours", new DateTime(2022, 6, 17, 13, 0, 0, 0, DateTimeKind.Unspecified), "Hidden villa deep in the jungle, surrounded by wildlife.", "https://example.com/images/villa7.jpg", "Jungle Hideaway", 5, 320.0, 2000, new DateTime(2024, 1, 10, 11, 30, 0, 0, DateTimeKind.Unspecified) },
                    { 8, "Private Beach, Pool, Butler Service", new DateTime(2023, 4, 5, 7, 10, 0, 0, DateTimeKind.Unspecified), "Secluded villa on a private island, ultimate luxury experience.", "https://example.com/images/villa8.jpg", "Island Paradise", 6, 500.0, 2500, new DateTime(2024, 4, 15, 8, 55, 0, 0, DateTimeKind.Unspecified) },
                    { 9, "Heated Floors, Sauna, Ski Storage", new DateTime(2022, 12, 1, 10, 10, 0, 0, DateTimeKind.Unspecified), "Luxury villa near ski slopes, ideal for winter sports lovers.", "https://example.com/images/villa9.jpg", "Ski Chalet", 5, 400.0, 2100, new DateTime(2024, 2, 5, 9, 45, 0, 0, DateTimeKind.Unspecified) },
                    { 10, "Outdoor Bar, Pool, Wi-Fi", new DateTime(2023, 7, 20, 14, 0, 0, 0, DateTimeKind.Unspecified), "Vibrant and colorful villa in a tropical setting with lush gardens.", "https://example.com/images/villa10.jpg", "Tropical Bliss", 5, 330.0, 1950, new DateTime(2024, 4, 18, 17, 35, 0, 0, DateTimeKind.Unspecified) }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Villas");
        }
    }
}
