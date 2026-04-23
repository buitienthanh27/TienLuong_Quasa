using API_Sample.Models.Common;

namespace API_Sample.Models.Response
{
    public class MRes_Attendance : BaseModel.History
    {
        public int Id { get; set; }
        public int EmployeeId { get; set; }
        public string YearMonth { get; set; }

        /// <summary>Công thường</summary>
        public decimal RegularDays { get; set; }

        /// <summary>Công chủ nhật</summary>
        public decimal SundayDays { get; set; }

        /// <summary>Công cây non</summary>
        public decimal YoungTreeDays { get; set; }

        /// <summary>Công khộp nặng</summary>
        public decimal HardshipDays { get; set; }

        /// <summary>Cạo 2 lát thường</summary>
        public decimal DoubleCutDays { get; set; }

        /// <summary>Cạo 2 lát CN</summary>
        public decimal DoubleCutSunday { get; set; }

        /// <summary>Công chăm sóc</summary>
        public decimal CareDays { get; set; }

        /// <summary>Ngày vắng</summary>
        public decimal AbsentDays { get; set; }

        /// <summary>Ngày ốm</summary>
        public decimal SickDays { get; set; }

        /// <summary>Ngày phép</summary>
        public decimal LeaveDays { get; set; }

        /// <summary>Tổng công</summary>
        public decimal TotalDays { get; set; }

        public string AttendanceStatus { get; set; }

        public string EmployeeMsnv { get; set; }
        public string EmployeeName { get; set; }
        public int? TramId { get; set; }
        public string TramCode { get; set; }
        public string TramName { get; set; }
    }
}
