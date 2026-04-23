using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API_Sample.Data.Entities;

[Table("payroll")]
public partial class Payroll : BaseEntity
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

    [Column("base_salary", TypeName = "decimal(18,4)")]
    public decimal BaseSalary { get; set; }

    [Column("performance_coef", TypeName = "decimal(5,2)")]
    public decimal PerformanceCoef { get; set; }

    [Column("working_days", TypeName = "decimal(5,2)")]
    public decimal WorkingDays { get; set; }

    [Column("gross_salary", TypeName = "decimal(18,4)")]
    public decimal GrossSalary { get; set; }

    [Column("bhxh", TypeName = "decimal(18,4)")]
    public decimal Bhxh { get; set; }

    [Column("bhyt", TypeName = "decimal(18,4)")]
    public decimal Bhyt { get; set; }

    [Column("income_tax", TypeName = "decimal(18,4)")]
    public decimal IncomeTax { get; set; }

    [Column("total_deductions", TypeName = "decimal(18,4)")]
    public decimal TotalDeductions { get; set; }

    [Column("total_allowances", TypeName = "decimal(18,4)")]
    public decimal TotalAllowances { get; set; }

    [Column("net_salary", TypeName = "decimal(18,4)")]
    public decimal NetSalary { get; set; }

    [Column("payroll_status")]
    [StringLength(20)]
    public string PayrollStatus { get; set; } = "DRAFT";

    [Column("calculated_at", TypeName = "datetime")]
    public DateTime? CalculatedAt { get; set; }

    [Column("calculated_by")]
    public int? CalculatedBy { get; set; }

    [ForeignKey("EmployeeId")]
    public virtual Employee Employee { get; set; }

    public virtual ICollection<PayrollDetail> PayrollDetails { get; set; } = new List<PayrollDetail>();
    public virtual ICollection<CostAllocation> CostAllocations { get; set; } = new List<CostAllocation>();
    public virtual ICollection<PayrollAudit> PayrollAudits { get; set; } = new List<PayrollAudit>();
}
