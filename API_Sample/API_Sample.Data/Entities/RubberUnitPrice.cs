using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API_Sample.Data.Entities;

/// <summary>
/// Đơn giá mủ theo hạng kỹ thuật và trạm (Bath/kg)
/// Thường 1 năm đổi 1 lần
/// </summary>
[Table("rubber_unit_price")]
public partial class RubberUnitPrice : BaseEntity
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    /// <summary>
    /// Trạm
    /// </summary>
    [Column("tram_id")]
    public int TramId { get; set; }

    /// <summary>
    /// Hạng kỹ thuật: A, B, C, D
    /// </summary>
    [Required]
    [Column("grade")]
    [StringLength(5)]
    public string Grade { get; set; }

    /// <summary>
    /// Đơn giá (Bath/kg)
    /// </summary>
    [Column("unit_price", TypeName = "decimal(18,4)")]
    public decimal UnitPrice { get; set; }

    /// <summary>
    /// Đơn vị tiền tệ (mặc định THB)
    /// </summary>
    [Required]
    [Column("currency")]
    [StringLength(10)]
    public string Currency { get; set; } = "THB";

    /// <summary>
    /// Hỗ trợ thêm vùng khó khăn (Bath/kg)
    /// </summary>
    [Column("difficult_area_bonus", TypeName = "decimal(18,4)")]
    public decimal? DifficultAreaBonus { get; set; }

    [Column("description")]
    [StringLength(500)]
    public string? Description { get; set; }

    /// <summary>
    /// Ngày hiệu lực (~1 năm đổi 1 lần)
    /// </summary>
    [Column("effective_date", TypeName = "date")]
    public DateTime EffectiveDate { get; set; }

    [ForeignKey("TramId")]
    public virtual Tram Tram { get; set; }
}
