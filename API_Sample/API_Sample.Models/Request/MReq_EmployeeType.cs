using System.ComponentModel.DataAnnotations;
using API_Sample.Models.Common;

namespace API_Sample.Models.Request;

public class MReq_EmployeeType : BaseModel.History
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Mã loại là bắt buộc")]
    [StringLength(20)]
    public string Code { get; set; }

    [Required(ErrorMessage = "Tên loại là bắt buộc")]
    [StringLength(100)]
    public string Name { get; set; }

    [Required]
    [StringLength(10)]
    public string SalaryCurrency { get; set; } = "THB";

    [Required]
    [StringLength(10)]
    public string PaymentCurrency { get; set; } = "LAK";

    public bool HasInsurance { get; set; } = false;

    [Required]
    [StringLength(50)]
    public string CalculationMethod { get; set; } = "PRODUCTION";

    [StringLength(500)]
    public string? Description { get; set; }
}

public class MReq_EmployeeType_FullParam : PagingRequestBase
{
    public string? Code { get; set; }
    public string? Name { get; set; }
    public string? CalculationMethod { get; set; }
}
