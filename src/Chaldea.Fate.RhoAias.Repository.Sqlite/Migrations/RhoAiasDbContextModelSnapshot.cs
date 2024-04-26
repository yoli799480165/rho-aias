﻿// <auto-generated />
using System;
using Chaldea.Fate.RhoAias.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace Chaldea.Fate.RhoAias.Repository.Sqlite.Migrations
{
    [DbContext(typeof(RhoAiasDbContext))]
    partial class RhoAiasDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "8.0.4");

            modelBuilder.Entity("Chaldea.Fate.RhoAias.Cert", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<string>("CertInfo")
                        .HasColumnType("TEXT");

                    b.Property<int>("CertType")
                        .HasColumnType("INTEGER");

                    b.Property<Guid?>("DnsProviderId")
                        .HasColumnType("TEXT");

                    b.Property<string>("Domain")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<DateTime?>("Expires")
                        .HasColumnType("TEXT");

                    b.Property<string>("Issuer")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int>("Status")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.ToTable("Certs");
                });

            modelBuilder.Entity("Chaldea.Fate.RhoAias.Client", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<string>("ConnectionId")
                        .HasColumnType("TEXT");

                    b.Property<string>("Endpoint")
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<bool>("Status")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Token")
                        .HasColumnType("TEXT");

                    b.Property<string>("Version")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("Clients");
                });

            modelBuilder.Entity("Chaldea.Fate.RhoAias.DnsProvider", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<string>("Config")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("LatestRecordId")
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Provider")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("DnsProviders");
                });

            modelBuilder.Entity("Chaldea.Fate.RhoAias.Proxy", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<Guid>("ClientId")
                        .HasColumnType("TEXT");

                    b.Property<string>("Hosts")
                        .HasColumnType("TEXT");

                    b.Property<string>("LocalIP")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int>("LocalPort")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Path")
                        .HasColumnType("TEXT");

                    b.Property<int>("RemotePort")
                        .HasColumnType("INTEGER");

                    b.Property<int>("Type")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("ClientId");

                    b.ToTable("Proxies");
                });

            modelBuilder.Entity("Chaldea.Fate.RhoAias.User", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("UserName")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("Chaldea.Fate.RhoAias.Proxy", b =>
                {
                    b.HasOne("Chaldea.Fate.RhoAias.Client", "Client")
                        .WithMany("Proxies")
                        .HasForeignKey("ClientId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Client");
                });

            modelBuilder.Entity("Chaldea.Fate.RhoAias.Client", b =>
                {
                    b.Navigation("Proxies");
                });
#pragma warning restore 612, 618
        }
    }
}
