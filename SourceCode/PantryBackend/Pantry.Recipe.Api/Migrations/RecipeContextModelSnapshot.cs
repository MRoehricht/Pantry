﻿// <auto-generated />
using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using Pantry.Recipe.Api.Database.Contexts;

#nullable disable

namespace Pantry.Recipe.Api.Migrations
{
    [DbContext(typeof(RecipeContext))]
    partial class RecipeContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("Pantry.Recipe.Api.Database.Entities.RecipeEntity", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnType("timestamp without time zone");

                    b.Property<string>("Description")
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Owner")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("Recipes");
                });

            modelBuilder.Entity("Pantry.Recipe.Api.Database.Entities.RecipeEntity", b =>
                {
                    b.OwnsMany("Pantry.Recipe.Api.Database.Entities.IngredientEntity", "Ingredients", b1 =>
                        {
                            b1.Property<Guid>("RecipeEntityId")
                                .HasColumnType("uuid");

                            b1.Property<int>("Id")
                                .ValueGeneratedOnAdd()
                                .HasColumnType("integer");

                            b1.Property<double>("CountOff")
                                .HasColumnType("double precision");

                            b1.Property<string>("Name")
                                .IsRequired()
                                .HasColumnType("text");

                            b1.Property<Guid?>("PantryItemId")
                                .HasColumnType("uuid");

                            b1.Property<int>("Unit")
                                .HasColumnType("integer");

                            b1.HasKey("RecipeEntityId", "Id");

                            b1.ToTable("Recipes");

                            b1.ToJson("Ingredients");

                            b1.WithOwner()
                                .HasForeignKey("RecipeEntityId");
                        });

                    b.OwnsOne("Pantry.Recipe.Api.Database.Entities.RecipeDetailsEntity", "Details", b1 =>
                        {
                            b1.Property<Guid>("RecipeEntityId")
                                .HasColumnType("uuid");

                            b1.Property<List<DateOnly>>("CookedOn")
                                .IsRequired()
                                .HasColumnType("date[]");

                            b1.Property<List<int>>("Reviews")
                                .IsRequired()
                                .HasColumnType("integer[]");

                            b1.Property<List<string>>("Tags")
                                .IsRequired()
                                .HasColumnType("text[]");

                            b1.HasKey("RecipeEntityId");

                            b1.ToTable("Recipes");

                            b1.ToJson("Details");

                            b1.WithOwner()
                                .HasForeignKey("RecipeEntityId");
                        });

                    b.Navigation("Details")
                        .IsRequired();

                    b.Navigation("Ingredients");
                });
#pragma warning restore 612, 618
        }
    }
}
