using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace API_Sample.Data.Entities;

public partial class Image
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("serial_id")]
    public int? SerialId { get; set; }

    [Column("ref_id")]
    [StringLength(36)]
    public string RefId { get; set; }

    [Column("name")]
    [StringLength(50)]
    public string Name { get; set; }

    [Column("description")]
    [StringLength(150)]
    public string Description { get; set; }

    [Column("relative_url")]
    [StringLength(250)]
    public string RelativeUrl { get; set; }

    [Column("small_url")]
    [StringLength(250)]
    public string SmallUrl { get; set; }

    [Column("medium_url")]
    [StringLength(250)]
    public string MediumUrl { get; set; }

    [Column("created_at", TypeName = "datetime")]
    public DateTime CreatedAt { get; set; }

    [Column("created_by")]
    public int CreatedBy { get; set; }

    [Column("status")]
    public int Status { get; set; }

    [Column("timer", TypeName = "datetime")]
    public DateTime? Timer { get; set; }
}
