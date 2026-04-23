namespace API_Sample.Utilities.Constants
{
    public class MessageErrorConstants
    {
        public const string DO_NOT_FIND_DATA = "Không tìm thấy dữ liệu!";
        public const string REQUEST_DATA_INVALID = "Dữ liệu đầu vào không hợp lệ!";
        public const string ACCESS_DENIED = "Không có quyền thao tác!";
        public const string EXCEPTION_DO_NOT_CREATE = "Xảy ra lỗi trong quá trình thêm dữ liệu!";
        public const string EXCEPTION_DO_NOT_UPDATE = "Xảy ra lỗi trong quá trình cập nhật dữ liệu!";
        public const string EXCEPTION_DO_NOT_DELETE = "Xảy ra lỗi trong quá trình xóa dữ liệu!";
        public const string EXCEPTION_DO_NOT_ACCESS_DATA = "Xảy ra lỗi trong quá trình truy cập dữ liệu. Thử lại sau!";

        public const string CREATE_SUCCESS = "Thêm dữ liệu thành công!";
        public const string UPDATE_SUCCESS = "Cập nhật dữ liệu thành công!";
        public const string DELETE_SUCCESS = "Xóa dữ liệu thành công!";

        // Payroll
        public const string PAYROLL_CALCULATE_SUCCESS = "Tính lương thành công!";
        public const string PAYROLL_ALREADY_LOCKED = "Bảng lương đã khóa, không thể thay đổi!";
        public const string PAYROLL_NO_PRODUCTION = "Không có sản lượng trong tháng!";
        public const string PAYROLL_NO_UNIT_PRICE = "Không tìm thấy đơn giá!";
    }
}
