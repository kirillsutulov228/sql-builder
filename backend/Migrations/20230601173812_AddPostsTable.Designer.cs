﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace backend.Migrations
{
    [DbContext(typeof(TaskDbContext))]
    [Migration("20230601173812_AddPostsTable")]
    partial class AddPostsTable
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "7.0.5");

            modelBuilder.Entity("Task", b =>
                {
                    b.Property<int>("taskId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("correctAnswer")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("taskDescription")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("taskName")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int>("taskNum")
                        .HasColumnType("INTEGER");

                    b.HasKey("taskId");

                    b.ToTable("Tasks", (string)null);
                });

            modelBuilder.Entity("User", b =>
                {
                    b.Property<int>("userId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("age")
                        .HasColumnType("INTEGER");

                    b.Property<string>("firstname")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("hobby")
                        .HasColumnType("TEXT");

                    b.Property<string>("lastname")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<DateOnly>("registrationDate")
                        .HasColumnType("TEXT");

                    b.HasKey("userId");

                    b.ToTable("Users", (string)null);
                });

            modelBuilder.Entity("backend.DataBase.Models.Post", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("AuthoruserId")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("TEXT");

                    b.Property<string>("Text")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("AuthoruserId");

                    b.ToTable("Posts", (string)null);
                });

            modelBuilder.Entity("backend.DataBase.Models.Post", b =>
                {
                    b.HasOne("User", "Author")
                        .WithMany("Posts")
                        .HasForeignKey("AuthoruserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Author");
                });

            modelBuilder.Entity("User", b =>
                {
                    b.Navigation("Posts");
                });
#pragma warning restore 612, 618
        }
    }
}
