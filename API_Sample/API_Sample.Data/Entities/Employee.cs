using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API_Sample.Data.Entities;

[Table("employee")]
public partial class Employee : BaseEntity
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Required]
    [Column("msnv")]
    [StringLength(20)]
    public string Msnv { get; set; }

    [Required]
    [Column("full_name")]
    [StringLength(100)]
    public string FullName { get; set; }

    [Column("full_name_local")]
    [StringLength(100)]
    public string? FullNameLocal { get; set; }

    [Column("tram_id")]
    public int TramId { get; set; }

    [Column("department_id")]
    public int? DepartmentId { get; set; }

    [Column("position_id")]
    public int? PositionId { get; set; }

    [Column("technical_grade")]
    [StringLength(5)]
    public string? TechnicalGrade { get; set; }

    [Column("hire_date", TypeName = "date")]
    public DateTime? HireDate { get; set; }

    [Column("birth_date", TypeName = "date")]
    public DateTime? BirthDate { get; set; }

    [Column("gender")]
    [StringLength(10)]
    public string? Gender { get; set; }

    [Column("insurance_included")]
    public bool InsuranceIncluded { get; set; } = true;

    /// <summary>
    /// Loại lao động: CNKT, BV, TV, CB, CS
    /// </summary>
    [Column("employee_type_id")]
    public int? EmployeeTypeId { get; set; }

    /// <summary>
    /// Thuộc vùng khó khăn (có hỗ trợ thêm đơn giá)
    /// </summary>
    [Column("is_difficult_area")]
    public bool IsDifficultArea { get; set; } = false;

    /// <summary>
    /// Quốc tịch: VN, LA
    /// </summary>
    [Column("nationality")]
    [StringLength(20)]
    public string? Nationality { get; set; }

    /// <summary>
    /// Ghi chú bảo hiểm đặc biệt (CN Lào có BH riêng)
    /// </summary>
    [Column("insurance_note")]
    [StringLength(500)]
    public string? InsuranceNote { get; set; }

    /// <summary>
    /// Có thuộc diện chịu thuế TNCN không
    /// </summary>
    [Column("is_taxable")]
    public bool IsTaxable { get; set; } = false;

    [ForeignKey("TramId")]
    public virtual Tram Tram { get; set; }

    [ForeignKey("EmployeeTypeId")]
    public virtual EmployeeType? EmployeeType { get; set; }

    [ForeignKey("DepartmentId")]
    public virtual Department? Department { get; set; }

    [ForeignKey("PositionId")]
    public virtual Position? Position { get; set; }

    public virtual ICollection<Attendance> Attendances { get; set; } = new List<Attendance>();
    public virtual ICollection<Performance> Performances { get; set; } = new List<Performance>();
    public virtual ICollection<Allowance> Allowances { get; set; } = new List<Allowance>();
    public virtual ICollection<Payroll> Payrolls { get; set; } = new List<Payroll>();
    public virtual ICollection<Production> Productions { get; set; } = new List<Production>();
    public virtual ICollection<AdvancePayment> AdvancePayments { get; set; } = new List<AdvancePayment>();
    public virtual ICollection<TechnicalEvaluation> TechnicalEvaluations { get; set; } = new List<TechnicalEvaluation>();
    public virtual ICollection<CareAdjustment> CareAdjustments { get; set; } = new List<CareAdjustment>();
    public virtual ICollection<EmployeeHistory> EmployeeHistories { get; set; } = new List<EmployeeHistory>();
}
