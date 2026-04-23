using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API_Sample.Data.Entities;

/// <summary>
/// Biểu thuế TNCN theo bậc lũy tiến
/// </summary>
[Table("tax_bracket")]
public partial class TaxBracket : BaseEntity
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    /// <summary>
    /// Ngưỡng thu nhập từ (KIP)
    /// </summary>
    [Column("from_amount", TypeName = "decimal(18,2)")]
    public decimal FromAmount { get; set; }

    /// <summary>
    /// Ngưỡng thu nhập đến (KIP). NULL = bậc cuối không giới hạn
    /// </summary>
    [Column("to_amount", TypeName = "decimal(18,2)")]
    public decimal? ToAmount { get; set; }

    /// <summary>
    /// Thuế suất (VD: 0.05 = 5%)
    /// </summary>
    [Column("tax_rate", TypeName = "decimal(5,4)")]
    public decimal TaxRate { get; set; }

    /// <summary>
    /// Số tiền cộng thêm theo công thức thuế lũy tiến
    /// </summary>
    [Column("additional_amount", TypeName = "decimal(18,2)")]
    public decimal AdditionalAmount { get; set; }

    /// <summary>
    /// Ngày hiệu lực
    /// </summary>
    [Column("effective_date", TypeName = "date")]
    public DateTime EffectiveDate { get; set; }

    /// <summary>
    /// Ngày kết thúc hiệu lực. NULL = không giới hạn
    /// </summary>
    [Column("end_date", TypeName = "date")]
    public DateTime? EndDate { get; set; }

    /// <summary>
    /// Thứ tự sắp xếp bậc thuế
    /// </summary>
    [Column("sort_order")]
    public int SortOrder { get; set; }

    [Column("description")]
    [StringLength(500)]
    public string? Description { get; set; }
}
