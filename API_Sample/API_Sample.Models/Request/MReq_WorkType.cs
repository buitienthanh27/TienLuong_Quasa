using System;
using System.ComponentModel.DataAnnotations;
using API_Sample.Models.Common;

namespace API_Sample.Models.Request;

public class MReq_WorkType : BaseModel.History
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Mã loại công là bắt buộc")]
    [StringLength(50)]
    public string Code { get; set; }

    [Required(ErrorMessage = "Tên loại công là bắt buộc")]
    [StringLength(200)]
    public string Name { get; set; }

    [Range(0, double.MaxValue, ErrorMessage = "Đơn giá phải >= 0")]
    public decimal UnitPrice { get; set; }

    [Required]
    [StringLength(10)]
    public string Currency { get; set; } = "LAK";

    [Required]
    [StringLength(50)]
    public string CalculationType { get; set; } = "PER_DAY";

    public decimal? Multiplier { get; set; }

    [StringLength(20)]
    public string? AppliesToType { get; set; }

    [StringLength(500)]
    public string? Description { get; set; }

    [Required(ErrorMessage = "Ngày hiệu lực là bắt buộc")]
    public DateTime EffectiveDate { get; set; }
}

public class MReq_WorkType_FullParam : PagingRequestBase
{
    public string? Code { get; set; }
    public string? Name { get; set; }
    public string? CalculationType { get; set; }
    public string? AppliesToType { get; set; }
    public DateTime? EffectiveDate { get; set; }
}
