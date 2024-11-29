using System;
using System.Collections.Generic;

namespace Oracle_WEB_BTL.Models
{
    public partial class Chatlieu
    {
        public Chatlieu()
        {
            Dmhanghoas = new HashSet<Dmhanghoa>();
        }

        public decimal Machatlieu { get; set; }
        public string? Tenchatlieu { get; set; }

        public virtual ICollection<Dmhanghoa> Dmhanghoas { get; set; }
    }
}
