namespace Oracle_WEB_BTL.Helpers
{
    public static class AppDefaults
    {
        public const string DefaultImageFile = "user_default.png";
        public const string DefaultSQLFile = "Data\\QLBG_data_2_oracle.sql";
        public const string DefaultProductImageFile = "1.png";
        public const string DefaultImageFolder = "wwwroot/images";
        public const string DefaultProductImageFolder = "wwwroot/images/ProductImages";

        public const string CannotDeleteSelfMessage = "Bạn không thể xóa tài khoản của chính mình.";
        public const string EmailNotFoundMessage = "Email không tồn tại trong hệ thống.";
        public const string InvalidLoginMessage = "Thông tin đăng nhập không chính xác.";
        public const string PasswordUpdatedMessage = "Mật khẩu đã được cập nhật thành công.";
        public const string InvalidOtpMessage = "Mã OTP không hợp lệ.";
        public const string ResetPasswordOtpMessage = "Mã OTP Đặt Lại Mật Khẩu";

        public const int TimeOut = 20;

        public static string GetOtpMessage(string otpCode)
        {
            return $"Mã OTP của bạn là: {otpCode}";
        }
    }
}
