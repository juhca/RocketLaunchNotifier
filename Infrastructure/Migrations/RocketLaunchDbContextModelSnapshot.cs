﻿// <auto-generated />
using System;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace RocketLauncherNotifier.Migrations
{
    [DbContext(typeof(RocketLaunchDbContext))]
    partial class RocketLaunchDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "8.0.14");

            modelBuilder.Entity("RocketLauncherNotifier.Models.ApiCallTracking.ApiCallTrackingEntity", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<DateTime?>("LastSuccessfulCall")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("ApiCallTracking");
                });

            modelBuilder.Entity("RocketLauncherNotifier.Models.Email.EmailEntity", b =>
                {
                    b.Property<int>("EmailId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("EmailId");

                    b.ToTable("Emails");
                });

            modelBuilder.Entity("RocketLauncherNotifier.Models.RocketLaunch.RocketLaunchEntity", b =>
                {
                    b.Property<string>("RockerLaunchEntryId")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("LastUpdated")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("LaunchDate")
                        .HasColumnType("TEXT");

                    b.Property<string>("LaunchDescription")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("LaunchStatus")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Url")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("RockerLaunchEntryId");

                    b.ToTable("RocketLaunches");
                });
#pragma warning restore 612, 618
        }
    }
}
