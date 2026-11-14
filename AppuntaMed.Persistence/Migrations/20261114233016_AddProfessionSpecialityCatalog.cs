using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace AppuntaMed.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddProfessionSpecialityCatalog : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Profession",
                table: "Doctors");

            migrationBuilder.DropColumn(
                name: "Speciality",
                table: "Doctors");

            migrationBuilder.AddColumn<Guid>(
                name: "ProfessionId",
                table: "Doctors",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "SpecialityId",
                table: "Doctors",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Professions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Professions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Specialities",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProfessionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Specialities", x => x.Id);
                    table.UniqueConstraint("AK_Specialities_ProfessionId_Id", x => new { x.ProfessionId, x.Id });
                    table.ForeignKey(
                        name: "FK_Specialities_Professions_ProfessionId",
                        column: x => x.ProfessionId,
                        principalTable: "Professions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "Professions",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { new Guid("38113eab-b310-54d2-98b0-fd0450bafdda"), "Nutritionist" },
                    { new Guid("4bc5ba9b-f30a-5d65-9e16-a8489db164f2"), "Physiotherapist" },
                    { new Guid("67c0884c-ef4f-5457-8017-9eaffff23484"), "Dentist" },
                    { new Guid("6b792a54-4790-54b6-9a3c-afda9ef91c02"), "Physician" },
                    { new Guid("835fbbce-e62b-5cf7-8d8f-fa26f607dcbc"), "Psychologist" }
                });

            migrationBuilder.InsertData(
                table: "Specialities",
                columns: new[] { "Id", "Name", "ProfessionId" },
                values: new object[,]
                {
                    { new Guid("09354c8d-3ceb-5b8c-9472-7c2c4459f4ed"), "Pediatrics", new Guid("6b792a54-4790-54b6-9a3c-afda9ef91c02") },
                    { new Guid("17375fd5-cfec-5718-b98a-8b1c373ee7f5"), "Periodontics", new Guid("67c0884c-ef4f-5457-8017-9eaffff23484") },
                    { new Guid("186f64ad-3715-5339-b385-90fc46e1876f"), "Neuropsychology", new Guid("835fbbce-e62b-5cf7-8d8f-fa26f607dcbc") },
                    { new Guid("222c255e-75ad-58e4-8c18-964dd79b709e"), "Sports Rehabilitation", new Guid("4bc5ba9b-f30a-5d65-9e16-a8489db164f2") },
                    { new Guid("277830e8-1588-5127-87a1-3a033f9039ec"), "Ophthalmology", new Guid("6b792a54-4790-54b6-9a3c-afda9ef91c02") },
                    { new Guid("41e09464-6560-5c1d-b10a-c211bec0000b"), "Dermatology", new Guid("6b792a54-4790-54b6-9a3c-afda9ef91c02") },
                    { new Guid("447f5876-4071-508a-a527-0cacbe8c8f7e"), "Pediatric Dentistry", new Guid("67c0884c-ef4f-5457-8017-9eaffff23484") },
                    { new Guid("4f739c23-17df-5e66-bcf6-ace6e9e08599"), "Sports Nutrition", new Guid("38113eab-b310-54d2-98b0-fd0450bafdda") },
                    { new Guid("52ab4b2d-5586-529b-9031-1a6b38380b40"), "Neurological Rehabilitation", new Guid("4bc5ba9b-f30a-5d65-9e16-a8489db164f2") },
                    { new Guid("52f28894-8139-5bef-a1c4-c743ee322b1d"), "Oral Surgery", new Guid("67c0884c-ef4f-5457-8017-9eaffff23484") },
                    { new Guid("6b812dc3-cc3e-5cf5-9b27-b6c02f9e291b"), "Clinical Psychology", new Guid("835fbbce-e62b-5cf7-8d8f-fa26f607dcbc") },
                    { new Guid("72cb5346-81f7-524f-a8ca-b6300ed0fa75"), "Endodontics", new Guid("67c0884c-ef4f-5457-8017-9eaffff23484") },
                    { new Guid("94aca0ac-0beb-567d-8643-7e70dfedf1e1"), "Orthodontics", new Guid("67c0884c-ef4f-5457-8017-9eaffff23484") },
                    { new Guid("a1612ff6-43da-5f0b-bebd-a050bb743a67"), "Gynecology", new Guid("6b792a54-4790-54b6-9a3c-afda9ef91c02") },
                    { new Guid("c3628f69-0e58-5876-a247-a28d3ebddd2d"), "Clinical Nutrition", new Guid("38113eab-b310-54d2-98b0-fd0450bafdda") },
                    { new Guid("d1742153-3c77-50f8-926a-91549b814fde"), "Orthopedics", new Guid("6b792a54-4790-54b6-9a3c-afda9ef91c02") },
                    { new Guid("d57c3270-0034-5a73-b4bf-181842b8282b"), "Neurology", new Guid("6b792a54-4790-54b6-9a3c-afda9ef91c02") },
                    { new Guid("f5e73d96-1596-5201-8839-c19b357982c7"), "Cardiology", new Guid("6b792a54-4790-54b6-9a3c-afda9ef91c02") },
                    { new Guid("f67434c6-c1a8-53e0-b69a-882ea762f2db"), "Child Psychology", new Guid("835fbbce-e62b-5cf7-8d8f-fa26f607dcbc") }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Doctors_ProfessionId_SpecialityId",
                table: "Doctors",
                columns: new[] { "ProfessionId", "SpecialityId" });

            migrationBuilder.CreateIndex(
                name: "IX_Professions_Name",
                table: "Professions",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Specialities_ProfessionId_Name",
                table: "Specialities",
                columns: new[] { "ProfessionId", "Name" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Doctors_Professions_ProfessionId",
                table: "Doctors",
                column: "ProfessionId",
                principalTable: "Professions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Doctors_Specialities_ProfessionId_SpecialityId",
                table: "Doctors",
                columns: new[] { "ProfessionId", "SpecialityId" },
                principalTable: "Specialities",
                principalColumns: new[] { "ProfessionId", "Id" },
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Doctors_Professions_ProfessionId",
                table: "Doctors");

            migrationBuilder.DropForeignKey(
                name: "FK_Doctors_Specialities_ProfessionId_SpecialityId",
                table: "Doctors");

            migrationBuilder.DropTable(
                name: "Specialities");

            migrationBuilder.DropTable(
                name: "Professions");

            migrationBuilder.DropIndex(
                name: "IX_Doctors_ProfessionId_SpecialityId",
                table: "Doctors");

            migrationBuilder.DropColumn(
                name: "ProfessionId",
                table: "Doctors");

            migrationBuilder.DropColumn(
                name: "SpecialityId",
                table: "Doctors");

            migrationBuilder.AddColumn<string>(
                name: "Profession",
                table: "Doctors",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Speciality",
                table: "Doctors",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");
        }
    }
}
