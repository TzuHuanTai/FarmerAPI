using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace FarmerAPI.Models.Weather
{
    public partial class WeatherContext : DbContext
    {
        public virtual DbSet<Actions> Actions { get; set; }
        public virtual DbSet<City> City { get; set; }
        public virtual DbSet<Ctrl> Ctrl { get; set; }
        public virtual DbSet<IactionRole> IactionRole { get; set; }
        public virtual DbSet<ImemRole> ImemRole { get; set; }
        public virtual DbSet<ImenuRole> ImenuRole { get; set; }
        public virtual DbSet<Member> Member { get; set; }
        public virtual DbSet<Menu> Menu { get; set; }
        public virtual DbSet<RealTime> RealTime { get; set; }
        public virtual DbSet<RoleGroup> RoleGroup { get; set; }
        public virtual DbSet<StationInfo> StationInfo { get; set; }
        public virtual DbSet<SystemLog> SystemLog { get; set; }
        public virtual DbSet<Token> Token { get; set; }
        public virtual DbSet<WeatherData> WeatherData { get; set; }

        //        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //        {
        //            if (!optionsBuilder.IsConfigured)
        //            {
        //#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
        //                optionsBuilder.UseSqlServer(@"Server=localhost\SQLEXPRESS; Database=Weather;Trusted_Connection=True; User ID=sa;Password=2ooixuui;");
        //            }
        //        }
        public WeatherContext(DbContextOptions<WeatherContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Actions>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Description)
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.Property(e => e.Method)
                    .IsRequired()
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.HasOne(d => d.Controller)
                    .WithMany(p => p.Actions)
                    .HasForeignKey(d => d.ControllerId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Action_Ctrl");
            });

            modelBuilder.Entity<City>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Name).HasMaxLength(10);
            });

            modelBuilder.Entity<Ctrl>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Description)
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<IactionRole>(entity =>
            {
                entity.HasKey(e => new { e.ActionId, e.RoleId });

                entity.ToTable("IActionRole");

                entity.HasOne(d => d.Action)
                    .WithMany(p => p.IactionRole)
                    .HasForeignKey(d => d.ActionId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_IActionRole_Action");

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.IactionRole)
                    .HasForeignKey(d => d.RoleId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_IActionRole_RoleGroup");
            });

            modelBuilder.Entity<ImemRole>(entity =>
            {
                entity.HasKey(e => new { e.Account, e.RoleId });

                entity.ToTable("IMemRole");

                entity.Property(e => e.Account)
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.Property(e => e.RoleId).HasColumnName("RoleID");

                entity.HasOne(d => d.AccountNavigation)
                    .WithMany(p => p.ImemRole)
                    .HasForeignKey(d => d.Account)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_IMemRole_Member");

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.ImemRole)
                    .HasForeignKey(d => d.RoleId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_IMemRole_RoleGroup");
            });

            modelBuilder.Entity<ImenuRole>(entity =>
            {
                entity.HasKey(e => new { e.MenuId, e.RoleId });

                entity.ToTable("IMenuRole");

                entity.Property(e => e.MenuId).HasColumnName("MenuID");

                entity.Property(e => e.RoleId).HasColumnName("RoleID");

                entity.HasOne(d => d.Menu)
                    .WithMany(p => p.ImenuRole)
                    .HasForeignKey(d => d.MenuId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_IMenuRole_Menu");

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.ImenuRole)
                    .HasForeignKey(d => d.RoleId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_IMenuRole_RoleGroup");
            });

            modelBuilder.Entity<Member>(entity =>
            {
                entity.HasKey(e => e.Account);

                entity.Property(e => e.Account)
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .ValueGeneratedNever();

                entity.Property(e => e.AddTime).HasColumnType("datetime");

                entity.Property(e => e.DeptId)
                    .HasColumnName("DeptID")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Domain).HasMaxLength(50);

                entity.Property(e => e.Email).HasMaxLength(50);

                entity.Property(e => e.FirstName)
                    .IsRequired()
                    .HasMaxLength(20);

                entity.Property(e => e.LastName).HasMaxLength(20);

                entity.Property(e => e.Password).HasMaxLength(50);

                entity.Property(e => e.UpdatedTime).HasColumnType("datetime");
            });

            modelBuilder.Entity<Menu>(entity =>
            {
                entity.Property(e => e.MenuId)
                    .HasColumnName("MenuID")
                    .ValueGeneratedNever();

                entity.Property(e => e.Selector).HasMaxLength(50);

                entity.Property(e => e.Component).HasMaxLength(50);

                entity.Property(e => e.MenuText)
                    .IsRequired()
                    .HasMaxLength(20);

                entity.Property(e => e.Path)
                    .IsRequired()
                    .HasMaxLength(20);

                entity.Property(e => e.RootMenuId).HasColumnName("RootMenuID");
            });

            modelBuilder.Entity<RealTime>(entity =>
            {
                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .ValueGeneratedNever();

                entity.Property(e => e.Rh)
                    .HasColumnName("RH")
                    .HasColumnType("decimal?(5, 2)");

				entity.Property(e => e.Lux)
					.HasColumnName("Lux")
					.HasColumnType("decimal?(5, 2)");

				entity.Property(e => e.Temperature).HasColumnType("decimal?(5, 2)");
            });

            modelBuilder.Entity<RoleGroup>(entity =>
            {
                entity.HasKey(e => e.RoleId);

                entity.Property(e => e.RoleId)
                    .HasColumnName("RoleID")
                    .ValueGeneratedNever();

                entity.Property(e => e.Description).HasMaxLength(100);

                entity.Property(e => e.ParentRoleId).HasColumnName("ParentRoleID");

                entity.Property(e => e.RoleName)
                    .IsRequired()
                    .HasMaxLength(20);
            });

            modelBuilder.Entity<StationInfo>(entity =>
            {
                entity.Property(e => e.Address).HasMaxLength(100);

                entity.Property(e => e.Name).HasMaxLength(10);

                entity.HasOne(d => d.City)
                    .WithMany(p => p.StationInfo)
                    .HasForeignKey(d => d.CityId)
                    .HasConstraintName("FK_StationInfo_City");
            });

            modelBuilder.Entity<SystemLog>(entity =>
            {
                entity.HasKey(e => e.Num);

                entity.Property(e => e.Account)
                    .IsRequired()
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.Property(e => e.Action)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Detail).HasColumnType("ntext");

                entity.Property(e => e.Ip)
                    .HasColumnName("IP")
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.LogTime).HasColumnType("datetime");

                entity.Property(e => e.Method)
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.Route)
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Token>(entity =>
            {
                entity.HasKey(e => e.Account);

                entity.Property(e => e.Account)
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .ValueGeneratedNever();

                entity.Property(e => e.AuthTime).HasColumnType("datetime");

                entity.Property(e => e.ExpiredTime).HasColumnType("datetime");

                entity.Property(e => e.Ip)
                    .HasColumnName("IP")
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.Property(e => e.TokenCode)
                    .IsRequired()
                    .HasMaxLength(300)
                    .IsUnicode(false);

                entity.HasOne(d => d.AccountNavigation)
                    .WithOne(p => p.Token)
                    .HasForeignKey<Token>(d => d.Account)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Token_Member");
            });

            modelBuilder.Entity<WeatherData>(entity =>
            {
                entity.HasKey(e => new { e.StationId, e.ObsTime });

                entity.Property(e => e.ObsTime).HasColumnType("datetime");

                entity.Property(e => e.GlobalRad).HasColumnType("decimal?(5, 2)");

                entity.Property(e => e.Precp).HasColumnType("decimal?(4, 1)");

                entity.Property(e => e.PrecpHour).HasColumnType("decimal?(3, 1)");

                entity.Property(e => e.Rh)
                    .HasColumnName("RH")
                    .HasColumnType("decimal?(3, 0)");

                entity.Property(e => e.SeaPres).HasColumnType("decimal?(5, 1)");

                entity.Property(e => e.StnPres).HasColumnType("decimal?(5, 1)");

                entity.Property(e => e.SunShine).HasColumnType("decimal?(2, 1)");

                entity.Property(e => e.Td).HasColumnType("decimal?(3, 1)");

                entity.Property(e => e.Temperature).HasColumnType("decimal?(3, 1)");

                entity.Property(e => e.Visb).HasColumnType("decimal?(3, 1)");

                entity.Property(e => e.Wd)
                    .HasColumnName("WD")
                    .HasColumnType("decimal?(3, 0)");

                entity.Property(e => e.Wdgust)
                    .HasColumnName("WDGust")
                    .HasColumnType("decimal?(3, 0)");

                entity.Property(e => e.Ws)
                    .HasColumnName("WS")
                    .HasColumnType("decimal?(3, 1)");

                entity.Property(e => e.Wsgust)
                    .HasColumnName("WSGust")
                    .HasColumnType("decimal?(3, 1)");

                entity.HasOne(d => d.StationIdNavigation)
                    .WithMany(p => p.WeatherData)
                    .HasForeignKey(d => d.StationId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_WeatherData_StationInfo");
            });
        }
    }
}
