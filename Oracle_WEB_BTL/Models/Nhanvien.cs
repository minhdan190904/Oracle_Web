using Oracle_WEB_BTL.ValidationAttributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Oracle_WEB_BTL.Models
{
    public partial class Nhanvien
    {
        public Nhanvien()
        {
            Hoadonbans = new HashSet<Hoadonban>();
            Hoadonnhaps = new HashSet<Hoadonnhap>();
        }

        [Key]
        [Display(Name = "Mã Nhân Viên")]
        public decimal Manv { get; set; }

        [Required(ErrorMessage = "Tên nhân viên là bắt buộc.")]
        [StringLength(100, ErrorMessage = "Tên nhân viên không được vượt quá 100 ký tự.")]
        [Display(Name = "Tên Nhân Viên")]
        public string? Tennv { get; set; }

        [Required(ErrorMessage = "Giới tính là bắt buộc.")]
        [Display(Name = "Giới Tính")]
        public bool? Gioitinh { get; set; }

        [Required(ErrorMessage = "Ngày sinh là bắt buộc.")]
        [DataType(DataType.Date)]
        [Display(Name = "Ngày Sinh")]
        [MinimumAge(18, ErrorMessage = "Nhân viên phải trên 18 tuổi.")]
        public DateTime? Ngaysinh { get; set; }

        [Required(ErrorMessage = "Số điện thoại là bắt buộc.")]
        [Display(Name = "Số Điện Thoại")]
        [RegularExpression(@"^\d{10}$", ErrorMessage = "Số điện thoại phải bao gồm 10 chữ số.")]
        [UniquePhoneNumber]
        public string? Dienthoai { get; set; }

        [Required(ErrorMessage = "Mật khẩu là bắt buộc.")]
        [Display(Name = "Mật Khẩu")]
        [PasswordValidation]
        public string? Matkhau { get; set; }

        [Required(ErrorMessage = "Thông tin quyền admin là bắt buộc.")]
        [Display(Name = "Quyền Admin")]
        public bool? Quyenadmin { get; set; }

        [Required(ErrorMessage = "Email là bắt buộc.")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ.")]
        [UniqueEmail]
        [Display(Name = "Email")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Địa chỉ là bắt buộc.")]
        [StringLength(200, ErrorMessage = "Địa chỉ không được vượt quá 200 ký tự.")]
        [Display(Name = "Địa Chỉ")]
        public string? Diachi { get; set; }

        [Display(Name = "Ảnh")]
        public string? Anh { get; set; }

        [Required(ErrorMessage = "Bạn nên thêm công việc trước.")]
        [Display(Name = "Công Việc")]
        public decimal? Macv { get; set; }

        [ForeignKey("Macv")]
        public virtual Congviec? MacvNavigation { get; set; }

        public virtual ICollection<Hoadonban> Hoadonbans { get; set; }
        public virtual ICollection<Hoadonnhap> Hoadonnhaps { get; set; }
    }
}
