﻿// <auto-generated />
using Inventory_Backend_NET.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace Inventory_Backend_NET.Migrations
{
    [DbContext(typeof(MyDbContext))]
    [Migration("20231222010249_init")]
    partial class init
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("Inventory_Backend_NET.Models.Barang", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<long>("CreatedAt")
                        .HasColumnType("bigint");

                    b.Property<int>("CurrentStock")
                        .HasColumnType("int");

                    b.Property<int>("KategoriId")
                        .HasColumnType("int");

                    b.Property<string>("KodeBarang")
                        .IsRequired()
                        .HasMaxLength(20)
                        .HasColumnType("nvarchar(20)");

                    b.Property<int>("LastMonthStock")
                        .HasColumnType("int");

                    b.Property<int>("MinStock")
                        .HasColumnType("int");

                    b.Property<string>("Nama")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<int>("NomorKolom")
                        .HasColumnType("int");

                    b.Property<int>("NomorLaci")
                        .HasColumnType("int");

                    b.Property<int>("NomorRak")
                        .HasColumnType("int");

                    b.Property<int>("UnitPrice")
                        .HasColumnType("int");

                    b.Property<string>("Uom")
                        .IsRequired()
                        .HasMaxLength(20)
                        .HasColumnType("nvarchar(20)");

                    b.HasKey("Id");

                    b.HasIndex("KategoriId");

                    b.HasIndex("KodeBarang")
                        .IsUnique();

                    b.HasIndex("Nama")
                        .IsUnique();

                    b.HasIndex("NomorRak", "NomorLaci", "NomorKolom")
                        .IsUnique();

                    b.ToTable("Barangs");
                });

            modelBuilder.Entity("Inventory_Backend_NET.Models.BarangAjuan", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("BarangId")
                        .HasColumnType("int");

                    b.Property<string>("Keterangan")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("PengajuanId")
                        .HasColumnType("int");

                    b.Property<int>("Quantity")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("BarangId");

                    b.HasIndex("PengajuanId");

                    b.ToTable("BarangAjuans");
                });

            modelBuilder.Entity("Inventory_Backend_NET.Models.Kategori", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Nama")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Id");

                    b.HasIndex("Nama")
                        .IsUnique();

                    b.ToTable("Kategoris");
                });

            modelBuilder.Entity("Inventory_Backend_NET.Models.Pengaju", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<bool>("IsPemasok")
                        .HasColumnType("bit");

                    b.Property<string>("Nama")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Id");

                    b.HasIndex("Nama")
                        .IsUnique();

                    b.ToTable("Pengajus");
                });

            modelBuilder.Entity("Inventory_Backend_NET.Models.Pengajuan", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("KodeTransaksi")
                        .IsRequired()
                        .HasMaxLength(25)
                        .HasColumnType("nvarchar(25)");

                    b.Property<int>("PengajuId")
                        .HasColumnType("int");

                    b.Property<string>("Status")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.Property<long>("WaktuPengajuan")
                        .HasColumnType("bigint");

                    b.HasKey("Id");

                    b.HasIndex("KodeTransaksi")
                        .IsUnique();

                    b.HasIndex("PengajuId");

                    b.HasIndex("UserId");

                    b.ToTable("Pengajuans");
                });

            modelBuilder.Entity("Inventory_Backend_NET.Models.StatusPengajuan", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Value")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("StatusPengajuans");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            Value = "diterima"
                        },
                        new
                        {
                            Id = 2,
                            Value = "menunggu"
                        },
                        new
                        {
                            Id = 3,
                            Value = "ditolak"
                        });
                });

            modelBuilder.Entity("Inventory_Backend_NET.Models.TotalPengajuanByTanggal", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Tanggal")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Total")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("TotalPengajuanByTanggals");
                });

            modelBuilder.Entity("Inventory_Backend_NET.Models.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<bool>("IsAdmin")
                        .HasColumnType("bit");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Username")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Id");

                    b.HasIndex("Username")
                        .IsUnique();

                    b.ToTable("Users");
                });

            modelBuilder.Entity("Inventory_Backend_NET.Models.Barang", b =>
                {
                    b.HasOne("Inventory_Backend_NET.Models.Kategori", "Kategori")
                        .WithMany("Barangs")
                        .HasForeignKey("KategoriId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Kategori");
                });

            modelBuilder.Entity("Inventory_Backend_NET.Models.BarangAjuan", b =>
                {
                    b.HasOne("Inventory_Backend_NET.Models.Barang", "Barang")
                        .WithMany()
                        .HasForeignKey("BarangId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Inventory_Backend_NET.Models.Pengajuan", "Pengajuan")
                        .WithMany("BarangAjuans")
                        .HasForeignKey("PengajuanId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Barang");

                    b.Navigation("Pengajuan");
                });

            modelBuilder.Entity("Inventory_Backend_NET.Models.Pengajuan", b =>
                {
                    b.HasOne("Inventory_Backend_NET.Models.Pengaju", "Pengaju")
                        .WithMany()
                        .HasForeignKey("PengajuId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Inventory_Backend_NET.Models.User", "User")
                        .WithMany("Pengajuans")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Pengaju");

                    b.Navigation("User");
                });

            modelBuilder.Entity("Inventory_Backend_NET.Models.Kategori", b =>
                {
                    b.Navigation("Barangs");
                });

            modelBuilder.Entity("Inventory_Backend_NET.Models.Pengajuan", b =>
                {
                    b.Navigation("BarangAjuans");
                });

            modelBuilder.Entity("Inventory_Backend_NET.Models.User", b =>
                {
                    b.Navigation("Pengajuans");
                });
#pragma warning restore 612, 618
        }
    }
}
