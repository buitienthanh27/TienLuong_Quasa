using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API_Sample.Data.Entities;

/// <summary>
/// Hỗ trợ vùng khó khăn / vùng sâu vùng xa
/// </summary>
[Table("zone_support")]
public partial class ZoneSupport : BaseEntity
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    /// <summary>
    /// Trạm được hỗ trợ
    /// </summary>
    [Column("tram_id")]
    public int TramId { get; set; }

    /// <summary>
    /// Loại hỗ trợ: DIFFICULT_AREA (vùng khó khăn), REMOTE (vùng sâu/xa)
    /// </summary>
    [Required]
    [Column("support_type")]
    [StringLength(30)]
    public string SupportType { get; set; }

    /// <summary>
    /// Số tiền hỗ trợ cố định
    /// </summary>
    [Column("support_amount", TypeName = "decimal(18,4)")]
    public decimal SupportAmount { get; set; }

    /// <summary>
    /// Hệ số cộng thêm vào đơn giá (VD: 0.1 = +10%)
    /// </summary>
    [Column("support_rate", TypeName = "decimal(5,4)")]
    public decimal? SupportRate { get; set; }

    /// <summary>
    /// Ngày bắt đầu áp dụng
    /// </summary>
    [Column("effective_date", TypeName = "date")]
    public DateTime EffectiveDate { get; set; }

    /// <summary>
    /// Ngày kết thúc (null = vô thời hạn)
    /// </summary>
    [Column("end_date", TypeName = "date")]
    public DateTime? EndDate { get; set; }

    /// <summary>
    /// Mô tả chi tiết
    /// </summary>
    [Column("description")]
    [StringLength(500)]
    public string? Description { get; set; }

    [ForeignKey("TramId")]
    public virtual Tram Tram { get; set; }
}
