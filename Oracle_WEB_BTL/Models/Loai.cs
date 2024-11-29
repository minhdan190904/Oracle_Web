using System;
using System.Collections.Generic;

namespace Oracle_WEB_BTL.Models
{
    public partial class Loai
    {
        public Loai()
        {
            Dmhanghoas = new HashSet<Dmhanghoa>();
        }

        public decimal Maloai { get; set; }
        public string? Tenloai { get; set; }

        public virtual ICollection<Dmhanghoa> Dmhanghoas { get; set; }
    }
}
