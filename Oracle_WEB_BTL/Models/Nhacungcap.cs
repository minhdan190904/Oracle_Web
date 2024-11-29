using System;
using System.Collections.Generic;

namespace Oracle_WEB_BTL.Models
{
    public partial class Nhacungcap
    {
        public Nhacungcap()
        {
            Hoadonnhaps = new HashSet<Hoadonnhap>();
        }

        public decimal Mancc { get; set; }
        public string? Tenncc { get; set; }
        public string? Diachi { get; set; }
        public string? Dienthoai { get; set; }

        public virtual ICollection<Hoadonnhap> Hoadonnhaps { get; set; }
    }
}
