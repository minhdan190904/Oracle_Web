using System;
using System.Collections.Generic;

namespace Oracle_WEB_BTL.Models
{
    public partial class Dacdiem
    {
        public Dacdiem()
        {
            Dmhanghoas = new HashSet<Dmhanghoa>();
        }

        public decimal Madacdiem { get; set; }
        public string? Tendacdiem { get; set; }

        public virtual ICollection<Dmhanghoa> Dmhanghoas { get; set; }
    }
}
