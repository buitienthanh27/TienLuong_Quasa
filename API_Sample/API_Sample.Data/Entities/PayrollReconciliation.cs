using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API_Sample.Data.Entities;

/// <summary>
/// Cân đối và chốt quỹ lương theo tháng
/// </summary>
[Table("payroll_reconciliation")]
public partial class PayrollReconciliation : BaseEntity
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    /// <summary>
    /// Tháng/năm: "2025-11"
    /// </summary>
    [Required]
    [Column("year_month")]
    [StringLength(7)]
    public string YearMonth { get; set; }

    /// <summary>
    /// Trạm (null = toàn công ty)
    /// </summary>
    [Column("tram_id")]
    public int? TramId { get; set; }

    /// <summary>
    /// Tổng số nhân viên trong kỳ
    /// </summary>
    [Column("total_employees")]
    public int TotalEmployees { get; set; }

    /// <summary>
    /// Tổng lương gộp (Gross)
    /// </summary>
    [Column("total_gross_salary", TypeName = "decimal(18,4)")]
    public decimal TotalGrossSalary { get; set; }

    /// <summary>
    /// Tổng các khoản khấu trừ
    /// </summary>
    [Column("total_deductions", TypeName = "decimal(18,4)")]
    public decimal TotalDeductions { get; set; }

    /// <summary>
    /// Tổng lương thực lĩnh (Net)
    /// </summary>
    [Column("total_net_salary", TypeName = "decimal(18,4)")]
    public decimal TotalNetSalary { get; set; }

    /// <summary>
    /// Tổng BHXH (người lao động)
    /// </summary>
    [Column("total_bhxh", TypeName = "decimal(18,4)")]
    public decimal TotalBhxh { get; set; }

    /// <summary>
    /// Tổng BHYT (người lao động)
    /// </summary>
    [Column("total_bhyt", TypeName = "decimal(18,4)")]
    public decimal TotalBhyt { get; set; }

    /// <summary>
    /// Tổng BHXH (doanh nghiệp đóng)
    /// </summary>
    [Column("total_bhxh_company", TypeName = "decimal(18,4)")]
    public decimal TotalBhxhCompany { get; set; }

    /// <summary>
    /// Tổng BHYT (doanh nghiệp đóng)
    /// </summary>
    [Column("total_bhyt_company", TypeName = "decimal(18,4)")]
    public decimal TotalBhytCompany { get; set; }

    /// <summary>
    /// Tổng thuế TNCN
    /// </summary>
    [Column("total_tax", TypeName = "decimal(18,4)")]
    public decimal TotalTax { get; set; }

    /// <summary>
    /// Tổng phụ cấp
    /// </summary>
    [Column("total_allowances", TypeName = "decimal(18,4)")]
    public decimal TotalAllowances { get; set; }

    /// <summary>
    /// Trạng thái: DRAFT, BALANCED, LOCKED
    /// </summary>
    [Required]
    [Column("reconciliation_status")]
    [StringLength(20)]
    public string ReconciliationStatus { get; set; } = "DRAFT";

    /// <summary>
    /// Người cân đối
    /// </summary>
    [Column("balanced_by")]
    public int? BalancedBy { get; set; }

    /// <summary>
    /// Thời gian cân đối
    /// </summary>
    [Column("balanced_at", TypeName = "datetime")]
    public DateTime? BalancedAt { get; set; }

    /// <summary>
    /// Người chốt (khóa)
    /// </summary>
    [Column("locked_by")]
    public int? LockedBy { get; set; }

    /// <summary>
    /// Thời gian chốt
    /// </summary>
    [Column("locked_at", TypeName = "datetime")]
    public DateTime? LockedAt { get; set; }

    /// <summary>
    /// Ghi chú cân đối
    /// </summary>
    [Column("notes")]
    [StringLength(1000)]
    public string? Notes { get; set; }

    [ForeignKey("TramId")]
    public virtual Tram? Tram { get; set; }
}
