using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API_Sample.Data.Entities;

/// <summary>
/// Mã kế toán (hệ thống tài khoản kế toán cho phân bổ chi phí lương)
/// </summary>
[Table("accounting_code")]
public partial class AccountingCode : BaseEntity
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    /// <summary>
    /// Mã tài khoản: 334, 3341, 641, 642...
    /// </summary>
    [Required]
    [Column("code")]
    [StringLength(20)]
    public string Code { get; set; }

    /// <summary>
    /// Tên tài khoản
    /// </summary>
    [Required]
    [Column("name")]
    [StringLength(200)]
    public string Name { get; set; }

    /// <summary>
    /// Loại: EXPENSE (chi phí), LIABILITY (công nợ), ASSET (tài sản)
    /// </summary>
    [Required]
    [Column("account_type")]
    [StringLength(20)]
    public string AccountType { get; set; }

    /// <summary>
    /// Mã cha (cho cấu trúc cây)
    /// </summary>
    [Column("parent_id")]
    public int? ParentId { get; set; }

    /// <summary>
    /// Trung tâm chi phí liên kết
    /// </summary>
    [Column("cost_center_id")]
    public int? CostCenterId { get; set; }

    /// <summary>
    /// Cấp độ trong cây (1=gốc, 2=con cấp 1...)
    /// </summary>
    [Column("level")]
    public int Level { get; set; } = 1;

    /// <summary>
    /// Có phải tài khoản chi tiết (lá) không
    /// </summary>
    [Column("is_detail")]
    public bool IsDetail { get; set; } = true;

    /// <summary>
    /// Mô tả
    /// </summary>
    [Column("description")]
    [StringLength(500)]
    public string? Description { get; set; }

    [ForeignKey("ParentId")]
    public virtual AccountingCode? Parent { get; set; }

    [ForeignKey("CostCenterId")]
    public virtual CostCenter? CostCenter { get; set; }

    public virtual ICollection<AccountingCode> Children { get; set; } = new List<AccountingCode>();
}
