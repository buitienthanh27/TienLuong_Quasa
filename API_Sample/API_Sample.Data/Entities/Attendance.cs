using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API_Sample.Data.Entities;

/// <summary>
/// Bảng chấm công - Hệ thống lương nông trường cao su
/// Ghi nhận các loại công: công thường, CN, cây non, khộp nặng, cạo 2 lát, chăm sóc
/// </summary>
[Table("attendance")]
public partial class Attendance : BaseEntity
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("employee_id")]
    public int EmployeeId { get; set; }

    [Required]
    [Column("year_month")]
    [StringLength(7)]
    public string YearMonth { get; set; }

    // === CÔNG KHAI THÁC ===

    /// <summary>Công thường (ngày làm việc bình thường) - Hệ số 46.1 × DRC</summary>
    [Column("regular_days", TypeName = "decimal(5,2)")]
    public decimal RegularDays { get; set; }

    /// <summary>Công chủ nhật - Hệ số 76.9 × DRC</summary>
    [Column("sunday_days", TypeName = "decimal(5,2)")]
    public decimal SundayDays { get; set; }

    /// <summary>Công cây non (chăm sóc cây chưa khai thác) - Hệ số 30.7 × DRC</summary>
    [Column("young_tree_days", TypeName = "decimal(5,2)")]
    public decimal YoungTreeDays { get; set; }

    /// <summary>Công khộp nặng (vùng khó khăn) - +20.000 kíp/công</summary>
    [Column("hardship_days", TypeName = "decimal(5,2)")]
    public decimal HardshipDays { get; set; }

    /// <summary>Cạo 2 lát ngày thường - +100.000 kíp/công</summary>
    [Column("double_cut_days", TypeName = "decimal(5,2)")]
    public decimal DoubleCutDays { get; set; }

    /// <summary>Cạo 2 lát chủ nhật</summary>
    [Column("double_cut_sunday", TypeName = "decimal(5,2)")]
    public decimal DoubleCutSunday { get; set; }

    // === CÔNG CHĂM SÓC ===

    /// <summary>Công chăm sóc vườn cây - 25.000 kíp/công</summary>
    [Column("care_days", TypeName = "decimal(5,2)")]
    public decimal CareDays { get; set; }

    // === VẮNG MẶT ===

    /// <summary>Ngày nghỉ không lương</summary>
    [Column("absent_days", TypeName = "decimal(5,2)")]
    public decimal AbsentDays { get; set; }

    /// <summary>Ngày nghỉ ốm</summary>
    [Column("sick_days", TypeName = "decimal(5,2)")]
    public decimal SickDays { get; set; }

    /// <summary>Ngày nghỉ phép</summary>
    [Column("leave_days", TypeName = "decimal(5,2)")]
    public decimal LeaveDays { get; set; }

    // === TỔNG HỢP ===

    /// <summary>Tổng công (tính toán) = Regular + Sunday + YoungTree + HardshipDays + DoubleCut + Care</summary>
    [Column("total_days", TypeName = "decimal(5,2)")]
    public decimal TotalDays { get; set; }

    /// <summary>Trạng thái: DRAFT, SUBMITTED, APPROVED, LOCKED</summary>
    [Column("attendance_status")]
    [StringLength(20)]
    public string AttendanceStatus { get; set; } = "DRAFT";

    [ForeignKey("EmployeeId")]
    public virtual Employee Employee { get; set; }
}
