using System;
using System.Collections.Generic;

namespace Oracle_WEB_BTL.Models
{
    public partial class Chitiethoadonnhap
    {
        public decimal Sohdn { get; set; }
        public decimal Mahang { get; set; }
        public decimal? Soluong { get; set; }
        public decimal? Giamgia { get; set; }

        public virtual Dmhanghoa MahangNavigation { get; set; } = null!;
        public virtual Hoadonnhap SohdnNavigation { get; set; } = null!;
    }
}
