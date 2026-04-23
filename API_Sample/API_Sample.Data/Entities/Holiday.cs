using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API_Sample.Data.Entities;

/// <summary>
/// Ngày lễ trong năm (VN + Lào)
/// </summary>
[Table("holiday")]
public partial class Holiday : BaseEntity
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    /// <summary>
    /// Năm: 2025, 2026...
    /// </summary>
    [Column("year")]
    public int Year { get; set; }

    /// <summary>
    /// Ngày lễ cụ thể
    /// </summary>
    [Column("holiday_date", TypeName = "date")]
    public DateTime HolidayDate { get; set; }

    /// <summary>
    /// Tên ngày lễ: Tết Nguyên đán, Quốc khánh Lào...
    /// </summary>
    [Required]
    [Column("name")]
    [StringLength(200)]
    public string Name { get; set; }

    /// <summary>
    /// Quốc gia: VN, LA, BOTH
    /// </summary>
    [Required]
    [Column("country")]
    [StringLength(10)]
    public string Country { get; set; }

    /// <summary>
    /// Có trả lương ngày nghỉ không
    /// </summary>
    [Column("is_paid")]
    public bool IsPaid { get; set; } = true;

    /// <summary>
    /// Hệ số thưởng nếu đi làm (VD: 2.0 = gấp đôi)
    /// </summary>
    [Column("bonus_rate", TypeName = "decimal(5,2)")]
    public decimal? BonusRate { get; set; }

    [Column("description")]
    [StringLength(500)]
    public string? Description { get; set; }
}
