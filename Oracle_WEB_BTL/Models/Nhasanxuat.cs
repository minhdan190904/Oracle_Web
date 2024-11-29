using System;
using System.Collections.Generic;

namespace Oracle_WEB_BTL.Models
{
    public partial class Nhasanxuat
    {
        public Nhasanxuat()
        {
            Dmhanghoas = new HashSet<Dmhanghoa>();
        }

        public decimal Mansx { get; set; }
        public string? Tennsx { get; set; }

        public virtual ICollection<Dmhanghoa> Dmhanghoas { get; set; }
    }
}
