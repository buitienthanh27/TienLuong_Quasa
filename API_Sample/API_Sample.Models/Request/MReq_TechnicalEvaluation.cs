using System;
using System.ComponentModel.DataAnnotations;
using API_Sample.Models.Common;

namespace API_Sample.Models.Request;

public class MReq_TechnicalEvaluation : BaseModel.History
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Nhân viên là bắt buộc")]
    public int EmployeeId { get; set; }

    [Required(ErrorMessage = "Tháng/năm là bắt buộc")]
    [StringLength(7)]
    public string YearMonth { get; set; }

    [Required(ErrorMessage = "Hạng đánh giá là bắt buộc")]
    [StringLength(5)]
    public string EvaluatedGrade { get; set; }

    public decimal? EvaluationScore { get; set; }

    [Required]
    public int EvaluatedBy { get; set; }

    public DateTime EvaluatedAt { get; set; } = DateTime.UtcNow;

    public bool IsReviewed { get; set; } = false;

    [StringLength(5)]
    public string? ReviewedGrade { get; set; }

    public decimal? ReviewedScore { get; set; }

    public int? ReviewedBy { get; set; }
    public DateTime? ReviewedAt { get; set; }

    [Required]
    [StringLength(5)]
    public string FinalGrade { get; set; }

    [StringLength(1000)]
    public string? Note { get; set; }
}

public class MReq_TechnicalEvaluation_FullParam : PagingRequestBase
{
    public int? EmployeeId { get; set; }
    public string? YearMonth { get; set; }
    public string? FinalGrade { get; set; }
    public bool? IsReviewed { get; set; }
    public int? TramId { get; set; }
}
