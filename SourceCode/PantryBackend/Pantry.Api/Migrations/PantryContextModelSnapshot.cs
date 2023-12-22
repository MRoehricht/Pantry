﻿// <auto-generated />
using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using Pantry.Api.Database.Contexts;

#nullable disable

namespace Pantry.Api.Migrations
{
    [DbContext(typeof(PantryContext))]
    partial class PantryContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("Pantry.Api.Database.Entities.GoodEntity", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<double>("Amount")
                        .HasColumnType("double precision");

                    b.Property<double?>("CurrentPrice")
                        .HasColumnType("double precision");

                    b.Property<string>("Description")
                        .HasColumnType("text");

                    b.Property<long?>("EAN")
                        .HasColumnType("bigint");

                    b.Property<double?>("MinimumAmount")
                        .HasColumnType("double precision");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<Guid>("OwnerId")
                        .HasColumnType("uuid");

                    b.Property<string>("ShoppinglistName")
                        .HasColumnType("text");

                    b.Property<string>("StorageLocation")
                        .HasColumnType("text");

                    b.Property<int>("UnitOfMeasurement")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.ToTable("Goods");
                });

            modelBuilder.Entity("Pantry.Api.Database.Entities.PriceHistoryEntity", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<DateOnly>("Date")
                        .HasColumnType("date");

                    b.Property<Guid?>("GoodEntityId")
                        .HasColumnType("uuid");

                    b.Property<Guid>("GoodId")
                        .HasColumnType("uuid");

                    b.Property<double>("Price")
                        .HasColumnType("double precision");

                    b.HasKey("Id");

                    b.HasIndex("GoodEntityId");

                    b.ToTable("PriceHistories");
                });

            modelBuilder.Entity("Pantry.Api.Database.Entities.GoodEntity", b =>
                {
                    b.OwnsOne("Pantry.Api.Database.Entities.GoodDetailsEntity", "Details", b1 =>
                        {
                            b1.Property<Guid>("GoodEntityId")
                                .HasColumnType("uuid");

                            b1.Property<int?>("DaysUntilConsume")
                                .HasColumnType("integer");

                            b1.Property<List<string>>("PurchaseLocations")
                                .IsRequired()
                                .HasColumnType("text[]");

                            b1.Property<List<int>>("Ratings")
                                .IsRequired()
                                .HasColumnType("integer[]");

                            b1.Property<List<string>>("Tags")
                                .IsRequired()
                                .HasColumnType("text[]");

                            b1.Property<int>("TotalPurchaseNumber")
                                .HasColumnType("integer");

                            b1.HasKey("GoodEntityId");

                            b1.ToTable("Goods");

                            b1.ToJson("Details");

                            b1.WithOwner()
                                .HasForeignKey("GoodEntityId");
                        });

                    b.Navigation("Details")
                        .IsRequired();
                });

            modelBuilder.Entity("Pantry.Api.Database.Entities.PriceHistoryEntity", b =>
                {
                    b.HasOne("Pantry.Api.Database.Entities.GoodEntity", "GoodEntity")
                        .WithMany("PriceHistories")
                        .HasForeignKey("GoodEntityId");

                    b.Navigation("GoodEntity");
                });

            modelBuilder.Entity("Pantry.Api.Database.Entities.GoodEntity", b =>
                {
                    b.Navigation("PriceHistories");
                });
#pragma warning restore 612, 618
        }
    }
}
