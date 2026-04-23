using System;
using System.ComponentModel.DataAnnotations;
using API_Sample.Models.Common;

namespace API_Sample.Models.Request;

public class MReq_EmployeeHistory : BaseModel.History
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Nhân viên là bắt buộc")]
    public int EmployeeId { get; set; }

    [Required(ErrorMessage = "Loại thay đổi là bắt buộc")]
    [StringLength(30)]
    public string ChangeType { get; set; }

    [StringLength(500)]
    public string? OldValue { get; set; }

    [StringLength(500)]
    public string? NewValue { get; set; }

    [Required(ErrorMessage = "Ngày thay đổi là bắt buộc")]
    public DateTime ChangeDate { get; set; }

    public int ChangedBy { get; set; }

    [StringLength(500)]
    public string? Reason { get; set; }

    [StringLength(50)]
    public string? DecisionNumber { get; set; }

    public DateTime? DecisionDate { get; set; }
}

public class MReq_EmployeeHistory_FullParam : PagingRequestBase
{
    public int? EmployeeId { get; set; }
    public string? ChangeType { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
    public int? TramId { get; set; }
}
