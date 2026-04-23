using System;
using System.ComponentModel.DataAnnotations;
using API_Sample.Models.Common;

namespace API_Sample.Models.Request;

public class MReq_CareAdjustment : BaseModel.History
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Nhân viên là bắt buộc")]
    public int EmployeeId { get; set; }

    [Required(ErrorMessage = "Tháng/năm là bắt buộc")]
    [StringLength(7)]
    public string YearMonth { get; set; }

    [Required(ErrorMessage = "Số ngày bổ công là bắt buộc")]
    public decimal CareDays { get; set; }

    public decimal CareAmount { get; set; }

    [Required(ErrorMessage = "Loại điều chỉnh là bắt buộc")]
    [StringLength(20)]
    public string AdjustmentType { get; set; } = "ADD";

    [StringLength(500)]
    public string? Reason { get; set; }

    public int? ApprovedBy { get; set; }
    public DateTime? ApprovedAt { get; set; }
    public string? ApprovalStatus { get; set; } = "PENDING";
}

public class MReq_CareAdjustment_FullParam : PagingRequestBase
{
    public int? EmployeeId { get; set; }
    public string? YearMonth { get; set; }
    public string? AdjustmentType { get; set; }
    public string? ApprovalStatus { get; set; }
    public int? TramId { get; set; }
}
