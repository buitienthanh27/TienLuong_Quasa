using System;

namespace API_Sample.Models.Response;

public class MRes_TechnicalEvaluation
{
    public int Id { get; set; }
    public int EmployeeId { get; set; }
    public string EmployeeMsnv { get; set; }
    public string EmployeeName { get; set; }
    public string TramCode { get; set; }
    public string YearMonth { get; set; }
    public string EvaluatedGrade { get; set; }
    public decimal? EvaluationScore { get; set; }
    public int EvaluatedBy { get; set; }
    public DateTime EvaluatedAt { get; set; }
    public bool IsReviewed { get; set; }
    public string? ReviewedGrade { get; set; }
    public decimal? ReviewedScore { get; set; }
    public int? ReviewedBy { get; set; }
    public DateTime? ReviewedAt { get; set; }
    public string FinalGrade { get; set; }
    public string? Note { get; set; }
    public short Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public int CreatedBy { get; set; }
}
