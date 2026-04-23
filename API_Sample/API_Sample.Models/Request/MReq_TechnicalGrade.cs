using System;
using System.ComponentModel.DataAnnotations;
using API_Sample.Models.Common;

namespace API_Sample.Models.Request;

public class MReq_TechnicalGrade : BaseModel.History
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Hạng là bắt buộc")]
    [StringLength(5)]
    public string Grade { get; set; }

    [Required(ErrorMessage = "Tên hạng là bắt buộc")]
    [StringLength(100)]
    public string Name { get; set; }

    [Range(0, 10, ErrorMessage = "Hệ số điểm phải từ 0 đến 10")]
    public decimal PointCoefficient { get; set; }

    public decimal? MinScore { get; set; }
    public decimal? MaxScore { get; set; }

    [StringLength(500)]
    public string? Description { get; set; }

    [Required(ErrorMessage = "Ngày hiệu lực là bắt buộc")]
    public DateTime EffectiveDate { get; set; }
}

public class MReq_TechnicalGrade_FullParam : PagingRequestBase
{
    public string? Grade { get; set; }
    public DateTime? EffectiveDate { get; set; }
}
