using System;
using System.Collections.Generic;
using API_Sample.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace API_Sample.Data.EF;

public partial class MainDbContext : DbContext
{
    public MainDbContext()
    {
    }

    public MainDbContext(DbContextOptions<MainDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Account> Accounts { get; set; }
    public virtual DbSet<Image> Images { get; set; }
    public virtual DbSet<Product> Products { get; set; }

    #region Payroll Entities
    public virtual DbSet<Tram> Trams { get; set; }
    public virtual DbSet<Department> Departments { get; set; }
    public virtual DbSet<Position> Positions { get; set; }
    public virtual DbSet<Employee> Employees { get; set; }
    public virtual DbSet<SystemParameter> SystemParameters { get; set; }
    public virtual DbSet<SalaryScale> SalaryScales { get; set; }
    public virtual DbSet<AllowanceType> AllowanceTypes { get; set; }
    public virtual DbSet<CostCenter> CostCenters { get; set; }
    public virtual DbSet<Attendance> Attendances { get; set; }
    public virtual DbSet<Performance> Performances { get; set; }
    public virtual DbSet<Allowance> Allowances { get; set; }
    public virtual DbSet<Payroll> Payrolls { get; set; }
    public virtual DbSet<PayrollDetail> PayrollDetails { get; set; }
    public virtual DbSet<CostAllocation> CostAllocations { get; set; }
    public virtual DbSet<AuditLog> AuditLogs { get; set; }
    public virtual DbSet<Production> Productions { get; set; }
    public virtual DbSet<DrcRate> DrcRates { get; set; }
    #endregion

    #region New Config Entities (No Hardcode)
    public virtual DbSet<EmployeeType> EmployeeTypes { get; set; }
    public virtual DbSet<TechnicalGrade> TechnicalGrades { get; set; }
    public virtual DbSet<RubberUnitPrice> RubberUnitPrices { get; set; }
    public virtual DbSet<ExchangeRate> ExchangeRates { get; set; }
    public virtual DbSet<WorkType> WorkTypes { get; set; }
    public virtual DbSet<Holiday> Holidays { get; set; }
    public virtual DbSet<AdvancePayment> AdvancePayments { get; set; }
    public virtual DbSet<TechnicalEvaluation> TechnicalEvaluations { get; set; }
    public virtual DbSet<EmployeeCodeRule> EmployeeCodeRules { get; set; }
    #endregion

    #region Additional Payroll Entities
    public virtual DbSet<CareAdjustment> CareAdjustments { get; set; }
    public virtual DbSet<PayrollReconciliation> PayrollReconciliations { get; set; }
    public virtual DbSet<ZoneSupport> ZoneSupports { get; set; }
    public virtual DbSet<EmployeeHistory> EmployeeHistories { get; set; }
    public virtual DbSet<PayrollAudit> PayrollAudits { get; set; }
    public virtual DbSet<AccountingCode> AccountingCodes { get; set; }
    public virtual DbSet<TaxBracket> TaxBrackets { get; set; }
    public virtual DbSet<PayrollPolicy> PayrollPolicies { get; set; }
    #endregion

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Account>(entity =>
        {
            entity.ToTable("Account", tb => tb.HasComment("fruit-to-seed conversion rate"));
            entity.Property(e => e.Id).ValueGeneratedNever();
        });

        modelBuilder.Entity<Image>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Images__3213E83F47534C37");
            entity.Property(e => e.Id).ValueGeneratedNever();
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.ToTable("Product", tb => tb.HasComment("fruit-to-seed conversion rate"));
            entity.Property(e => e.RatioTransfer).HasComment("Tỉ lệ chuyển đổi");
        });

        #region Payroll Entity Configurations
        modelBuilder.Entity<Tram>(entity =>
        {
            entity.HasIndex(e => e.Code).IsUnique();
        });

        modelBuilder.Entity<Department>(entity =>
        {
            entity.HasIndex(e => e.Code).IsUnique();
            entity.HasOne(d => d.Parent)
                .WithMany(p => p.Children)
                .HasForeignKey(d => d.ParentId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Position>(entity =>
        {
            entity.HasIndex(e => e.Code).IsUnique();
        });

        modelBuilder.Entity<Employee>(entity =>
        {
            entity.HasIndex(e => e.Msnv).IsUnique();
            entity.HasOne(d => d.Tram)
                .WithMany(p => p.Employees)
                .HasForeignKey(d => d.TramId)
                .OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(d => d.Department)
                .WithMany(p => p.Employees)
                .HasForeignKey(d => d.DepartmentId)
                .OnDelete(DeleteBehavior.SetNull);
            entity.HasOne(d => d.Position)
                .WithMany(p => p.Employees)
                .HasForeignKey(d => d.PositionId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        modelBuilder.Entity<SystemParameter>(entity =>
        {
            entity.HasIndex(e => new { e.ParamCode, e.EffectiveDate }).IsUnique();
        });

        modelBuilder.Entity<SalaryScale>(entity =>
        {
            entity.HasIndex(e => new { e.TramId, e.Grade, e.EffectiveDate }).IsUnique();
            entity.HasOne(d => d.Tram)
                .WithMany(p => p.SalaryScales)
                .HasForeignKey(d => d.TramId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<AllowanceType>(entity =>
        {
            entity.HasIndex(e => e.Code).IsUnique();
        });

        modelBuilder.Entity<CostCenter>(entity =>
        {
            entity.HasIndex(e => e.Code).IsUnique();
        });

        modelBuilder.Entity<Attendance>(entity =>
        {
            entity.HasIndex(e => new { e.EmployeeId, e.YearMonth }).IsUnique();
            entity.HasOne(d => d.Employee)
                .WithMany(p => p.Attendances)
                .HasForeignKey(d => d.EmployeeId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Performance>(entity =>
        {
            entity.HasIndex(e => new { e.EmployeeId, e.YearMonth }).IsUnique();
            entity.HasOne(d => d.Employee)
                .WithMany(p => p.Performances)
                .HasForeignKey(d => d.EmployeeId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Allowance>(entity =>
        {
            entity.HasOne(d => d.Employee)
                .WithMany(p => p.Allowances)
                .HasForeignKey(d => d.EmployeeId)
                .OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(d => d.AllowanceType)
                .WithMany(p => p.Allowances)
                .HasForeignKey(d => d.AllowanceTypeId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Payroll>(entity =>
        {
            entity.HasIndex(e => new { e.EmployeeId, e.YearMonth }).IsUnique();
            entity.HasOne(d => d.Employee)
                .WithMany(p => p.Payrolls)
                .HasForeignKey(d => d.EmployeeId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<PayrollDetail>(entity =>
        {
            entity.HasOne(d => d.Payroll)
                .WithMany(p => p.PayrollDetails)
                .HasForeignKey(d => d.PayrollId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<CostAllocation>(entity =>
        {
            entity.HasOne(d => d.Payroll)
                .WithMany(p => p.CostAllocations)
                .HasForeignKey(d => d.PayrollId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(d => d.CostCenter)
                .WithMany(p => p.CostAllocations)
                .HasForeignKey(d => d.CostCenterId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<AuditLog>(entity =>
        {
            entity.HasIndex(e => new { e.TableName, e.RecordId });
            entity.HasIndex(e => e.ChangedAt);
        });

        modelBuilder.Entity<Production>(entity =>
        {
            entity.HasIndex(e => new { e.EmployeeId, e.YearMonth }).IsUnique();
            entity.HasOne(d => d.Employee)
                .WithMany(p => p.Productions)
                .HasForeignKey(d => d.EmployeeId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<DrcRate>(entity =>
        {
            entity.HasIndex(e => new { e.YearMonth, e.TramId }).IsUnique();
            entity.HasOne(d => d.Tram)
                .WithMany()
                .HasForeignKey(d => d.TramId)
                .OnDelete(DeleteBehavior.Restrict);
        });
        #endregion

        #region New Config Entity Configurations
        modelBuilder.Entity<EmployeeType>(entity =>
        {
            entity.HasIndex(e => e.Code).IsUnique().HasFilter("[status] != -1");
        });

        modelBuilder.Entity<Employee>(entity =>
        {
            entity.HasOne(d => d.EmployeeType)
                .WithMany(p => p.Employees)
                .HasForeignKey(d => d.EmployeeTypeId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        modelBuilder.Entity<TechnicalGrade>(entity =>
        {
            entity.HasIndex(e => new { e.Grade, e.EffectiveDate }).IsUnique().HasFilter("[status] != -1");
        });

        modelBuilder.Entity<RubberUnitPrice>(entity =>
        {
            entity.HasIndex(e => new { e.TramId, e.Grade, e.EffectiveDate }).IsUnique().HasFilter("[status] != -1");
            entity.HasOne(d => d.Tram)
                .WithMany()
                .HasForeignKey(d => d.TramId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<ExchangeRate>(entity =>
        {
            entity.HasIndex(e => new { e.YearMonth, e.FromCurrency, e.ToCurrency }).IsUnique().HasFilter("[status] != -1");
        });

        modelBuilder.Entity<WorkType>(entity =>
        {
            entity.HasIndex(e => new { e.Code, e.EffectiveDate }).IsUnique().HasFilter("[status] != -1");
        });

        modelBuilder.Entity<Holiday>(entity =>
        {
            entity.HasIndex(e => e.HolidayDate).IsUnique().HasFilter("[status] != -1");
        });

        modelBuilder.Entity<AdvancePayment>(entity =>
        {
            entity.HasIndex(e => new { e.EmployeeId, e.YearMonth });
            entity.HasOne(d => d.Employee)
                .WithMany(p => p.AdvancePayments)
                .HasForeignKey(d => d.EmployeeId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<TechnicalEvaluation>(entity =>
        {
            entity.HasIndex(e => new { e.EmployeeId, e.YearMonth }).IsUnique().HasFilter("[status] != -1");
            entity.HasOne(d => d.Employee)
                .WithMany(p => p.TechnicalEvaluations)
                .HasForeignKey(d => d.EmployeeId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<EmployeeCodeRule>(entity =>
        {
            entity.HasIndex(e => e.Year).IsUnique().HasFilter("[status] != -1");
        });
        #endregion

        #region Additional Payroll Entity Configurations
        modelBuilder.Entity<CareAdjustment>(entity =>
        {
            entity.HasIndex(e => new { e.EmployeeId, e.YearMonth });
            entity.HasOne(d => d.Employee)
                .WithMany(p => p.CareAdjustments)
                .HasForeignKey(d => d.EmployeeId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<PayrollReconciliation>(entity =>
        {
            entity.HasIndex(e => new { e.YearMonth, e.TramId }).IsUnique().HasFilter("[status] != -1");
            entity.HasOne(d => d.Tram)
                .WithMany()
                .HasForeignKey(d => d.TramId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<ZoneSupport>(entity =>
        {
            entity.HasIndex(e => new { e.TramId, e.SupportType, e.EffectiveDate }).IsUnique().HasFilter("[status] != -1");
            entity.HasOne(d => d.Tram)
                .WithMany(p => p.ZoneSupports)
                .HasForeignKey(d => d.TramId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<EmployeeHistory>(entity =>
        {
            entity.HasIndex(e => new { e.EmployeeId, e.ChangeType, e.ChangeDate });
            entity.HasOne(d => d.Employee)
                .WithMany(p => p.EmployeeHistories)
                .HasForeignKey(d => d.EmployeeId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<PayrollAudit>(entity =>
        {
            entity.HasIndex(e => new { e.PayrollId, e.AuditedAt });
            entity.HasOne(d => d.Payroll)
                .WithMany(p => p.PayrollAudits)
                .HasForeignKey(d => d.PayrollId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<AccountingCode>(entity =>
        {
            entity.HasIndex(e => e.Code).IsUnique().HasFilter("[status] != -1");
            entity.HasOne(d => d.Parent)
                .WithMany(p => p.Children)
                .HasForeignKey(d => d.ParentId)
                .OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(d => d.CostCenter)
                .WithMany()
                .HasForeignKey(d => d.CostCenterId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        modelBuilder.Entity<TaxBracket>(entity =>
        {
            entity.HasIndex(e => new { e.SortOrder, e.EffectiveDate }).HasFilter("[status] != -1");
        });

        modelBuilder.Entity<PayrollPolicy>(entity =>
        {
            entity.HasIndex(e => e.Code).IsUnique().HasFilter("[status] != -1");
            entity.HasIndex(e => new { e.EmployeeTypeId, e.TramId, e.PositionId, e.EffectiveDate, e.Priority })
                .HasFilter("[status] != -1");
            entity.HasOne(d => d.EmployeeType)
                .WithMany()
                .HasForeignKey(d => d.EmployeeTypeId)
                .OnDelete(DeleteBehavior.SetNull);
            entity.HasOne(d => d.Tram)
                .WithMany()
                .HasForeignKey(d => d.TramId)
                .OnDelete(DeleteBehavior.SetNull);
            entity.HasOne(d => d.Position)
                .WithMany()
                .HasForeignKey(d => d.PositionId)
                .OnDelete(DeleteBehavior.SetNull);
        });
        #endregion

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
