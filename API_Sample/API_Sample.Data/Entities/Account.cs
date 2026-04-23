using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace API_Sample.Data.Entities;

/// <summary>
/// fruit-to-seed conversion rate
/// </summary>
[Table("Account")]
public partial class Account
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Required]
    [Column("use_name")]
    [StringLength(20)]
    public string UserName { get; set; }

    [Required]
    [Column("password")]
    [StringLength(100)]
    public string Password { get; set; }

    [Required]
    [Column("first_name")]
    [StringLength(10)]
    public string FirstName { get; set; }

    [Required]
    [Column("last_name")]
    [StringLength(10)]
    public string LastName { get; set; }

    [Required]
    [Column("account_type")]
    [StringLength(10)]
    public string AccountType { get; set; }

    [Column("email")]
    [StringLength(100)]
    public string Email { get; set; }

    [Column("phone")]
    [StringLength(20)]
    public string Phone { get; set; }

    [Column("updated_at", TypeName = "datetime")]
    public DateTime? UpdatedAt { get; set; }

    [Column("updated_by")]
    public int? UpdatedBy { get; set; }

    [Column("status")]
    public short Status { get; set; }

    [Column("created_at", TypeName = "datetime")]
    public DateTime CreatedAt { get; set; }

    [Column("created_by")]
    public int CreatedBy { get; set; }

    [Column("refresh_token")]
    [StringLength(500)]
    public string? RefreshToken { get; set; }

    [Column("refresh_token_expiry_time", TypeName = "datetime")]
    public DateTime? RefreshTokenExpiryTime { get; set; }
}
