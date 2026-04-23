using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace API_Sample.Data.Entities;

public abstract class BaseEntity
{
    [Column("status")]
    public short Status { get; set; }

    [Column("created_at", TypeName = "datetime")]
    public DateTime CreatedAt { get; set; }

    [Column("created_by")]
    public int CreatedBy { get; set; }

    [Column("updated_at", TypeName = "datetime")]
    public DateTime? UpdatedAt { get; set; }

    [Column("updated_by")]
    public int? UpdatedBy { get; set; }
}
