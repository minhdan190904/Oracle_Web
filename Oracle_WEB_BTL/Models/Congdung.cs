using System;
using System.Collections.Generic;

namespace Oracle_WEB_BTL.Models
{
    public partial class Congdung
    {
        public Congdung()
        {
            Dmhanghoas = new HashSet<Dmhanghoa>();
        }

        public decimal Macongdung { get; set; }
        public string? Tencongdung { get; set; }

        public virtual ICollection<Dmhanghoa> Dmhanghoas { get; set; }
    }
}
