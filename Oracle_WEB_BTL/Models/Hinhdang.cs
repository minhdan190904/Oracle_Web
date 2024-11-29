using System;
using System.Collections.Generic;

namespace Oracle_WEB_BTL.Models
{
    public partial class Hinhdang
    {
        public Hinhdang()
        {
            Dmhanghoas = new HashSet<Dmhanghoa>();
        }

        public decimal Mahinhdang { get; set; }
        public string? Tenhinhdang { get; set; }

        public virtual ICollection<Dmhanghoa> Dmhanghoas { get; set; }
    }
}
