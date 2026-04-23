using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API_Sample.Data.Entities;

/// <summary>
/// Tỷ giá hàng tháng (nguồn Vietinbank, P.Tổ chức xác nhận)
/// </summary>
[Table("exchange_rate")]
public partial class ExchangeRate : BaseEntity
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
    /// Đơn vị tiền nguồn: THB, USD
    /// </summary>
    [Required]
    [Column("from_currency")]
    [StringLength(10)]
    public string FromCurrency { get; set; }

    /// <summary>
    /// Đơn vị tiền đích: LAK
    /// </summary>
    [Required]
    [Column("to_currency")]
    [StringLength(10)]
    public string ToCurrency { get; set; }

    /// <summary>
    /// Tỷ giá (VD: 1 THB = 611.23 LAK)
    /// </summary>
    [Column("rate", TypeName = "decimal(18,6)")]
    public decimal Rate { get; set; }

    /// <summary>
    /// Nguồn tỷ giá: Vietinbank
    /// </summary>
    [Column("source")]
    [StringLength(100)]
    public string? Source { get; set; }

    /// <summary>
    /// Người xác nhận (P.Tổ chức)
    /// </summary>
    [Column("approved_by")]
    public int? ApprovedBy { get; set; }

    [Column("approved_at", TypeName = "datetime")]
    public DateTime? ApprovedAt { get; set; }

    [Column("description")]
    [StringLength(500)]
    public string? Description { get; set; }
}
