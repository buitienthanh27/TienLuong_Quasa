using System;
using System.ComponentModel.DataAnnotations;
using API_Sample.Models.Common;

namespace API_Sample.Models.Request;

public class MReq_AdvancePayment : BaseModel.History
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Nhân viên là bắt buộc")]
    public int EmployeeId { get; set; }

    [Required(ErrorMessage = "Tháng/năm là bắt buộc")]
    [StringLength(7)]
    public string YearMonth { get; set; }

    [Required(ErrorMessage = "Loại là bắt buộc")]
    [StringLength(20)]
    public string PaymentType { get; set; } = "ADVANCE";

    [Required(ErrorMessage = "Số tiền là bắt buộc")]
    [Range(0, double.MaxValue, ErrorMessage = "Số tiền phải >= 0")]
    public decimal Amount { get; set; }

    [Required]
    [StringLength(10)]
    public string Currency { get; set; } = "LAK";

    public DateTime? PaymentDate { get; set; }

    [StringLength(500)]
    public string? Reason { get; set; }

    public bool IsDeducted { get; set; } = false;

    [StringLength(7)]
    public string? DeductedInMonth { get; set; }

    public int? ApprovedBy { get; set; }
    public DateTime? ApprovedAt { get; set; }
}

public class MReq_AdvancePayment_FullParam : PagingRequestBase
{
    public int? EmployeeId { get; set; }
    public string? YearMonth { get; set; }
    public string? PaymentType { get; set; }
    public bool? IsDeducted { get; set; }
    public int? TramId { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
}
