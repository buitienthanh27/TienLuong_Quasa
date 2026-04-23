using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API_Sample.Data.Entities;

[Table("tram")]
public partial class Tram : BaseEntity
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Required]
    [Column("code")]
    [StringLength(10)]
    public string Code { get; set; }

    [Required]
    [Column("name")]
    [StringLength(100)]
    public string Name { get; set; }

    [Column("description")]
    [StringLength(500)]
    public string? Description { get; set; }

    public virtual ICollection<Employee> Employees { get; set; } = new List<Employee>();
    public virtual ICollection<SalaryScale> SalaryScales { get; set; } = new List<SalaryScale>();
    public virtual ICollection<ZoneSupport> ZoneSupports { get; set; } = new List<ZoneSupport>();
}
