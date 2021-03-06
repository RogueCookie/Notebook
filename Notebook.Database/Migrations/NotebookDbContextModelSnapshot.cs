﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Notebook.Database;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace Notebook.Database.Migrations
{
    [DbContext(typeof(NotebookDbContext))]
    partial class NotebookDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasDefaultSchema("notebook")
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn)
                .HasAnnotation("ProductVersion", "3.1.5")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            modelBuilder.Entity("Notebook.Domain.Entity.Contact", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("id")
                        .HasColumnType("bigint")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<DateTime?>("BirthDate")
                        .HasColumnName("birth_date")
                        .HasColumnType("timestamp without time zone");

                    b.Property<string>("FirstName")
                        .HasColumnName("first_name")
                        .HasColumnType("text");

                    b.Property<string>("LastName")
                        .HasColumnName("last_name")
                        .HasColumnType("text");

                    b.Property<string>("OrganizationName")
                        .HasColumnName("organization_name")
                        .HasColumnType("text");

                    b.Property<string>("Patronymic")
                        .HasColumnName("patronymic")
                        .HasColumnType("text");

                    b.Property<string>("Position")
                        .HasColumnName("position")
                        .HasColumnType("text");

                    b.HasKey("Id")
                        .HasName("pk_contacts");

                    b.ToTable("contacts");
                });

            modelBuilder.Entity("Notebook.Domain.Entity.ContactInformation", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("id")
                        .HasColumnType("bigint")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<long>("ContactId")
                        .HasColumnName("contact_id")
                        .HasColumnType("bigint");

                    b.Property<string>("Email")
                        .HasColumnName("email")
                        .HasColumnType("text");

                    b.Property<string>("Other")
                        .HasColumnName("other")
                        .HasColumnType("text");

                    b.Property<string>("PhoneNumber")
                        .HasColumnName("phone_number")
                        .HasColumnType("text");

                    b.Property<string>("Skype")
                        .HasColumnName("skype")
                        .HasColumnType("text");

                    b.HasKey("Id")
                        .HasName("pk_contact_informations");

                    b.HasIndex("ContactId")
                        .HasName("ix_contact_informations_contact_id");

                    b.ToTable("contact_informations");
                });

            modelBuilder.Entity("Notebook.Domain.Entity.Record", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("id")
                        .HasColumnType("bigint")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnName("created_at")
                        .HasColumnType("timestamp without time zone");

                    b.Property<DateTime?>("EndDate")
                        .HasColumnName("end_date")
                        .HasColumnType("timestamp without time zone");

                    b.Property<bool>("IsComplete")
                        .HasColumnName("is_complete")
                        .HasColumnType("boolean");

                    b.Property<bool>("IsDeleted")
                        .HasColumnName("is_deleted")
                        .HasColumnType("boolean");

                    b.Property<string>("Place")
                        .HasColumnName("place")
                        .HasColumnType("text");

                    b.Property<long>("RecordTypeId")
                        .HasColumnName("record_type_id")
                        .HasColumnType("bigint");

                    b.Property<DateTime>("StartDate")
                        .HasColumnName("start_date")
                        .HasColumnType("timestamp without time zone");

                    b.Property<string>("Theme")
                        .HasColumnName("theme")
                        .HasColumnType("text");

                    b.HasKey("Id")
                        .HasName("pk_records");

                    b.HasIndex("RecordTypeId")
                        .HasName("ix_records_record_type_id");

                    b.ToTable("records");
                });

            modelBuilder.Entity("Notebook.Domain.Entity.RecordType", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("id")
                        .HasColumnType("bigint")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<string>("Alias")
                        .IsRequired()
                        .HasColumnName("alias")
                        .HasColumnType("text");

                    b.Property<string>("Description")
                        .HasColumnName("description")
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .HasColumnName("name")
                        .HasColumnType("text");

                    b.HasKey("Id")
                        .HasName("pk_record_types");

                    b.ToTable("record_types");

                    b.HasData(
                        new
                        {
                            Id = 3L,
                            Alias = "Deal",
                            Description = "Planning deal",
                            Name = "Deal"
                        },
                        new
                        {
                            Id = 1L,
                            Alias = "Meeting",
                            Description = "Planning meeting",
                            Name = "Meeting"
                        },
                        new
                        {
                            Id = 2L,
                            Alias = "Notes",
                            Description = "Notes",
                            Name = "Notes"
                        });
                });

            modelBuilder.Entity("Notebook.Domain.Entity.RecordsToContacts", b =>
                {
                    b.Property<long>("ContactId")
                        .HasColumnName("contact_id")
                        .HasColumnType("bigint");

                    b.Property<long>("RecordId")
                        .HasColumnName("record_id")
                        .HasColumnType("bigint");

                    b.HasKey("ContactId", "RecordId")
                        .HasName("pk_records_to_contacts");

                    b.HasIndex("RecordId")
                        .HasName("ix_records_to_contacts_record_id");

                    b.ToTable("records_to_contacts");
                });

            modelBuilder.Entity("Notebook.Domain.Entity.ContactInformation", b =>
                {
                    b.HasOne("Notebook.Domain.Entity.Contact", "Contact")
                        .WithMany("CollectionInformations")
                        .HasForeignKey("ContactId")
                        .HasConstraintName("fk_contact_informations_contacts_contact_id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Notebook.Domain.Entity.Record", b =>
                {
                    b.HasOne("Notebook.Domain.Entity.RecordType", "RecordType")
                        .WithMany("Records")
                        .HasForeignKey("RecordTypeId")
                        .HasConstraintName("fk_records_record_types_record_type_id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Notebook.Domain.Entity.RecordsToContacts", b =>
                {
                    b.HasOne("Notebook.Domain.Entity.Contact", "Contact")
                        .WithMany("RecordsToContacts")
                        .HasForeignKey("ContactId")
                        .HasConstraintName("fk_records_to_contacts_contacts_contact_id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Notebook.Domain.Entity.Record", "Record")
                        .WithMany("RecordsToContacts")
                        .HasForeignKey("RecordId")
                        .HasConstraintName("fk_records_to_contacts_records_record_id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}
