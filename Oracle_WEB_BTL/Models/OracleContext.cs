using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Oracle_WEB_BTL.Models
{
    public partial class OracleContext : DbContext
    {
        public OracleContext()
        {
        }

        public OracleContext(DbContextOptions<OracleContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Chatlieu> Chatlieus { get; set; } = null!;
        public virtual DbSet<Chitiethoadonban> Chitiethoadonbans { get; set; } = null!;
        public virtual DbSet<Chitiethoadonnhap> Chitiethoadonnhaps { get; set; } = null!;
        public virtual DbSet<Congdung> Congdungs { get; set; } = null!;
        public virtual DbSet<Congviec> Congviecs { get; set; } = null!;
        public virtual DbSet<Dacdiem> Dacdiems { get; set; } = null!;
        public virtual DbSet<Dmhanghoa> Dmhanghoas { get; set; } = null!;
        public virtual DbSet<Hinhdang> Hinhdangs { get; set; } = null!;
        public virtual DbSet<Hoadonban> Hoadonbans { get; set; } = null!;
        public virtual DbSet<Hoadonnhap> Hoadonnhaps { get; set; } = null!;
        public virtual DbSet<Khachhang> Khachhangs { get; set; } = null!;
        public virtual DbSet<Kichthuoc> Kichthuocs { get; set; } = null!;
        public virtual DbSet<Loai> Loais { get; set; } = null!;
        public virtual DbSet<Mausac> Mausacs { get; set; } = null!;
        public virtual DbSet<Nhacungcap> Nhacungcaps { get; set; } = null!;
        public virtual DbSet<Nhanvien> Nhanviens { get; set; } = null!;
        public virtual DbSet<Nhasanxuat> Nhasanxuats { get; set; } = null!;
        public virtual DbSet<Nuocsx> Nuocsxes { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseOracle("User Id=QLBG;Password=abc123;Data Source=//DanMinhTran:1521/orcl");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("QLBG")
                .UseCollation("USING_NLS_COMP");

            modelBuilder.Entity<Chatlieu>(entity =>
            {
                entity.HasKey(e => e.Machatlieu)
                    .HasName("SYS_C008925");

                entity.ToTable("CHATLIEU");

                entity.Property(e => e.Machatlieu)
                    .HasColumnType("NUMBER")
                    .ValueGeneratedOnAdd()
                    .HasColumnName("MACHATLIEU");

                entity.Property(e => e.Tenchatlieu)
                    .HasMaxLength(50)
                    .HasColumnName("TENCHATLIEU");
            });

            modelBuilder.Entity<Chitiethoadonban>(entity =>
            {
                entity.HasKey(e => new { e.Sohdb, e.Mahang })
                    .HasName("SYS_C008967");

                entity.ToTable("CHITIETHOADONBAN");

                entity.Property(e => e.Sohdb)
                    .HasColumnType("NUMBER")
                    .HasColumnName("SOHDB");

                entity.Property(e => e.Mahang)
                    .HasColumnType("NUMBER")
                    .HasColumnName("MAHANG");

                entity.Property(e => e.Giamgia)
                    .HasColumnType("NUMBER(5,2)")
                    .HasColumnName("GIAMGIA");

                entity.Property(e => e.Soluong)
                    .HasColumnType("NUMBER")
                    .HasColumnName("SOLUONG");

                entity.HasOne(d => d.MahangNavigation)
                    .WithMany(p => p.Chitiethoadonbans)
                    .HasForeignKey(d => d.Mahang)
                    .HasConstraintName("FK_CTHDB_DMHANGHOA");

                entity.HasOne(d => d.SohdbNavigation)
                    .WithMany(p => p.Chitiethoadonbans)
                    .HasForeignKey(d => d.Sohdb)
                    .HasConstraintName("FK_CTHDB_HOADONBAN");
            });

            modelBuilder.Entity<Chitiethoadonnhap>(entity =>
            {
                entity.HasKey(e => new { e.Sohdn, e.Mahang })
                    .HasName("SYS_C008960");

                entity.ToTable("CHITIETHOADONNHAP");

                entity.Property(e => e.Sohdn)
                    .HasColumnType("NUMBER")
                    .HasColumnName("SOHDN");

                entity.Property(e => e.Mahang)
                    .HasColumnType("NUMBER")
                    .HasColumnName("MAHANG");

                entity.Property(e => e.Giamgia)
                    .HasColumnType("NUMBER(5,2)")
                    .HasColumnName("GIAMGIA");

                entity.Property(e => e.Soluong)
                    .HasColumnType("NUMBER")
                    .HasColumnName("SOLUONG");

                entity.HasOne(d => d.MahangNavigation)
                    .WithMany(p => p.Chitiethoadonnhaps)
                    .HasForeignKey(d => d.Mahang)
                    .HasConstraintName("FK_CTHDN_DMHANGHOA");

                entity.HasOne(d => d.SohdnNavigation)
                    .WithMany(p => p.Chitiethoadonnhaps)
                    .HasForeignKey(d => d.Sohdn)
                    .HasConstraintName("FK_CTHDN_HOADONNHAP");
            });

            modelBuilder.Entity<Congdung>(entity =>
            {
                entity.HasKey(e => e.Macongdung)
                    .HasName("SYS_C008939");

                entity.ToTable("CONGDUNG");

                entity.Property(e => e.Macongdung)
                    .HasColumnType("NUMBER")
                    .ValueGeneratedOnAdd()
                    .HasColumnName("MACONGDUNG");

                entity.Property(e => e.Tencongdung)
                    .HasMaxLength(100)
                    .HasColumnName("TENCONGDUNG");
            });

            modelBuilder.Entity<Congviec>(entity =>
            {
                entity.HasKey(e => e.Macv)
                    .HasName("SYS_C008933");

                entity.ToTable("CONGVIEC");

                entity.Property(e => e.Macv)
                    .HasColumnType("NUMBER")
                    .ValueGeneratedOnAdd()
                    .HasColumnName("MACV");

                entity.Property(e => e.Mucluong)
                    .HasColumnType("NUMBER(18,2)")
                    .HasColumnName("MUCLUONG");

                entity.Property(e => e.Tencv)
                    .HasMaxLength(50)
                    .HasColumnName("TENCV");
            });

            modelBuilder.Entity<Dacdiem>(entity =>
            {
                entity.HasKey(e => e.Madacdiem)
                    .HasName("SYS_C008931");

                entity.ToTable("DACDIEM");

                entity.Property(e => e.Madacdiem)
                    .HasColumnType("NUMBER")
                    .ValueGeneratedOnAdd()
                    .HasColumnName("MADACDIEM");

                entity.Property(e => e.Tendacdiem)
                    .HasMaxLength(50)
                    .HasColumnName("TENDACDIEM");
            });

            modelBuilder.Entity<Dmhanghoa>(entity =>
            {
                entity.HasKey(e => e.Mahang)
                    .HasName("SYS_C008946");

                entity.ToTable("DMHANGHOA");

                entity.Property(e => e.Mahang)
                    .HasColumnType("NUMBER")
                    .ValueGeneratedOnAdd()
                    .HasColumnName("MAHANG");

                entity.Property(e => e.Anh)
                    .HasMaxLength(255)
                    .HasColumnName("ANH");

                entity.Property(e => e.Dongiaban)
                    .HasColumnType("NUMBER(18,2)")
                    .HasColumnName("DONGIABAN");

                entity.Property(e => e.Dongianhap)
                    .HasColumnType("NUMBER(18,2)")
                    .HasColumnName("DONGIANHAP");

                entity.Property(e => e.Ghichu)
                    .HasMaxLength(255)
                    .HasColumnName("GHICHU");

                entity.Property(e => e.Machatlieu)
                    .HasColumnType("NUMBER")
                    .HasColumnName("MACHATLIEU");

                entity.Property(e => e.Macongdung)
                    .HasColumnType("NUMBER")
                    .HasColumnName("MACONGDUNG");

                entity.Property(e => e.Madacdiem)
                    .HasColumnType("NUMBER")
                    .HasColumnName("MADACDIEM");

                entity.Property(e => e.Mahinhdang)
                    .HasColumnType("NUMBER")
                    .HasColumnName("MAHINHDANG");

                entity.Property(e => e.Makichthuoc)
                    .HasColumnType("NUMBER")
                    .HasColumnName("MAKICHTHUOC");

                entity.Property(e => e.Maloai)
                    .HasColumnType("NUMBER")
                    .HasColumnName("MALOAI");

                entity.Property(e => e.Mamau)
                    .HasColumnType("NUMBER")
                    .HasColumnName("MAMAU");

                entity.Property(e => e.Mansx)
                    .HasColumnType("NUMBER")
                    .HasColumnName("MANSX");

                entity.Property(e => e.Manuocsx)
                    .HasColumnType("NUMBER")
                    .HasColumnName("MANUOCSX");

                entity.Property(e => e.Tenhanghoa)
                    .HasMaxLength(50)
                    .HasColumnName("TENHANGHOA");

                entity.Property(e => e.Thoigianbaohanh)
                    .HasColumnType("NUMBER")
                    .HasColumnName("THOIGIANBAOHANH");

                entity.HasOne(d => d.MachatlieuNavigation)
                    .WithMany(p => p.Dmhanghoas)
                    .HasForeignKey(d => d.Machatlieu)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_DMHANGHOA_CHATLIEU");

                entity.HasOne(d => d.MacongdungNavigation)
                    .WithMany(p => p.Dmhanghoas)
                    .HasForeignKey(d => d.Macongdung)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_DMHANGHOA_CONGDUNG");

                entity.HasOne(d => d.MadacdiemNavigation)
                    .WithMany(p => p.Dmhanghoas)
                    .HasForeignKey(d => d.Madacdiem)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_DMHANGHOA_DACDIEM");

                entity.HasOne(d => d.MahinhdangNavigation)
                    .WithMany(p => p.Dmhanghoas)
                    .HasForeignKey(d => d.Mahinhdang)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_DMHANGHOA_HINHDANG");

                entity.HasOne(d => d.MakichthuocNavigation)
                    .WithMany(p => p.Dmhanghoas)
                    .HasForeignKey(d => d.Makichthuoc)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_DMHANGHOA_KICHTHUOC");

                entity.HasOne(d => d.MaloaiNavigation)
                    .WithMany(p => p.Dmhanghoas)
                    .HasForeignKey(d => d.Maloai)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_DMHANGHOA_LOAI");

                entity.HasOne(d => d.MamauNavigation)
                    .WithMany(p => p.Dmhanghoas)
                    .HasForeignKey(d => d.Mamau)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_DMHANGHOA_MAU");

                entity.HasOne(d => d.MansxNavigation)
                    .WithMany(p => p.Dmhanghoas)
                    .HasForeignKey(d => d.Mansx)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_DMHANGHOA_NSX");

                entity.HasOne(d => d.ManuocsxNavigation)
                    .WithMany(p => p.Dmhanghoas)
                    .HasForeignKey(d => d.Manuocsx)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_DMHANGHOA_NUOCSX");
            });

            modelBuilder.Entity<Hinhdang>(entity =>
            {
                entity.HasKey(e => e.Mahinhdang)
                    .HasName("SYS_C008923");

                entity.ToTable("HINHDANG");

                entity.Property(e => e.Mahinhdang)
                    .HasColumnType("NUMBER")
                    .ValueGeneratedOnAdd()
                    .HasColumnName("MAHINHDANG");

                entity.Property(e => e.Tenhinhdang)
                    .HasMaxLength(50)
                    .HasColumnName("TENHINHDANG");
            });

            modelBuilder.Entity<Hoadonban>(entity =>
            {
                entity.HasKey(e => e.Sohdb)
                    .HasName("SYS_C008964");

                entity.ToTable("HOADONBAN");

                entity.Property(e => e.Sohdb)
                    .HasColumnType("NUMBER")
                    .ValueGeneratedOnAdd()
                    .HasColumnName("SOHDB");

                entity.Property(e => e.Makhach)
                    .HasColumnType("NUMBER")
                    .HasColumnName("MAKHACH");

                entity.Property(e => e.Manv)
                    .HasColumnType("NUMBER")
                    .HasColumnName("MANV");

                entity.Property(e => e.Ngayban)
                    .HasColumnType("DATE")
                    .HasColumnName("NGAYBAN");

                entity.HasOne(d => d.MakhachNavigation)
                    .WithMany(p => p.Hoadonbans)
                    .HasForeignKey(d => d.Makhach)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_HOADONBAN_KHACHHANG");

                entity.HasOne(d => d.ManvNavigation)
                    .WithMany(p => p.Hoadonbans)
                    .HasForeignKey(d => d.Manv)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_HOADONBAN_NHANVIEN");
            });

            modelBuilder.Entity<Hoadonnhap>(entity =>
            {
                entity.HasKey(e => e.Sohdn)
                    .HasName("SYS_C008957");

                entity.ToTable("HOADONNHAP");

                entity.Property(e => e.Sohdn)
                    .HasColumnType("NUMBER")
                    .ValueGeneratedOnAdd()
                    .HasColumnName("SOHDN");

                entity.Property(e => e.Mancc)
                    .HasColumnType("NUMBER")
                    .HasColumnName("MANCC");

                entity.Property(e => e.Manv)
                    .HasColumnType("NUMBER")
                    .HasColumnName("MANV");

                entity.Property(e => e.Ngaynhap)
                    .HasColumnType("DATE")
                    .HasColumnName("NGAYNHAP");

                entity.HasOne(d => d.ManccNavigation)
                    .WithMany(p => p.Hoadonnhaps)
                    .HasForeignKey(d => d.Mancc)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_HOADONNHAP_NHACUNGCAP");

                entity.HasOne(d => d.ManvNavigation)
                    .WithMany(p => p.Hoadonnhaps)
                    .HasForeignKey(d => d.Manv)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_HOADONNHAP_NHANVIEN");
            });

            modelBuilder.Entity<Khachhang>(entity =>
            {
                entity.HasKey(e => e.Makhach)
                    .HasName("SYS_C008937");

                entity.ToTable("KHACHHANG");

                entity.Property(e => e.Makhach)
                    .HasColumnType("NUMBER")
                    .ValueGeneratedOnAdd()
                    .HasColumnName("MAKHACH");

                entity.Property(e => e.Diachi)
                    .HasMaxLength(100)
                    .HasColumnName("DIACHI");

                entity.Property(e => e.Dienthoai)
                    .HasMaxLength(15)
                    .HasColumnName("DIENTHOAI");

                entity.Property(e => e.Tenkhach)
                    .HasMaxLength(50)
                    .HasColumnName("TENKHACH");
            });

            modelBuilder.Entity<Kichthuoc>(entity =>
            {
                entity.HasKey(e => e.Makichthuoc)
                    .HasName("SYS_C008919");

                entity.ToTable("KICHTHUOC");

                entity.Property(e => e.Makichthuoc)
                    .HasColumnType("NUMBER")
                    .ValueGeneratedOnAdd()
                    .HasColumnName("MAKICHTHUOC");

                entity.Property(e => e.Tenkichthuoc)
                    .HasMaxLength(50)
                    .HasColumnName("TENKICHTHUOC");
            });

            modelBuilder.Entity<Loai>(entity =>
            {
                entity.HasKey(e => e.Maloai)
                    .HasName("SYS_C008921");

                entity.ToTable("LOAI");

                entity.Property(e => e.Maloai)
                    .HasColumnType("NUMBER")
                    .ValueGeneratedOnAdd()
                    .HasColumnName("MALOAI");

                entity.Property(e => e.Tenloai)
                    .HasMaxLength(50)
                    .HasColumnName("TENLOAI");
            });

            modelBuilder.Entity<Mausac>(entity =>
            {
                entity.HasKey(e => e.Mamau)
                    .HasName("SYS_C008929");

                entity.ToTable("MAUSAC");

                entity.Property(e => e.Mamau)
                    .HasColumnType("NUMBER")
                    .ValueGeneratedOnAdd()
                    .HasColumnName("MAMAU");

                entity.Property(e => e.Tenmau)
                    .HasMaxLength(50)
                    .HasColumnName("TENMAU");
            });

            modelBuilder.Entity<Nhacungcap>(entity =>
            {
                entity.HasKey(e => e.Mancc)
                    .HasName("SYS_C008935");

                entity.ToTable("NHACUNGCAP");

                entity.Property(e => e.Mancc)
                    .HasColumnType("NUMBER")
                    .ValueGeneratedOnAdd()
                    .HasColumnName("MANCC");

                entity.Property(e => e.Diachi)
                    .HasMaxLength(100)
                    .HasColumnName("DIACHI");

                entity.Property(e => e.Dienthoai)
                    .HasMaxLength(15)
                    .HasColumnName("DIENTHOAI");

                entity.Property(e => e.Tenncc)
                    .HasMaxLength(50)
                    .HasColumnName("TENNCC");
            });

            modelBuilder.Entity<Nhanvien>(entity =>
            {
                entity.HasKey(e => e.Manv)
                    .HasName("SYS_C008943");

                entity.ToTable("NHANVIEN");

                entity.Property(e => e.Manv)
                    .HasColumnType("NUMBER")
                    .ValueGeneratedOnAdd()
                    .HasColumnName("MANV");

                entity.Property(e => e.Anh)
                    .HasMaxLength(255)
                    .HasColumnName("ANH");

                entity.Property(e => e.Diachi)
                    .HasMaxLength(100)
                    .HasColumnName("DIACHI");

                entity.Property(e => e.Dienthoai)
                    .HasMaxLength(15)
                    .HasColumnName("DIENTHOAI");

                entity.Property(e => e.Email)
                    .HasMaxLength(50)
                    .HasColumnName("EMAIL");

                entity.Property(e => e.Gioitinh)
                    .HasPrecision(1)
                    .HasColumnName("GIOITINH");

                entity.Property(e => e.Macv)
                    .HasColumnType("NUMBER")
                    .HasColumnName("MACV");

                entity.Property(e => e.Matkhau)
                    .HasMaxLength(50)
                    .HasColumnName("MATKHAU");

                entity.Property(e => e.Ngaysinh)
                    .HasColumnType("DATE")
                    .HasColumnName("NGAYSINH");

                entity.Property(e => e.Quyenadmin)
                    .HasPrecision(1)
                    .HasColumnName("QUYENADMIN");

                entity.Property(e => e.Tennv)
                    .HasMaxLength(50)
                    .HasColumnName("TENNV");

                entity.HasOne(d => d.MacvNavigation)
                    .WithMany(p => p.Nhanviens)
                    .HasForeignKey(d => d.Macv)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_NHANVIEN_CONGVIEC");
            });

            modelBuilder.Entity<Nhasanxuat>(entity =>
            {
                entity.HasKey(e => e.Mansx)
                    .HasName("SYS_C008941");

                entity.ToTable("NHASANXUAT");

                entity.Property(e => e.Mansx)
                    .HasColumnType("NUMBER")
                    .ValueGeneratedOnAdd()
                    .HasColumnName("MANSX");

                entity.Property(e => e.Tennsx)
                    .HasMaxLength(50)
                    .HasColumnName("TENNSX");
            });

            modelBuilder.Entity<Nuocsx>(entity =>
            {
                entity.HasKey(e => e.Manuocsx)
                    .HasName("SYS_C008927");

                entity.ToTable("NUOCSX");

                entity.Property(e => e.Manuocsx)
                    .HasColumnType("NUMBER")
                    .ValueGeneratedOnAdd()
                    .HasColumnName("MANUOCSX");

                entity.Property(e => e.Tennuocsx)
                    .HasMaxLength(50)
                    .HasColumnName("TENNUOCSX");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
