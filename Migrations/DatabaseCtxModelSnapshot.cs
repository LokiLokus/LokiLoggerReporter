﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using lokiloggerreporter.Models;

namespace lokiloggerreporter.Migrations
{
    [DbContext(typeof(DatabaseCtx))]
    partial class DatabaseCtxModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.1.8-servicing-32085")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            modelBuilder.Entity("lokiloggerreporter.Models.Log", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Class");

                    b.Property<string>("Data");

                    b.Property<string>("Exception");

                    b.Property<int>("Line");

                    b.Property<int>("LogLevel");

                    b.Property<int>("LogTyp");

                    b.Property<string>("Message");

                    b.Property<string>("Method");

                    b.Property<string>("SourceId");

                    b.Property<int>("ThreadId");

                    b.Property<DateTime>("Time");

                    b.HasKey("ID");

                    b.HasIndex("SourceId");

                    b.ToTable("Logs");
                });

            modelBuilder.Entity("lokiloggerreporter.Models.Source", b =>
                {
                    b.Property<string>("SourceId")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Description");

                    b.Property<string>("Name");

                    b.Property<string>("Secret");

                    b.Property<string>("Tag");

                    b.Property<string>("Version");

                    b.HasKey("SourceId");

                    b.ToTable("Sources");
                });

            modelBuilder.Entity("lokiloggerreporter.Models.Log", b =>
                {
                    b.HasOne("lokiloggerreporter.Models.Source", "Source")
                        .WithMany()
                        .HasForeignKey("SourceId");
                });
#pragma warning restore 612, 618
        }
    }
}
