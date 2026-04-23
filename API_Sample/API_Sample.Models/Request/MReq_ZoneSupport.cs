using System;
using System.ComponentModel.DataAnnotations;
using API_Sample.Models.Common;

namespace API_Sample.Models.Request;

public class MReq_ZoneSupport : BaseModel.History
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Trạm là bắt buộc")]
    public int TramId { get; set; }

    [Required(ErrorMessage = "Loại hỗ trợ là bắt buộc")]
    [StringLength(30)]
    public string SupportType { get; set; }

    [Required(ErrorMessage = "Số tiền hỗ trợ là bắt buộc")]
    public decimal SupportAmount { get; set; }

    public decimal? SupportRate { get; set; }

    [Required(ErrorMessage = "Ngày áp dụng là bắt buộc")]
    public DateTime EffectiveDate { get; set; }

    public DateTime? EndDate { get; set; }

    [StringLength(500)]
    public string? Description { get; set; }
}

public class MReq_ZoneSupport_FullParam : PagingRequestBase
{
    public int? TramId { get; set; }
    public string? SupportType { get; set; }
    public DateTime? EffectiveDate { get; set; }
    public bool? IsActive { get; set; }
}
