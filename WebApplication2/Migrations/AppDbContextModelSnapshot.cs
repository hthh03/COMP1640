﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace WebApplication2.Migrations
{
    [DbContext(typeof(AppDbContext))]
    partial class AppDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.2")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("WebApplication2.Data.Blogs", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("text");

                    b.Property<string>("Content")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTime>("CreateAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("UsersId")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("UsersId");

                    b.ToTable("Blogs");
                });

            modelBuilder.Entity("WebApplication2.Data.Comments", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("text");

                    b.Property<string>("PostId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("PostsId")
                        .HasColumnType("text");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("UsersId")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("PostsId");

                    b.HasIndex("UsersId");

                    b.ToTable("Comments");
                });

            modelBuilder.Entity("WebApplication2.Data.Files", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("text");

                    b.Property<string>("StudentId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("StudentId");

                    b.ToTable("Files");
                });

            modelBuilder.Entity("WebApplication2.Data.Meetings", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("text");

                    b.Property<string>("StudentId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("StudentsId")
                        .HasColumnType("text");

                    b.Property<string>("TeacherId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("TeachersId")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("StudentsId");

                    b.HasIndex("TeachersId");

                    b.ToTable("Meetings");
                });

            modelBuilder.Entity("WebApplication2.Data.Messages", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("text");

                    b.Property<string>("StudentId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("StudentsId")
                        .HasColumnType("text");

                    b.Property<string>("TeacherId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("TeachersId")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("StudentsId");

                    b.HasIndex("TeachersId");

                    b.ToTable("Messages");
                });

            modelBuilder.Entity("WebApplication2.Data.Posts", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("text");

                    b.Property<string>("BlogId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("BlogsId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("UsersId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("userId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("BlogsId");

                    b.HasIndex("UsersId");

                    b.ToTable("Posts");
                });

            modelBuilder.Entity("WebApplication2.Data.Roles", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("Roles");
                });

            modelBuilder.Entity("WebApplication2.Data.Staffs", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("Staffs");
                });

            modelBuilder.Entity("WebApplication2.Data.Students", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("Students");
                });

            modelBuilder.Entity("WebApplication2.Data.Teachers", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("Teachers");
                });

            modelBuilder.Entity("WebApplication2.Data.UserRoles", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("text");

                    b.Property<string>("RoleId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("RolesId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("UsersId")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("RolesId");

                    b.HasIndex("UsersId");

                    b.ToTable("UserRoles");
                });

            modelBuilder.Entity("WebApplication2.Data.Users", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("text");

                    b.Property<string>("Address")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Fullname")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Phone")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Username")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("WebApplication2.Data.Blogs", b =>
                {
                    b.HasOne("WebApplication2.Data.Users", "Users")
                        .WithMany()
                        .HasForeignKey("UsersId");

                    b.Navigation("Users");
                });

            modelBuilder.Entity("WebApplication2.Data.Comments", b =>
                {
                    b.HasOne("WebApplication2.Data.Posts", "Posts")
                        .WithMany("Comments")
                        .HasForeignKey("PostsId");

                    b.HasOne("WebApplication2.Data.Users", "Users")
                        .WithMany()
                        .HasForeignKey("UsersId");

                    b.Navigation("Posts");

                    b.Navigation("Users");
                });

            modelBuilder.Entity("WebApplication2.Data.Files", b =>
                {
                    b.HasOne("WebApplication2.Data.Students", "Student")
                        .WithMany()
                        .HasForeignKey("StudentId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Student");
                });

            modelBuilder.Entity("WebApplication2.Data.Meetings", b =>
                {
                    b.HasOne("WebApplication2.Data.Students", "Students")
                        .WithMany()
                        .HasForeignKey("StudentsId");

                    b.HasOne("WebApplication2.Data.Teachers", "Teachers")
                        .WithMany()
                        .HasForeignKey("TeachersId");

                    b.Navigation("Students");

                    b.Navigation("Teachers");
                });

            modelBuilder.Entity("WebApplication2.Data.Messages", b =>
                {
                    b.HasOne("WebApplication2.Data.Students", "Students")
                        .WithMany()
                        .HasForeignKey("StudentsId");

                    b.HasOne("WebApplication2.Data.Teachers", "Teachers")
                        .WithMany()
                        .HasForeignKey("TeachersId");

                    b.Navigation("Students");

                    b.Navigation("Teachers");
                });

            modelBuilder.Entity("WebApplication2.Data.Posts", b =>
                {
                    b.HasOne("WebApplication2.Data.Blogs", "Blogs")
                        .WithMany("Posts")
                        .HasForeignKey("BlogsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("WebApplication2.Data.Users", "Users")
                        .WithMany()
                        .HasForeignKey("UsersId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Blogs");

                    b.Navigation("Users");
                });

            modelBuilder.Entity("WebApplication2.Data.UserRoles", b =>
                {
                    b.HasOne("WebApplication2.Data.Roles", "Roles")
                        .WithMany("UserRoles")
                        .HasForeignKey("RolesId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("WebApplication2.Data.Users", "Users")
                        .WithMany("UserRoles")
                        .HasForeignKey("UsersId");

                    b.Navigation("Roles");

                    b.Navigation("Users");
                });

            modelBuilder.Entity("WebApplication2.Data.Blogs", b =>
                {
                    b.Navigation("Posts");
                });

            modelBuilder.Entity("WebApplication2.Data.Posts", b =>
                {
                    b.Navigation("Comments");
                });

            modelBuilder.Entity("WebApplication2.Data.Roles", b =>
                {
                    b.Navigation("UserRoles");
                });

            modelBuilder.Entity("WebApplication2.Data.Users", b =>
                {
                    b.Navigation("UserRoles");
                });
#pragma warning restore 612, 618
        }
    }
}
