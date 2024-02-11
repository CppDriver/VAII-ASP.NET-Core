﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using MultimediaLibrary.Data;

#nullable disable

namespace MultimediaLibrary.Migrations
{
    [DbContext(typeof(DatabaseContext))]
    [Migration("20240211123851_GalleryFix")]
    partial class GalleryFix
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.14")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("GalleryMedia", b =>
                {
                    b.Property<string>("MediaUuid")
                        .HasColumnType("VARCHAR(36)");

                    b.Property<decimal>("GalleryId")
                        .HasColumnType("decimal(20,0)");

                    b.HasKey("MediaUuid", "GalleryId");

                    b.HasIndex("GalleryId");

                    b.ToTable("GalleryMedia");
                });

            modelBuilder.Entity("MediaTags", b =>
                {
                    b.Property<decimal>("TagId")
                        .HasColumnType("decimal(20,0)");

                    b.Property<string>("MediaUuid")
                        .HasColumnType("VARCHAR(36)");

                    b.HasKey("TagId", "MediaUuid");

                    b.HasIndex("MediaUuid");

                    b.ToTable("MediaTags");
                });

            modelBuilder.Entity("MultimediaLibrary.Models.Comment", b =>
                {
                    b.Property<decimal>("CommentId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("decimal(20,0)");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<decimal>("CommentId"));

                    b.Property<DateTime>("DateCreated")
                        .HasColumnType("datetime2");

                    b.Property<decimal?>("GalleryId")
                        .HasColumnType("decimal(20,0)");

                    b.Property<string>("MediaUuid")
                        .HasMaxLength(36)
                        .HasColumnType("VARCHAR");

                    b.Property<string>("Text")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<decimal>("UserId")
                        .HasColumnType("decimal(20,0)");

                    b.HasKey("CommentId");

                    b.HasIndex("GalleryId");

                    b.HasIndex("MediaUuid");

                    b.HasIndex("UserId");

                    b.ToTable("Comments");
                });

            modelBuilder.Entity("MultimediaLibrary.Models.Gallery", b =>
                {
                    b.Property<decimal>("GalleryId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("decimal(20,0)");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<decimal>("GalleryId"));

                    b.Property<int>("Access")
                        .HasColumnType("int");

                    b.Property<DateTime?>("DateCreated")
                        .HasColumnType("datetime2");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<decimal>("UserId")
                        .HasColumnType("decimal(20,0)");

                    b.HasKey("GalleryId");

                    b.HasIndex("UserId");

                    b.ToTable("Galleries");
                });

            modelBuilder.Entity("MultimediaLibrary.Models.Media", b =>
                {
                    b.Property<string>("MediaUuid")
                        .HasMaxLength(36)
                        .HasColumnType("VARCHAR");

                    b.Property<int>("Access")
                        .HasColumnType("int");

                    b.Property<string>("Blurhash")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("DateCreated")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("DateUpdated")
                        .HasColumnType("datetime2");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Exif")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Height")
                        .HasColumnType("int");

                    b.Property<int>("Size")
                        .HasColumnType("int");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<decimal>("UserId")
                        .HasColumnType("decimal(20,0)");

                    b.Property<int>("Width")
                        .HasColumnType("int");

                    b.HasKey("MediaUuid");

                    b.HasIndex("UserId");

                    b.ToTable("Media");
                });

            modelBuilder.Entity("MultimediaLibrary.Models.Tag", b =>
                {
                    b.Property<decimal>("TagId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("decimal(20,0)");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<decimal>("TagId"));

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("TagId");

                    b.ToTable("Tags");
                });

            modelBuilder.Entity("MultimediaLibrary.Models.User", b =>
                {
                    b.Property<decimal>("UserId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("decimal(20,0)");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<decimal>("UserId"));

                    b.Property<string>("Bio")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Location")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Salt")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Surname")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Username")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("UserId");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("UserFollowers", b =>
                {
                    b.Property<decimal>("FollowingUserId")
                        .HasColumnType("decimal(20,0)");

                    b.Property<decimal>("FollowerUserId")
                        .HasColumnType("decimal(20,0)");

                    b.HasKey("FollowingUserId", "FollowerUserId");

                    b.HasIndex("FollowerUserId");

                    b.ToTable("UserFollowers");
                });

            modelBuilder.Entity("GalleryMedia", b =>
                {
                    b.HasOne("MultimediaLibrary.Models.Gallery", null)
                        .WithMany()
                        .HasForeignKey("GalleryId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("MultimediaLibrary.Models.Media", null)
                        .WithMany()
                        .HasForeignKey("MediaUuid")
                        .OnDelete(DeleteBehavior.ClientCascade)
                        .IsRequired();
                });

            modelBuilder.Entity("MediaTags", b =>
                {
                    b.HasOne("MultimediaLibrary.Models.Media", null)
                        .WithMany()
                        .HasForeignKey("MediaUuid")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("MultimediaLibrary.Models.Tag", null)
                        .WithMany()
                        .HasForeignKey("TagId")
                        .OnDelete(DeleteBehavior.ClientCascade)
                        .IsRequired();
                });

            modelBuilder.Entity("MultimediaLibrary.Models.Comment", b =>
                {
                    b.HasOne("MultimediaLibrary.Models.Gallery", "Gallery")
                        .WithMany()
                        .HasForeignKey("GalleryId");

                    b.HasOne("MultimediaLibrary.Models.Media", "Media")
                        .WithMany("Comments")
                        .HasForeignKey("MediaUuid");

                    b.HasOne("MultimediaLibrary.Models.User", "User")
                        .WithMany("Comments")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Gallery");

                    b.Navigation("Media");

                    b.Navigation("User");
                });

            modelBuilder.Entity("MultimediaLibrary.Models.Gallery", b =>
                {
                    b.HasOne("MultimediaLibrary.Models.User", "User")
                        .WithMany("Galleries")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("MultimediaLibrary.Models.Media", b =>
                {
                    b.HasOne("MultimediaLibrary.Models.User", "User")
                        .WithMany("Media")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("UserFollowers", b =>
                {
                    b.HasOne("MultimediaLibrary.Models.User", null)
                        .WithMany()
                        .HasForeignKey("FollowerUserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("MultimediaLibrary.Models.User", null)
                        .WithMany()
                        .HasForeignKey("FollowingUserId")
                        .OnDelete(DeleteBehavior.ClientCascade)
                        .IsRequired();
                });

            modelBuilder.Entity("MultimediaLibrary.Models.Media", b =>
                {
                    b.Navigation("Comments");
                });

            modelBuilder.Entity("MultimediaLibrary.Models.User", b =>
                {
                    b.Navigation("Comments");

                    b.Navigation("Galleries");

                    b.Navigation("Media");
                });
#pragma warning restore 612, 618
        }
    }
}