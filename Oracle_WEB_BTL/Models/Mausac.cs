using System;
using System.Collections.Generic;

namespace Oracle_WEB_BTL.Models
{
    public partial class Mausac
    {
        public Mausac()
        {
            Dmhanghoas = new HashSet<Dmhanghoa>();
        }

        public decimal Mamau { get; set; }
        public string? Tenmau { get; set; }

        public virtual ICollection<Dmhanghoa> Dmhanghoas { get; set; }
    }
}
