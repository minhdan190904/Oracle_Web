using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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

        [Display(Name = "Tên hàng hóa")]
        public string? Tenhanghoa { get; set; }

        [Display(Name = "Loại")]
        public decimal? Maloai { get; set; }

        [Display(Name = "Kích thước")]
        public decimal? Makichthuoc { get; set; }

        [Display(Name = "Hình dạng")]
        public decimal? Mahinhdang { get; set; }

        [Display(Name = "Chất liệu")]
        public decimal? Machatlieu { get; set; }

        [Display(Name = "Nước sản xuất")]
        public decimal? Manuocsx { get; set; }

        [Display(Name = "Đặc điểm")]
        public decimal? Madacdiem { get; set; }

        [Display(Name = "Màu sắc")]
        public decimal? Mamau { get; set; }

        [Display(Name = "Công dụng")]
        public decimal? Macongdung { get; set; }

        [Display(Name = "Nhà sản xuất")]
        public decimal? Mansx { get; set; }

        [Display(Name = "Đơn giá nhập")]
        public decimal? Dongianhap { get; set; }

        [Display(Name = "Đơn giá bán")]
        public decimal? Dongiaban { get; set; }

        [Display(Name = "Thời gian bảo hành")]
        public decimal? Thoigianbaohanh { get; set; }

        [NotMapped]
        [Display(Name = "Số lượng")]
        public decimal? Soluong { get; set; } // Thêm trường Soluong

        [Display(Name = "Ảnh")]
        public string? Anh { get; set; }

        [Display(Name = "Ghi chú")]
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
