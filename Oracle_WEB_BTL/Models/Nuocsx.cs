using System;
using System.Collections.Generic;

namespace Oracle_WEB_BTL.Models
{
    public partial class Nuocsx
    {
        public Nuocsx()
        {
            Dmhanghoas = new HashSet<Dmhanghoa>();
        }

        public decimal Manuocsx { get; set; }
        public string? Tennuocsx { get; set; }

        public virtual ICollection<Dmhanghoa> Dmhanghoas { get; set; }
    }
}
