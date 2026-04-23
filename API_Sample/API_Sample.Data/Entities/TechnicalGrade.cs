using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API_Sample.Data.Entities;

/// <summary>
/// Hạng kỹ thuật A/B/C/D và hệ số điểm (do Phòng KT quy định)
/// </summary>
[Table("technical_grade")]
public partial class TechnicalGrade : BaseEntity
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    /// <summary>
    /// Hạng: A, B, C, D
    /// </summary>
    [Required]
    [Column("grade")]
    [StringLength(5)]
    public string Grade { get; set; }

    /// <summary>
    /// Tên hạng: Hạng A - Xuất sắc
    /// </summary>
    [Required]
    [Column("name")]
    [StringLength(100)]
    public string Name { get; set; }

    /// <summary>
    /// Hệ số tính điểm (do Phòng KT quy định)
    /// </summary>
    [Column("point_coefficient", TypeName = "decimal(5,2)")]
    public decimal PointCoefficient { get; set; }

    /// <summary>
    /// Điểm tối thiểu để đạt hạng
    /// </summary>
    [Column("min_score", TypeName = "decimal(5,2)")]
    public decimal? MinScore { get; set; }

    /// <summary>
    /// Điểm tối đa
    /// </summary>
    [Column("max_score", TypeName = "decimal(5,2)")]
    public decimal? MaxScore { get; set; }

    [Column("description")]
    [StringLength(500)]
    public string? Description { get; set; }

    /// <summary>
    /// Ngày hiệu lực
    /// </summary>
    [Column("effective_date", TypeName = "date")]
    public DateTime EffectiveDate { get; set; }

    /// <summary>
    /// Thứ tự sắp xếp
    /// </summary>
    [Column("sort_order")]
    public int SortOrder { get; set; } = 0;
}
