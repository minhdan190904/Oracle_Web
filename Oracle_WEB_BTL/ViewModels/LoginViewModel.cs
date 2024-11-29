using System.ComponentModel.DataAnnotations;

namespace Oracle_WEB_BTL.ViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Email không được để trống.")]
        [EmailAddress(ErrorMessage = "Định dạng email không hợp lệ.")]
        public string Email { get; set; } // username unique

        [Required(ErrorMessage = "Mật khẩu không được để trống.")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        public bool KeepLoggedIn { get; set; }
    }
}
