using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Oracle_WEB_BTL.Models
{
    public partial class Hoadonnhap
    {
        public Hoadonnhap()
        {
            Chitiethoadonnhaps = new HashSet<Chitiethoadonnhap>();
        }

        public decimal Sohdn { get; set; }
        public decimal? Manv { get; set; }
        public DateTime? Ngaynhap { get; set; }
        public decimal? Mancc { get; set; }

        public virtual Nhacungcap? ManccNavigation { get; set; }
        public virtual Nhanvien? ManvNavigation { get; set; }
        public virtual ICollection<Chitiethoadonnhap> Chitiethoadonnhaps { get; set; }
        [NotMapped]
        public decimal? TongTien { get; set; }
    }
}
