using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API_Sample.Data.Entities;

[Table("position")]
public partial class Position : BaseEntity
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Required]
    [Column("code")]
    [StringLength(20)]
    public string Code { get; set; }

    [Required]
    [Column("name")]
    [StringLength(100)]
    public string Name { get; set; }

    [Column("type")]
    [StringLength(20)]
    public string? Type { get; set; }

    public virtual ICollection<Employee> Employees { get; set; } = new List<Employee>();
}
