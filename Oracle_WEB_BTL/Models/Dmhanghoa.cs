using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Oracle_WEB_BTL.Models
{
    public partial class Dmhanghoa
    {
        public Dmhanghoa()
        {
            Chitiethoadonbans = new HashSet<Chitiethoadonban>();
            Chitiethoadonnhaps = new HashSet<Chitiethoadonnhap>();
        }

        public decimal Mahang { get; set; }
        public string? Tenhanghoa { get; set; }
        public decimal? Maloai { get; set; }
        public decimal? Makichthuoc { get; set; }
        public decimal? Mahinhdang { get; set; }
        public decimal? Machatlieu { get; set; }
        public decimal? Manuocsx { get; set; }
        public decimal? Madacdiem { get; set; }
        public decimal? Mamau { get; set; }
        public decimal? Macongdung { get; set; }
        public decimal? Mansx { get; set; }
        public decimal? Dongianhap { get; set; }
        public decimal? Dongiaban { get; set; }
        public decimal? Thoigianbaohanh { get; set; }
        [NotMapped]
        public decimal? Soluong { get; set; } // Thêm trường Soluong
        public string? Anh { get; set; }
        public string? Ghichu { get; set; }

        public virtual Chatlieu? MachatlieuNavigation { get; set; }
        public virtual Congdung? MacongdungNavigation { get; set; }
        public virtual Dacdiem? MadacdiemNavigation { get; set; }
        public virtual Hinhdang? MahinhdangNavigation { get; set; }
        public virtual Kichthuoc? MakichthuocNavigation { get; set; }
        public virtual Loai? MaloaiNavigation { get; set; }
        public virtual Mausac? MamauNavigation { get; set; }
        public virtual Nhasanxuat? MansxNavigation { get; set; }
        public virtual Nuocsx? ManuocsxNavigation { get; set; }
        public virtual ICollection<Chitiethoadonban> Chitiethoadonbans { get; set; }
        public virtual ICollection<Chitiethoadonnhap> Chitiethoadonnhaps { get; set; }
    }
}
