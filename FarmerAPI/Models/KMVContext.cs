using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace FarmerAPI.Models
{
    public partial class KMVContext : DbContext
    {
      //  public virtual DbSet<A12> A12 { get; set; }
        public virtual DbSet<V34> V34 { get; set; }

       
        
        //        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //        {
        //            if (!optionsBuilder.IsConfigured)
        //            {
        //#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
        //                optionsBuilder.UseSqlServer(@"Server=127.0.0.1; Database=frudat;Trusted_Connection=True; User ID=sa;Password=keymaN;");
        //            }
        //        }

        //自行加入，進行初始化
        public KMVContext(DbContextOptions<KMVContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<V34>(entity =>
            {
                entity.HasKey(e => new { e.V3401, e.V3404 });

                entity.ToTable("v34");

                entity.HasIndex(e => new { e.V3401, e.V3404 })
                    .HasName("v34_k")
                    .IsUnique();

                entity.Property(e => e.V3401)
                    .HasColumnName("v34_01")
                    .HasColumnType("char(8)")
                    .HasDefaultValueSql("(' ')");

                entity.Property(e => e.V3404)
                    .HasColumnName("v34_04")
                    .HasColumnType("decimal?(1, 0)")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.V3402)
                    .HasColumnName("v34_02")
                    .HasColumnType("char(50)")
                    .HasDefaultValueSql("(' ')");

                entity.Property(e => e.V3403)
                    .HasColumnName("v34_03")
                    .HasColumnType("char(8)")
                    .HasDefaultValueSql("(' ')");

                entity.Property(e => e.V3405)
                    .HasColumnName("v34_05")
                    .HasColumnType("decimal?(1, 0)")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.V3406)
                    .HasColumnName("v34_06")
                    .HasColumnType("char(30)")
                    .HasDefaultValueSql("(' ')");

                entity.Property(e => e.V3407)
                    .HasColumnName("v34_07")
                    .HasColumnType("char(30)")
                    .HasDefaultValueSql("(' ')");

                entity.Property(e => e.V3408)
                    .HasColumnName("v34_08")
                    .HasColumnType("char(10)")
                    .HasDefaultValueSql("(' ')");

                entity.Property(e => e.V3409)
                    .HasColumnName("v34_09")
                    .HasColumnType("char(8)")
                    .HasDefaultValueSql("(' ')");

                entity.Property(e => e.V3410)
                    .HasColumnName("v34_10")
                    .HasColumnType("decimal?(9, 0)")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.V3411)
                    .HasColumnName("v34_11")
                    .HasColumnType("char(8)")
                    .HasDefaultValueSql("(' ')");

                entity.Property(e => e.V3412)
                    .HasColumnName("v34_12")
                    .HasColumnType("char(22)")
                    .HasDefaultValueSql("(' ')");

                entity.Property(e => e.V3413)
                    .HasColumnName("v34_13")
                    .HasColumnType("char(16)")
                    .HasDefaultValueSql("(' ')");

                entity.Property(e => e.V3414)
                    .HasColumnName("v34_14")
                    .HasColumnType("char(50)")
                    .HasDefaultValueSql("(' ')");

                entity.Property(e => e.V3415)
                    .HasColumnName("v34_15")
                    .HasColumnType("char(16)")
                    .HasDefaultValueSql("(' ')");

                entity.Property(e => e.V3416)
                    .HasColumnName("v34_16")
                    .HasColumnType("char(8)")
                    .HasDefaultValueSql("(' ')");

                entity.Property(e => e.V3417)
                    .HasColumnName("v34_17")
                    .HasColumnType("char(8)")
                    .HasDefaultValueSql("(' ')");

                entity.Property(e => e.V3418)
                    .HasColumnName("v34_18")
                    .HasColumnType("decimal?(1, 0)")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.V3419)
                    .HasColumnName("v34_19")
                    .HasColumnType("char(1)")
                    .HasDefaultValueSql("(' ')");

                entity.Property(e => e.V3420)
                    .HasColumnName("v34_20")
                    .HasColumnType("char(60)")
                    .HasDefaultValueSql("(' ')");

                entity.Property(e => e.V3421)
                    .HasColumnName("v34_21")
                    .HasColumnType("char(60)")
                    .HasDefaultValueSql("(' ')");

                entity.Property(e => e.V3422)
                    .HasColumnName("v34_22")
                    .HasColumnType("char(60)")
                    .HasDefaultValueSql("(' ')");

                entity.Property(e => e.V3423)
                    .HasColumnName("v34_23")
                    .HasColumnType("char(70)")
                    .HasDefaultValueSql("(' ')");

                entity.Property(e => e.V3424)
                    .HasColumnName("v34_24")
                    .HasColumnType("char(8)")
                    .HasDefaultValueSql("(' ')");

                entity.Property(e => e.V3425)
                    .HasColumnName("v34_25")
                    .HasColumnType("char(16)")
                    .HasDefaultValueSql("(' ')");

                entity.Property(e => e.V3426)
                    .HasColumnName("v34_26")
                    .HasColumnType("char(16)")
                    .HasDefaultValueSql("(' ')");

                entity.Property(e => e.V3427)
                    .HasColumnName("v34_27")
                    .HasColumnType("decimal?(1, 0)")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.V3428)
                    .HasColumnName("v34_28")
                    .HasColumnType("decimal?(3, 0)")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.V3429)
                    .HasColumnName("v34_29")
                    .HasColumnType("decimal?(1, 0)")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.V3430)
                    .HasColumnName("v34_30")
                    .HasColumnType("char(1)")
                    .HasDefaultValueSql("(' ')");

                entity.Property(e => e.V3431)
                    .HasColumnName("v34_31")
                    .HasColumnType("char(1)")
                    .HasDefaultValueSql("(' ')");

                entity.Property(e => e.V3432)
                    .HasColumnName("v34_32")
                    .HasColumnType("decimal?(2, 0)")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.V3433)
                    .HasColumnName("v34_33")
                    .HasColumnType("char(20)")
                    .HasDefaultValueSql("(' ')");

                entity.Property(e => e.V3434)
                    .HasColumnName("v34_34")
                    .HasColumnType("char(20)")
                    .HasDefaultValueSql("(' ')");

                entity.Property(e => e.V3435)
                    .HasColumnName("v34_35")
                    .HasColumnType("decimal?(18, 15)");

                entity.Property(e => e.V3436)
                    .HasColumnName("v34_36")
                    .HasColumnType("decimal?(18, 15)");

                entity.Property(e => e.V3496)
                    .HasColumnName("v34_96")
                    .HasColumnType("char(8)")
                    .HasDefaultValueSql("(' ')");

                entity.Property(e => e.V3497)
                    .HasColumnName("v34_97")
                    .HasColumnType("datetime");

                entity.Property(e => e.V3498)
                    .HasColumnName("v34_98")
                    .HasColumnType("char(8)")
                    .HasDefaultValueSql("(' ')");

                entity.Property(e => e.V3499)
                    .HasColumnName("v34_99")
                    .HasColumnType("datetime");
            });
        }
    }
}
