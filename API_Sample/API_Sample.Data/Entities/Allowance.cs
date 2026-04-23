using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API_Sample.Data.Entities;

[Table("allowance")]
public partial class Allowance : BaseEntity
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("employee_id")]
    public int EmployeeId { get; set; }

    [Required]
    [Column("year_month")]
    [StringLength(7)]
    public string YearMonth { get; set; }

    [Column("allowance_type_id")]
    public int AllowanceTypeId { get; set; }

    [Column("days_or_amount", TypeName = "decimal(18,4)")]
    public decimal DaysOrAmount { get; set; }

    [Column("calculated_amount", TypeName = "decimal(18,4)")]
    public decimal CalculatedAmount { get; set; }

    [ForeignKey("EmployeeId")]
    public virtual Employee Employee { get; set; }

    [ForeignKey("AllowanceTypeId")]
    public virtual AllowanceType AllowanceType { get; set; }
}
