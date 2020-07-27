using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace Notebook.Database.Migrations
{
    public partial class InitTables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "notebook");

            migrationBuilder.CreateTable(
                name: "contacts",
                schema: "notebook",
                columns: table => new
                {
                    id = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    first_name = table.Column<string>(nullable: true),
                    last_name = table.Column<string>(nullable: true),
                    patronymic = table.Column<string>(nullable: true),
                    birth_date = table.Column<DateTime>(nullable: true),
                    organization_name = table.Column<string>(nullable: true),
                    position = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_contacts", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "record_types",
                schema: "notebook",
                columns: table => new
                {
                    id = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(nullable: true),
                    alias = table.Column<string>(nullable: false),
                    description = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_record_types", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "contact_informations",
                schema: "notebook",
                columns: table => new
                {
                    id = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    phone_number = table.Column<string>(nullable: true),
                    email = table.Column<string>(nullable: true),
                    skype = table.Column<string>(nullable: true),
                    other = table.Column<string>(nullable: true),
                    contact_id = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_contact_informations", x => x.id);
                    table.ForeignKey(
                        name: "fk_contact_informations_contacts_contact_id",
                        column: x => x.contact_id,
                        principalSchema: "notebook",
                        principalTable: "contacts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "records",
                schema: "notebook",
                columns: table => new
                {
                    id = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    start_date = table.Column<DateTime>(nullable: false),
                    end_date = table.Column<DateTime>(nullable: true),
                    theme = table.Column<string>(nullable: true),
                    place = table.Column<string>(nullable: true),
                    is_deleted = table.Column<bool>(nullable: false),
                    is_complete = table.Column<bool>(nullable: false),
                    created_at = table.Column<DateTime>(nullable: false),
                    record_type_id = table.Column<long>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_records", x => x.id);
                    table.ForeignKey(
                        name: "fk_records_record_types_record_type_id",
                        column: x => x.record_type_id,
                        principalSchema: "notebook",
                        principalTable: "record_types",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "records_to_contacts",
                schema: "notebook",
                columns: table => new
                {
                    record_id = table.Column<long>(nullable: false),
                    contact_id = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_records_to_contacts", x => new { x.contact_id, x.record_id });
                    table.ForeignKey(
                        name: "fk_records_to_contacts_contacts_contact_id",
                        column: x => x.contact_id,
                        principalSchema: "notebook",
                        principalTable: "contacts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_records_to_contacts_records_record_id",
                        column: x => x.record_id,
                        principalSchema: "notebook",
                        principalTable: "records",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                schema: "notebook",
                table: "record_types",
                columns: new[] { "id", "alias", "description", "name" },
                values: new object[,]
                {
                    { 3L, "Deal", "Planning deal", "Deal" },
                    { 1L, "Meeting", "Planning meeting", "Meeting" },
                    { 2L, "Notes", "Notes", "Notes" }
                });

            migrationBuilder.CreateIndex(
                name: "ix_contact_informations_contact_id",
                schema: "notebook",
                table: "contact_informations",
                column: "contact_id");

            migrationBuilder.CreateIndex(
                name: "ix_records_record_type_id",
                schema: "notebook",
                table: "records",
                column: "record_type_id");

            migrationBuilder.CreateIndex(
                name: "ix_records_to_contacts_record_id",
                schema: "notebook",
                table: "records_to_contacts",
                column: "record_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "contact_informations",
                schema: "notebook");

            migrationBuilder.DropTable(
                name: "records_to_contacts",
                schema: "notebook");

            migrationBuilder.DropTable(
                name: "contacts",
                schema: "notebook");

            migrationBuilder.DropTable(
                name: "records",
                schema: "notebook");

            migrationBuilder.DropTable(
                name: "record_types",
                schema: "notebook");
        }
    }
}
