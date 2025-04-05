namespace lolapdp.Models
{
    /// <summary>
    /// Lớp ErrorViewModel đại diện cho thông tin lỗi trong hệ thống
    /// </summary>
    public class ErrorViewModel
    {
        /// <summary>
        /// ID của yêu cầu gây ra lỗi
        /// </summary>
        public string? RequestId { get; set; }

        /// <summary>
        /// Kiểm tra xem có ID yêu cầu để hiển thị hay không
        /// </summary>
        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }
}
//dbl