using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;

namespace Oracle_WEB_BTL.Models
{
    public partial class Chitiethoadonban
    {
        public decimal Sohdb { get; set; }
        public decimal Mahang { get; set; }
        public decimal? Soluong { get; set; }
        public decimal? Giamgia { get; set; }

        [ValidateNever]
        public virtual Dmhanghoa MahangNavigation { get; set; }
        [ValidateNever]
        public virtual Hoadonban SohdbNavigation { get; set; }
    }
}
