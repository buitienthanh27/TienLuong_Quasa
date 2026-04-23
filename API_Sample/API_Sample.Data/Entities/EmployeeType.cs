using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API_Sample.Data.Entities;

/// <summary>
/// Loại lao động: CNKT, Bảo vệ, Tạp vụ, Cán bộ, Chăm sóc
/// </summary>
[Table("employee_type")]
public partial class EmployeeType : BaseEntity
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    /// <summary>
    /// Mã loại: CNKT, BV, TV, CB, CS
    /// </summary>
    [Required]
    [Column("code")]
    [StringLength(20)]
    public string Code { get; set; }

    /// <summary>
    /// Tên loại: Công nhân kỹ thuật, Bảo vệ...
    /// </summary>
    [Required]
    [Column("name")]
    [StringLength(100)]
    public string Name { get; set; }

    /// <summary>
    /// Đơn vị tiền tính lương: THB, LAK
    /// </summary>
    [Required]
    [Column("salary_currency")]
    [StringLength(10)]
    public string SalaryCurrency { get; set; }

    /// <summary>
    /// Đơn vị tiền trả lương: THB, LAK
    /// </summary>
    [Required]
    [Column("payment_currency")]
    [StringLength(10)]
    public string PaymentCurrency { get; set; }

    /// <summary>
    /// Có bảo hiểm mặc định không
    /// </summary>
    [Column("has_insurance")]
    public bool HasInsurance { get; set; } = false;

    /// <summary>
    /// Phương thức tính lương: PRODUCTION (sản lượng), FIXED (cố định), DAILY (theo ngày)
    /// </summary>
    [Required]
    [Column("calculation_method")]
    [StringLength(50)]
    public string CalculationMethod { get; set; }

    [Column("description")]
    [StringLength(500)]
    public string? Description { get; set; }

    /// <summary>
    /// Thứ tự sắp xếp
    /// </summary>
    [Column("sort_order")]
    public int SortOrder { get; set; } = 0;

    public virtual ICollection<Employee> Employees { get; set; } = new List<Employee>();
}
