using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API_Sample.Data.Entities;

/// <summary>
/// Tạm ứng & Truy thu (tiền mặt ngày lễ, tạm ứng lương...)
/// </summary>
[Table("advance_payment")]
public partial class AdvancePayment : BaseEntity
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    /// <summary>
    /// Nhân viên
    /// </summary>
    [Column("employee_id")]
    public int EmployeeId { get; set; }

    /// <summary>
    /// Tháng/năm phát sinh: "2025-11"
    /// </summary>
    [Required]
    [Column("year_month")]
    [StringLength(7)]
    public string YearMonth { get; set; }

    /// <summary>
    /// Loại: ADVANCE (tạm ứng), RECOVERY (truy thu), HOLIDAY_PAY (tiền lễ)
    /// </summary>
    [Required]
    [Column("payment_type")]
    [StringLength(20)]
    public string PaymentType { get; set; }

    /// <summary>
    /// Số tiền
    /// </summary>
    [Column("amount", TypeName = "decimal(18,4)")]
    public decimal Amount { get; set; }

    /// <summary>
    /// Đơn vị tiền tệ: LAK, THB
    /// </summary>
    [Required]
    [Column("currency")]
    [StringLength(10)]
    public string Currency { get; set; }

    /// <summary>
    /// Ngày chi tiền mặt
    /// </summary>
    [Column("payment_date", TypeName = "date")]
    public DateTime? PaymentDate { get; set; }

    /// <summary>
    /// Lý do (VD: Tiền công ngày lễ 01/01)
    /// </summary>
    [Column("reason")]
    [StringLength(500)]
    public string? Reason { get; set; }

    /// <summary>
    /// Đã trừ vào lương chưa
    /// </summary>
    [Column("is_deducted")]
    public bool IsDeducted { get; set; } = false;

    /// <summary>
    /// Trừ vào tháng nào
    /// </summary>
    [Column("deducted_in_month")]
    [StringLength(7)]
    public string? DeductedInMonth { get; set; }

    /// <summary>
    /// Trừ vào bảng lương nào (FK payroll.id)
    /// </summary>
    [Column("deducted_in_payroll_id")]
    public int? DeductedInPayrollId { get; set; }

    /// <summary>
    /// Thời gian khấu trừ
    /// </summary>
    [Column("deducted_at", TypeName = "datetime")]
    public DateTime? DeductedAt { get; set; }

    /// <summary>
    /// Người duyệt
    /// </summary>
    [Column("approved_by")]
    public int? ApprovedBy { get; set; }

    [Column("approved_at", TypeName = "datetime")]
    public DateTime? ApprovedAt { get; set; }

    [ForeignKey("EmployeeId")]
    public virtual Employee Employee { get; set; }
}
