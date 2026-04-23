using API_Sample.Models.Common;
using System.ComponentModel.DataAnnotations;

namespace API_Sample.Models.Request
{
    public class MReq_Attendance : BaseModel.History
    {
        public int Id { get; set; }

        [Required]
        public int EmployeeId { get; set; }

        [Required]
        [StringLength(7)]
        public string YearMonth { get; set; }

        /// <summary>Công thường - Hệ số 46.1 × DRC</summary>
        public decimal RegularDays { get; set; }

        /// <summary>Công chủ nhật - Hệ số 76.9 × DRC</summary>
        public decimal SundayDays { get; set; }

        /// <summary>Công cây non - Hệ số 30.7 × DRC</summary>
        public decimal YoungTreeDays { get; set; }

        /// <summary>Công khộp nặng - +20.000 kíp/công</summary>
        public decimal HardshipDays { get; set; }

        /// <summary>Cạo 2 lát thường - +100.000 kíp/công</summary>
        public decimal DoubleCutDays { get; set; }

        /// <summary>Cạo 2 lát chủ nhật</summary>
        public decimal DoubleCutSunday { get; set; }

        /// <summary>Công chăm sóc - 25.000 kíp/công</summary>
        public decimal CareDays { get; set; }

        /// <summary>Ngày vắng không lương</summary>
        public decimal AbsentDays { get; set; }

        /// <summary>Ngày nghỉ ốm</summary>
        public decimal SickDays { get; set; }

        /// <summary>Ngày nghỉ phép</summary>
        public decimal LeaveDays { get; set; }

        /// <summary>Tổng công</summary>
        public decimal TotalDays { get; set; }

        [StringLength(20)]
        public string AttendanceStatus { get; set; } = "DRAFT";
    }

    public class MReq_Attendance_FullParam : PagingRequestBase
    {
        public string SequenceStatus { get; set; }
        public int? EmployeeId { get; set; }
        public int? TramId { get; set; }
        public string YearMonth { get; set; }
        public string AttendanceStatus { get; set; }
    }

    public class MReq_Attendance_BulkImport
    {
        [Required]
        public string YearMonth { get; set; }
        public List<MReq_Attendance_Item> Items { get; set; }
        public int CreatedBy { get; set; }
    }

    public class MReq_Attendance_Item
    {
        public int EmployeeId { get; set; }
        public decimal RegularDays { get; set; }
        public decimal SundayDays { get; set; }
        public decimal YoungTreeDays { get; set; }
        public decimal HardshipDays { get; set; }
        public decimal DoubleCutDays { get; set; }
        public decimal DoubleCutSunday { get; set; }
        public decimal CareDays { get; set; }
        public decimal AbsentDays { get; set; }
        public decimal SickDays { get; set; }
        public decimal LeaveDays { get; set; }
    }
}
