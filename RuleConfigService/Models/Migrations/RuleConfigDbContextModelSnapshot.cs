﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using RuleConfigService.Models;

#nullable disable

namespace RuleConfigService.Migrations
{
    [DbContext(typeof(RuleConfigDbContext))]
    partial class RuleConfigDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.12")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("Rule", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("EventType")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<bool>("IsCritical")
                        .HasColumnType("boolean");

                    b.HasKey("Id");

                    b.ToTable("Rules");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            EventType = "SELECT",
                            IsCritical = false
                        },
                        new
                        {
                            Id = 2,
                            EventType = "DELETE",
                            IsCritical = true
                        },
                        new
                        {
                            Id = 3,
                            EventType = "UPDATE",
                            IsCritical = true
                        });
                });
#pragma warning restore 612, 618
        }
    }
}
