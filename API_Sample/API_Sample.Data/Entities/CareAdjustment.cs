using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API_Sample.Data.Entities;

/// <summary>
/// Bổ công chăm sóc (cộng/trừ ngày công chăm sóc cây cao su)
/// </summary>
[Table("care_adjustment")]
public partial class CareAdjustment : BaseEntity
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    /// <summary>
    /// Nhân viên được bổ công
    /// </summary>
    [Column("employee_id")]
    public int EmployeeId { get; set; }

    /// <summary>
    /// Tháng/năm: "2025-11"
    /// </summary>
    [Required]
    [Column("year_month")]
    [StringLength(7)]
    public string YearMonth { get; set; }

    /// <summary>
    /// Số ngày bổ công chăm sóc
    /// </summary>
    [Column("care_days", TypeName = "decimal(5,2)")]
    public decimal CareDays { get; set; }

    /// <summary>
    /// Số tiền bổ công
    /// </summary>
    [Column("care_amount", TypeName = "decimal(18,4)")]
    public decimal CareAmount { get; set; }

    /// <summary>
    /// Loại: ADD (bổ sung), SUBTRACT (trừ)
    /// </summary>
    [Required]
    [Column("adjustment_type")]
    [StringLength(20)]
    public string AdjustmentType { get; set; }

    /// <summary>
    /// Lý do bổ công/trừ công
    /// </summary>
    [Column("reason")]
    [StringLength(500)]
    public string? Reason { get; set; }

    /// <summary>
    /// Người duyệt bổ công
    /// </summary>
    [Column("approved_by")]
    public int? ApprovedBy { get; set; }

    /// <summary>
    /// Thời gian duyệt
    /// </summary>
    [Column("approved_at", TypeName = "datetime")]
    public DateTime? ApprovedAt { get; set; }

    /// <summary>
    /// Trạng thái duyệt: PENDING, APPROVED, REJECTED
    /// </summary>
    [Column("approval_status")]
    [StringLength(20)]
    public string? ApprovalStatus { get; set; } = "PENDING";

    [ForeignKey("EmployeeId")]
    public virtual Employee Employee { get; set; }
}
