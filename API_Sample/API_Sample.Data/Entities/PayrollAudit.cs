using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API_Sample.Data.Entities;

/// <summary>
/// Audit trail cho bảng lương (theo dõi tính toán, phê duyệt, khóa/mở khóa)
/// </summary>
[Table("payroll_audit")]
public partial class PayrollAudit
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    /// <summary>
    /// Bảng lương được audit
    /// </summary>
    [Column("payroll_id")]
    public int PayrollId { get; set; }

    /// <summary>
    /// Hành động: CALCULATE, RECALCULATE, APPROVE, REJECT, LOCK, UNLOCK
    /// </summary>
    [Required]
    [Column("audit_action")]
    [StringLength(30)]
    public string AuditAction { get; set; }

    /// <summary>
    /// Dữ liệu audit (JSON snapshot trước/sau)
    /// </summary>
    [Column("audit_data", TypeName = "nvarchar(max)")]
    public string? AuditData { get; set; }

    /// <summary>
    /// Lý do (VD: "Tính lại do sai ngày công")
    /// </summary>
    [Column("reason")]
    [StringLength(500)]
    public string? Reason { get; set; }

    /// <summary>
    /// Người thực hiện
    /// </summary>
    [Column("audited_by")]
    public int AuditedBy { get; set; }

    /// <summary>
    /// Thời gian thực hiện
    /// </summary>
    [Column("audited_at", TypeName = "datetime")]
    public DateTime AuditedAt { get; set; }

    /// <summary>
    /// IP address người thực hiện
    /// </summary>
    [Column("ip_address")]
    [StringLength(50)]
    public string? IpAddress { get; set; }

    [ForeignKey("PayrollId")]
    public virtual Payroll Payroll { get; set; }
}
