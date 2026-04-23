using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API_Sample.Data.Entities;

/// <summary>
/// Loại công việc và đơn giá: Công chăm sóc, Công CN, Phụ cấp khó khăn...
/// </summary>
[Table("work_type")]
public partial class WorkType : BaseEntity
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    /// <summary>
    /// Mã loại công: CARE, SUNDAY, HARDSHIP, DOUBLE_CUT, YOUNG_TREE, HOLIDAY
    /// </summary>
    [Required]
    [Column("code")]
    [StringLength(50)]
    public string Code { get; set; }

    /// <summary>
    /// Tên loại công: Công chăm sóc vườn, Công chủ nhật...
    /// </summary>
    [Required]
    [Column("name")]
    [StringLength(200)]
    public string Name { get; set; }

    /// <summary>
    /// Đơn giá (VD: 25000 Kíp/công)
    /// </summary>
    [Column("unit_price", TypeName = "decimal(18,4)")]
    public decimal UnitPrice { get; set; }

    /// <summary>
    /// Đơn vị tiền tệ: LAK, THB
    /// </summary>
    [Required]
    [Column("currency")]
    [StringLength(10)]
    public string Currency { get; set; }

    /// <summary>
    /// Cách tính: PER_DAY (theo ngày), PER_DRC (theo DRC), MULTIPLIER (hệ số nhân)
    /// </summary>
    [Required]
    [Column("calculation_type")]
    [StringLength(50)]
    public string CalculationType { get; set; }

    /// <summary>
    /// Hệ số nhân (VD: CN = 1.67 lần, cạo 2 lát = 2.0)
    /// </summary>
    [Column("multiplier", TypeName = "decimal(5,2)")]
    public decimal? Multiplier { get; set; }

    /// <summary>
    /// Áp dụng cho loại NV: CNKT, BV, CS, ALL
    /// </summary>
    [Column("applies_to_type")]
    [StringLength(20)]
    public string? AppliesToType { get; set; }

    [Column("description")]
    [StringLength(500)]
    public string? Description { get; set; }

    /// <summary>
    /// Ngày hiệu lực
    /// </summary>
    [Column("effective_date", TypeName = "date")]
    public DateTime EffectiveDate { get; set; }
}
