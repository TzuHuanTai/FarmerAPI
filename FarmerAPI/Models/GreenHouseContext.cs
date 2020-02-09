using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace FarmerAPI.Models.SQLite
{
    public partial class GreenHouseContext : DbContext
    {
        public GreenHouseContext()
        {
        }

        public GreenHouseContext(DbContextOptions<GreenHouseContext> options)
            : base(options)
        {
        }

        public virtual DbSet<City> City { get; set; }
        public virtual DbSet<CwbData> CwbData { get; set; }
        public virtual DbSet<Climate> Climate { get; set; }
        public virtual DbSet<StationInfo> StationInfo { get; set; }

        //        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //        {
        //            if (!optionsBuilder.IsConfigured)
        //            {
        //#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
        //                optionsBuilder.UseSqlite("DataSource=D:\\sqlite_db\\GreenHouse.db");
        //            }
        //        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            var converter = new ValueConverter<decimal?, string>(
                v => v.ToString(),
                v => v == string.Empty ? (decimal?)null : decimal.Parse(v)
            );

            modelBuilder.Entity<City>(entity =>
            {
                entity.ToTable("city");

                entity.Property(e => e.CityId).ValueGeneratedNever();

                entity.Property(e => e.Name).HasColumnType("VARCHAR(10)");
            });

            modelBuilder.Entity<Climate>(entity =>
            {
                entity.ToTable("climate");

                entity.HasKey(e => new { e.ObsTime });

                entity.Property(e => e.ObsTime).HasColumnType("DATETIME");

                entity.Property(e => e.Temperature)
                    .HasColumnType("NUMERIC (3, 1)")
                    .HasConversion(converter);

                entity.Property(e => e.Rh)
                   .HasColumnName("Rh")
                   .HasColumnType("NUMERIC (3, 1)")
                   .HasConversion(converter);

                entity.Property(e => e.Lux)
                   .HasColumnName("Lux")
                   .HasColumnType("NUMERIC (7, 1)")
                   .HasConversion(converter);
            });

            modelBuilder.Entity<CwbData>(entity =>
            {
                entity.HasKey(e => new { e.StationId, e.ObsTime });

                entity.ToTable("cwb_data");

                entity.Property(e => e.ObsTime).HasColumnType("DATETIME");

                entity.Property(e => e.GlobalRad)
                    .HasColumnType("NUMERIC (5, 2)")
                    .HasConversion(converter);

                entity.Property(e => e.Precp)
                    .HasColumnType("NUMERIC (4, 1)")
                    .HasConversion(converter);

                entity.Property(e => e.PrecpHour)
                    .HasColumnType("NUMERIC (3, 1)")
                    .HasConversion(converter);

                entity.Property(e => e.Rh)
                    .HasColumnName("RH")
                    .HasColumnType("NUMERIC (3, 0)")
                    .HasConversion(converter);

                entity.Property(e => e.SeaPres)
                    .HasColumnType("NUMERIC (5, 1)")
                    .HasConversion(converter);

                entity.Property(e => e.StnPres)
                    .HasColumnType("NUMERIC (5, 1)")
                    .HasConversion(converter);

                entity.Property(e => e.SunShine)
                    .HasColumnType("NUMERIC (2, 1)")
                    .HasConversion(converter);

                entity.Property(e => e.Td)
                    .HasColumnType("NUMERIC (3, 1)")
                    .HasConversion(converter);

                entity.Property(e => e.Temperature)
                    .HasColumnType("NUMERIC (3, 1)")
                    .HasConversion(converter);

                entity.Property(e => e.Visb)
                    .HasColumnType("NUMERIC (3, 1)")
                    .HasConversion(converter);

                entity.Property(e => e.Wd)
                    .HasColumnName("WD")
                    .HasColumnType("NUMERIC (3, 0)")
                    .HasConversion(converter);

                entity.Property(e => e.Wdgust)
                    .HasColumnName("WDGust")
                    .HasColumnType("NUMERIC (3, 0)")
                    .HasConversion(converter);

                entity.Property(e => e.Ws)
                    .HasColumnName("WS")
                    .HasColumnType("NUMERIC (3, 1)")
                    .HasConversion(converter);

                entity.Property(e => e.Wsgust)
                    .HasColumnName("WSGust")
                    .HasColumnType("NUMERIC (3, 1)")
                    .HasConversion(converter);

                entity.HasOne(d => d.Station)
                    .WithMany(p => p.CwbData)
                    .HasForeignKey(d => d.StationId)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<StationInfo>(entity =>
            {
                entity.HasKey(e => e.StationId);

                entity.ToTable("station_info");

                entity.Property(e => e.StationId).ValueGeneratedNever();

                entity.Property(e => e.Address).HasColumnType("VARCHAR (100)");

                entity.Property(e => e.Name).HasColumnType("VARCHAR (10)");

                entity.Property(e => e.Latitude)
                    .HasColumnType("NUMERIC (18, 15)")
                    .HasConversion(converter);

                entity.Property(e => e.Longitude)
                    .HasColumnType("NUMERIC (18, 15)")
                    .HasConversion(converter);

                entity.HasOne(d => d.City)
                    .WithMany(p => p.StationInfo)
                    .HasForeignKey(d => d.CityId);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
