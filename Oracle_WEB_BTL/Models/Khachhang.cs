using System;
using System.Collections.Generic;

namespace Oracle_WEB_BTL.Models
{
    public partial class Khachhang
    {
        public Khachhang()
        {
            Hoadonbans = new HashSet<Hoadonban>();
        }

        public decimal Makhach { get; set; }
        public string? Tenkhach { get; set; }
        public string? Diachi { get; set; }
        public string? Dienthoai { get; set; }

        public virtual ICollection<Hoadonban> Hoadonbans { get; set; }
    }
}
