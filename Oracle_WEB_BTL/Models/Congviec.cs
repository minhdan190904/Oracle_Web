using System;
using System.Collections.Generic;

namespace Oracle_WEB_BTL.Models
{
    public partial class Congviec
    {
        public Congviec()
        {
            Nhanviens = new HashSet<Nhanvien>();
        }

        public decimal Macv { get; set; }
        public string? Tencv { get; set; }
        public decimal? Mucluong { get; set; }

        public virtual ICollection<Nhanvien> Nhanviens { get; set; }
    }
}
