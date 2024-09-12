﻿// <auto-generated />
using System;
using Investment1.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace Investment1.Migrations
{
    [DbContext(typeof(InvestmentDbContext))]
    [Migration("20240808045914_tables")]
    partial class tables
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.7")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("Investment1.Models.MutualFund", b =>
                {
                    b.Property<int>("MfId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("MfId"));

                    b.Property<int>("Category")
                        .HasColumnType("int");

                    b.Property<int>("LockinPeriod")
                        .HasColumnType("int");

                    b.Property<string>("MfName")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<decimal>("NAV")
                        .HasColumnType("decimal(18, 2)");

                    b.HasKey("MfId");

                    b.ToTable("MutualFunds");
                });

            modelBuilder.Entity("Investment1.Models.Portfolio", b =>
                {
                    b.Property<int>("InvestmentId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("InvestmentId"));

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("DueDate")
                        .HasColumnType("datetime2");

                    b.Property<int>("MfId")
                        .HasColumnType("int");

                    b.Property<decimal?>("SipAmount")
                        .HasColumnType("decimal(18, 2)");

                    b.Property<int>("Status")
                        .HasColumnType("int");

                    b.Property<decimal>("TotalCurrent")
                        .HasColumnType("decimal(18, 2)");

                    b.Property<decimal>("TotalInvestment")
                        .HasColumnType("decimal(18, 2)");

                    b.Property<int>("Type")
                        .HasColumnType("int");

                    b.Property<decimal>("Unit")
                        .HasColumnType("decimal(18,2)");

                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("datetime2");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.HasKey("InvestmentId");

                    b.HasIndex("MfId");

                    b.HasIndex("UserId");

                    b.ToTable("Portfolios");
                });

            modelBuilder.Entity("Investment1.Models.ROI", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("MfId")
                        .HasColumnType("int");

                    b.Property<decimal>("ROI_1M")
                        .HasColumnType("decimal(5, 2)");

                    b.Property<decimal>("ROI_1Y")
                        .HasColumnType("decimal(5, 2)");

                    b.Property<decimal>("ROI_3Y")
                        .HasColumnType("decimal(5, 2)");

                    b.Property<decimal>("ROI_5Y")
                        .HasColumnType("decimal(5, 2)");

                    b.Property<decimal>("ROI_6M")
                        .HasColumnType("decimal(5, 2)");

                    b.HasKey("Id");

                    b.HasIndex("MfId");

                    b.ToTable("ROIs");
                });

            modelBuilder.Entity("Investment1.Models.Support", b =>
                {
                    b.Property<int>("SupportId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("SupportId"));

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Subject")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.HasKey("SupportId");

                    b.HasIndex("UserId");

                    b.ToTable("Supports");
                });

            modelBuilder.Entity("Investment1.Models.Transaction", b =>
                {
                    b.Property<int>("TransactionId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("TransactionId"));

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<int>("InvestmentId")
                        .HasColumnType("int");

                    b.Property<decimal>("SipAmount")
                        .HasColumnType("decimal(18, 2)");

                    b.Property<int>("Type")
                        .HasColumnType("int");

                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("datetime2");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.HasKey("TransactionId");

                    b.HasIndex("InvestmentId");

                    b.HasIndex("UserId");

                    b.ToTable("Transactions");
                });

            modelBuilder.Entity("Investment1.Models.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Aadhaar")
                        .IsRequired()
                        .HasMaxLength(20)
                        .HasColumnType("nvarchar(20)");

                    b.Property<string>("AccountNo")
                        .IsRequired()
                        .HasMaxLength(20)
                        .HasColumnType("nvarchar(20)");

                    b.Property<string>("Address")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("DOB")
                        .HasColumnType("date");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("IFSCCode")
                        .IsRequired()
                        .HasMaxLength(20)
                        .HasColumnType("nvarchar(20)");

                    b.Property<string>("Mobile")
                        .IsRequired()
                        .HasMaxLength(20)
                        .HasColumnType("nvarchar(20)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("PAN")
                        .IsRequired()
                        .HasMaxLength(20)
                        .HasColumnType("nvarchar(20)");

                    b.Property<string>("PIN")
                        .IsRequired()
                        .HasMaxLength(10)
                        .HasColumnType("nvarchar(10)");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("Investment1.Models.Portfolio", b =>
                {
                    b.HasOne("Investment1.Models.MutualFund", "MutualFund")
                        .WithMany()
                        .HasForeignKey("MfId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("Investment1.Models.User", "User")
                        .WithMany("Portfolios")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("MutualFund");

                    b.Navigation("User");
                });

            modelBuilder.Entity("Investment1.Models.ROI", b =>
                {
                    b.HasOne("Investment1.Models.MutualFund", "MutualFund")
                        .WithMany()
                        .HasForeignKey("MfId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("MutualFund");
                });

            modelBuilder.Entity("Investment1.Models.Support", b =>
                {
                    b.HasOne("Investment1.Models.User", "User")
                        .WithMany("Supports")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("Investment1.Models.Transaction", b =>
                {
                    b.HasOne("Investment1.Models.Portfolio", "Portfolio")
                        .WithMany("Transactions")
                        .HasForeignKey("InvestmentId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("Investment1.Models.User", "User")
                        .WithMany("Transactions")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Portfolio");

                    b.Navigation("User");
                });

            modelBuilder.Entity("Investment1.Models.Portfolio", b =>
                {
                    b.Navigation("Transactions");
                });

            modelBuilder.Entity("Investment1.Models.User", b =>
                {
                    b.Navigation("Portfolios");

                    b.Navigation("Supports");

                    b.Navigation("Transactions");
                });
#pragma warning restore 612, 618
        }
    }
}
