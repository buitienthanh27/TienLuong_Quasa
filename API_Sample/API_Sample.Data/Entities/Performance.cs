using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API_Sample.Data.Entities;

[Table("performance")]
public partial class Performance : BaseEntity
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

    [Column("grade")]
    [StringLength(5)]
    public string? Grade { get; set; }

    [Column("output_kg", TypeName = "decimal(18,4)")]
    public decimal? OutputKg { get; set; }

    [Column("note")]
    [StringLength(500)]
    public string? Note { get; set; }

    [ForeignKey("EmployeeId")]
    public virtual Employee Employee { get; set; }
}
