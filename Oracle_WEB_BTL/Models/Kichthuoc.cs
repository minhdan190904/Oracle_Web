using System;
using System.Collections.Generic;

namespace Oracle_WEB_BTL.Models
{
    public partial class Kichthuoc
    {
        public Kichthuoc()
        {
            Dmhanghoas = new HashSet<Dmhanghoa>();
        }

        public decimal Makichthuoc { get; set; }
        public string? Tenkichthuoc { get; set; }

        public virtual ICollection<Dmhanghoa> Dmhanghoas { get; set; }
    }
}
