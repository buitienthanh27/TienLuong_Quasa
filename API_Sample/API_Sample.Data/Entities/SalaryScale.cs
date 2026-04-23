using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API_Sample.Data.Entities;

[Table("salary_scale")]
public partial class SalaryScale : BaseEntity
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("tram_id")]
    public int TramId { get; set; }

    [Required]
    [Column("grade")]
    [StringLength(5)]
    public string Grade { get; set; }

    [Column("coefficient", TypeName = "decimal(5,2)")]
    public decimal Coefficient { get; set; }

    [Column("base_rate", TypeName = "decimal(18,4)")]
    public decimal BaseRate { get; set; }

    [Column("effective_date", TypeName = "date")]
    public DateTime EffectiveDate { get; set; }

    /// <summary>
    /// Loại lao động áp dụng. NULL = áp dụng mọi loại
    /// </summary>
    [Column("employee_type_id")]
    public int? EmployeeTypeId { get; set; }

    /// <summary>
    /// Chức vụ áp dụng. NULL = áp dụng mọi chức vụ
    /// </summary>
    [Column("position_id")]
    public int? PositionId { get; set; }

    /// <summary>
    /// Độ ưu tiên khi có nhiều scale match. Cao hơn = ưu tiên hơn.
    /// </summary>
    [Column("priority")]
    public int Priority { get; set; } = 0;

    [ForeignKey("TramId")]
    public virtual Tram Tram { get; set; }

    [ForeignKey("EmployeeTypeId")]
    public virtual EmployeeType? EmployeeType { get; set; }

    [ForeignKey("PositionId")]
    public virtual Position? Position { get; set; }
}
