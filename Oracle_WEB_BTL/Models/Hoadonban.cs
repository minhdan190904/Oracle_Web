using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Oracle_WEB_BTL.Models
{
    public partial class Hoadonban
    {
        public Hoadonban()
        {
            Chitiethoadonbans = new HashSet<Chitiethoadonban>();
        }

        public decimal Sohdb { get; set; }
        public decimal? Manv { get; set; }
        public DateTime? Ngayban { get; set; }
        public decimal? Makhach { get; set; }
        [NotMapped]
        public decimal? TongTien { get; set; }
        [ValidateNever]
        public virtual Khachhang MakhachNavigation { get; set; }
        [ValidateNever]
        public virtual Nhanvien ManvNavigation { get; set; }
        [ValidateNever]
        public virtual ICollection<Chitiethoadonban> Chitiethoadonbans { get; set; }
    }
}
