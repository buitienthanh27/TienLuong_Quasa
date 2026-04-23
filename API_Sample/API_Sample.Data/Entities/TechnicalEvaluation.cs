using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API_Sample.Data.Entities;

/// <summary>
/// Đánh giá hạng kỹ thuật hàng tháng (QLKT đánh giá + phúc tra)
/// </summary>
[Table("technical_evaluation")]
public partial class TechnicalEvaluation : BaseEntity
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    /// <summary>
    /// Nhân viên
    /// </summary>
    [Column("employee_id")]
    public int EmployeeId { get; set; }

    /// <summary>
    /// Tháng/năm: "2025-11"
    /// </summary>
    [Required]
    [Column("year_month")]
    [StringLength(7)]
    public string YearMonth { get; set; }

    /// <summary>
    /// Hạng đánh giá lần đầu: A/B/C/D
    /// </summary>
    [Required]
    [Column("evaluated_grade")]
    [StringLength(5)]
    public string EvaluatedGrade { get; set; }

    /// <summary>
    /// Điểm đánh giá
    /// </summary>
    [Column("evaluation_score", TypeName = "decimal(5,2)")]
    public decimal? EvaluationScore { get; set; }

    /// <summary>
    /// QLKT đánh giá
    /// </summary>
    [Column("evaluated_by")]
    public int EvaluatedBy { get; set; }

    [Column("evaluated_at", TypeName = "datetime")]
    public DateTime EvaluatedAt { get; set; }

    /// <summary>
    /// Đã phúc tra chưa
    /// </summary>
    [Column("is_reviewed")]
    public bool IsReviewed { get; set; } = false;

    /// <summary>
    /// Hạng sau phúc tra
    /// </summary>
    [Column("reviewed_grade")]
    [StringLength(5)]
    public string? ReviewedGrade { get; set; }

    /// <summary>
    /// Điểm sau phúc tra
    /// </summary>
    [Column("reviewed_score", TypeName = "decimal(5,2)")]
    public decimal? ReviewedScore { get; set; }

    /// <summary>
    /// Người phúc tra
    /// </summary>
    [Column("reviewed_by")]
    public int? ReviewedBy { get; set; }

    [Column("reviewed_at", TypeName = "datetime")]
    public DateTime? ReviewedAt { get; set; }

    /// <summary>
    /// Hạng cuối cùng (= reviewed nếu có, else evaluated)
    /// </summary>
    [Required]
    [Column("final_grade")]
    [StringLength(5)]
    public string FinalGrade { get; set; }

    [Column("note")]
    [StringLength(1000)]
    public string? Note { get; set; }

    [ForeignKey("EmployeeId")]
    public virtual Employee Employee { get; set; }
}
