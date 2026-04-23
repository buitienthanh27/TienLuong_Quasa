using API_Sample.Data.Entities;
using API_Sample.Models.Common;
using API_Sample.Models.Request;
using API_Sample.Models.Response;
using AutoMapper;

namespace API_Sample.Application.Mapper
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            //Common
            #region Common
            CreateMap<Image, MRes_Image>();
            CreateMap<Image, BaseModel.Image>();
            #endregion

            //Main
            #region Main
            CreateMap<MReq_Product, Product>()
                .ForMember(x => x.Id, opt => opt.Ignore())
                .ForMember(x => x.CreatedAt, opt => opt.Ignore())
                .ForMember(x => x.CreatedBy, opt => opt.Ignore())
                .ForMember(x => x.UpdatedAt, opt => opt.Ignore())
                .ForMember(x => x.UpdatedBy, opt => opt.Ignore());
            CreateMap<Product, MRes_Product>();

            CreateMap<MReq_Account, Account>()
                .ForMember(x => x.Id, opt => opt.Ignore())
                .ForMember(x => x.CreatedAt, opt => opt.Ignore())
                .ForMember(x => x.CreatedBy, opt => opt.Ignore())
                .ForMember(x => x.UpdatedAt, opt => opt.Ignore())
                .ForMember(x => x.UpdatedBy, opt => opt.Ignore());
            CreateMap<Account, MRes_Account>();
            #endregion

            #region Payroll
            CreateMap<MReq_Tram, Tram>()
                .ForMember(x => x.Id, opt => opt.Ignore())
                .ForMember(x => x.CreatedAt, opt => opt.Ignore())
                .ForMember(x => x.CreatedBy, opt => opt.Ignore())
                .ForMember(x => x.UpdatedAt, opt => opt.Ignore())
                .ForMember(x => x.UpdatedBy, opt => opt.Ignore());
            CreateMap<Tram, MRes_Tram>();

            CreateMap<MReq_Employee, Employee>()
                .ForMember(x => x.Id, opt => opt.Ignore())
                .ForMember(x => x.CreatedAt, opt => opt.Ignore())
                .ForMember(x => x.CreatedBy, opt => opt.Ignore())
                .ForMember(x => x.UpdatedAt, opt => opt.Ignore())
                .ForMember(x => x.UpdatedBy, opt => opt.Ignore());
            CreateMap<Employee, MRes_Employee>()
                .ForMember(d => d.TramCode, opt => opt.MapFrom(s => s.Tram != null ? s.Tram.Code : null))
                .ForMember(d => d.TramName, opt => opt.MapFrom(s => s.Tram != null ? s.Tram.Name : null))
                .ForMember(d => d.DepartmentName, opt => opt.MapFrom(s => s.Department != null ? s.Department.Name : null))
                .ForMember(d => d.PositionName, opt => opt.MapFrom(s => s.Position != null ? s.Position.Name : null));

            CreateMap<MReq_SystemParameter, SystemParameter>()
                .ForMember(x => x.Id, opt => opt.Ignore())
                .ForMember(x => x.CreatedAt, opt => opt.Ignore())
                .ForMember(x => x.CreatedBy, opt => opt.Ignore())
                .ForMember(x => x.UpdatedAt, opt => opt.Ignore())
                .ForMember(x => x.UpdatedBy, opt => opt.Ignore());
            CreateMap<SystemParameter, MRes_SystemParameter>();

            CreateMap<MReq_SalaryScale, SalaryScale>()
                .ForMember(x => x.Id, opt => opt.Ignore())
                .ForMember(x => x.CreatedAt, opt => opt.Ignore())
                .ForMember(x => x.CreatedBy, opt => opt.Ignore())
                .ForMember(x => x.UpdatedAt, opt => opt.Ignore())
                .ForMember(x => x.UpdatedBy, opt => opt.Ignore());
            CreateMap<SalaryScale, MRes_SalaryScale>()
                .ForMember(d => d.TramCode, opt => opt.MapFrom(s => s.Tram != null ? s.Tram.Code : null))
                .ForMember(d => d.TramName, opt => opt.MapFrom(s => s.Tram != null ? s.Tram.Name : null));

            CreateMap<MReq_CostCenter, CostCenter>()
                .ForMember(x => x.Id, opt => opt.Ignore())
                .ForMember(x => x.CreatedAt, opt => opt.Ignore())
                .ForMember(x => x.CreatedBy, opt => opt.Ignore())
                .ForMember(x => x.UpdatedAt, opt => opt.Ignore())
                .ForMember(x => x.UpdatedBy, opt => opt.Ignore());
            CreateMap<CostCenter, MRes_CostCenter>();

            CreateMap<MReq_Attendance, Attendance>()
                .ForMember(x => x.Id, opt => opt.Ignore())
                .ForMember(x => x.CreatedAt, opt => opt.Ignore())
                .ForMember(x => x.CreatedBy, opt => opt.Ignore())
                .ForMember(x => x.UpdatedAt, opt => opt.Ignore())
                .ForMember(x => x.UpdatedBy, opt => opt.Ignore());
            CreateMap<Attendance, MRes_Attendance>()
                .ForMember(d => d.EmployeeMsnv, opt => opt.MapFrom(s => s.Employee != null ? s.Employee.Msnv : null))
                .ForMember(d => d.EmployeeName, opt => opt.MapFrom(s => s.Employee != null ? s.Employee.FullName : null))
                .ForMember(d => d.TramId, opt => opt.MapFrom(s => s.Employee != null ? s.Employee.TramId : (int?)null))
                .ForMember(d => d.TramCode, opt => opt.MapFrom(s => s.Employee != null && s.Employee.Tram != null ? s.Employee.Tram.Code : null))
                .ForMember(d => d.TramName, opt => opt.MapFrom(s => s.Employee != null && s.Employee.Tram != null ? s.Employee.Tram.Name : null));

            CreateMap<MReq_Performance, Performance>()
                .ForMember(x => x.Id, opt => opt.Ignore())
                .ForMember(x => x.CreatedAt, opt => opt.Ignore())
                .ForMember(x => x.CreatedBy, opt => opt.Ignore())
                .ForMember(x => x.UpdatedAt, opt => opt.Ignore())
                .ForMember(x => x.UpdatedBy, opt => opt.Ignore());
            CreateMap<Performance, MRes_Performance>()
                .ForMember(d => d.EmployeeMsnv, opt => opt.MapFrom(s => s.Employee != null ? s.Employee.Msnv : null))
                .ForMember(d => d.EmployeeName, opt => opt.MapFrom(s => s.Employee != null ? s.Employee.FullName : null))
                .ForMember(d => d.TramName, opt => opt.MapFrom(s => s.Employee != null && s.Employee.Tram != null ? s.Employee.Tram.Name : null));

            CreateMap<Payroll, MRes_Payroll>()
                .ForMember(d => d.EmployeeMsnv, opt => opt.MapFrom(s => s.Employee != null ? s.Employee.Msnv : null))
                .ForMember(d => d.EmployeeName, opt => opt.MapFrom(s => s.Employee != null ? s.Employee.FullName : null))
                .ForMember(d => d.TramCode, opt => opt.MapFrom(s => s.Employee != null && s.Employee.Tram != null ? s.Employee.Tram.Code : null))
                .ForMember(d => d.TramName, opt => opt.MapFrom(s => s.Employee != null && s.Employee.Tram != null ? s.Employee.Tram.Name : null))
                .ForMember(d => d.TechnicalGrade, opt => opt.MapFrom(s => s.Employee != null ? s.Employee.TechnicalGrade : null));

            CreateMap<PayrollDetail, MRes_PayrollDetail>();

            CreateMap<CostAllocation, MRes_CostAllocation>()
                .ForMember(d => d.CostCenterCode, opt => opt.MapFrom(s => s.CostCenter != null ? s.CostCenter.Code : null))
                .ForMember(d => d.CostCenterName, opt => opt.MapFrom(s => s.CostCenter != null ? s.CostCenter.Name : null));

            CreateMap<MReq_Production, Production>()
                .ForMember(x => x.Id, opt => opt.Ignore())
                .ForMember(x => x.CreatedAt, opt => opt.Ignore())
                .ForMember(x => x.CreatedBy, opt => opt.Ignore())
                .ForMember(x => x.UpdatedAt, opt => opt.Ignore())
                .ForMember(x => x.UpdatedBy, opt => opt.Ignore());
            CreateMap<Production, MRes_Production>()
                .ForMember(d => d.EmployeeMsnv, opt => opt.MapFrom(s => s.Employee != null ? s.Employee.Msnv : null))
                .ForMember(d => d.EmployeeName, opt => opt.MapFrom(s => s.Employee != null ? s.Employee.FullName : null))
                .ForMember(d => d.TramId, opt => opt.MapFrom(s => s.Employee != null ? s.Employee.TramId : (int?)null))
                .ForMember(d => d.TramCode, opt => opt.MapFrom(s => s.Employee != null && s.Employee.Tram != null ? s.Employee.Tram.Code : null))
                .ForMember(d => d.TramName, opt => opt.MapFrom(s => s.Employee != null && s.Employee.Tram != null ? s.Employee.Tram.Name : null));

            CreateMap<MReq_DrcRate, DrcRate>()
                .ForMember(x => x.Id, opt => opt.Ignore())
                .ForMember(x => x.CreatedAt, opt => opt.Ignore())
                .ForMember(x => x.CreatedBy, opt => opt.Ignore())
                .ForMember(x => x.UpdatedAt, opt => opt.Ignore())
                .ForMember(x => x.UpdatedBy, opt => opt.Ignore());
            CreateMap<DrcRate, MRes_DrcRate>()
                .ForMember(d => d.TramCode, opt => opt.MapFrom(s => s.Tram != null ? s.Tram.Code : null))
                .ForMember(d => d.TramName, opt => opt.MapFrom(s => s.Tram != null ? s.Tram.Name : null));
            #endregion

            #region New Config Entities (No Hardcode)
            CreateMap<MReq_EmployeeType, EmployeeType>()
                .ForMember(x => x.Id, opt => opt.Ignore())
                .ForMember(x => x.CreatedAt, opt => opt.Ignore())
                .ForMember(x => x.CreatedBy, opt => opt.Ignore())
                .ForMember(x => x.UpdatedAt, opt => opt.Ignore())
                .ForMember(x => x.UpdatedBy, opt => opt.Ignore());
            CreateMap<EmployeeType, MRes_EmployeeType>();

            CreateMap<MReq_TechnicalGrade, TechnicalGrade>()
                .ForMember(x => x.Id, opt => opt.Ignore())
                .ForMember(x => x.CreatedAt, opt => opt.Ignore())
                .ForMember(x => x.CreatedBy, opt => opt.Ignore())
                .ForMember(x => x.UpdatedAt, opt => opt.Ignore())
                .ForMember(x => x.UpdatedBy, opt => opt.Ignore());
            CreateMap<TechnicalGrade, MRes_TechnicalGrade>();

            CreateMap<MReq_RubberUnitPrice, RubberUnitPrice>()
                .ForMember(x => x.Id, opt => opt.Ignore())
                .ForMember(x => x.CreatedAt, opt => opt.Ignore())
                .ForMember(x => x.CreatedBy, opt => opt.Ignore())
                .ForMember(x => x.UpdatedAt, opt => opt.Ignore())
                .ForMember(x => x.UpdatedBy, opt => opt.Ignore());
            CreateMap<RubberUnitPrice, MRes_RubberUnitPrice>()
                .ForMember(d => d.TramCode, opt => opt.MapFrom(s => s.Tram != null ? s.Tram.Code : null))
                .ForMember(d => d.TramName, opt => opt.MapFrom(s => s.Tram != null ? s.Tram.Name : null));

            CreateMap<MReq_ExchangeRate, ExchangeRate>()
                .ForMember(x => x.Id, opt => opt.Ignore())
                .ForMember(x => x.CreatedAt, opt => opt.Ignore())
                .ForMember(x => x.CreatedBy, opt => opt.Ignore())
                .ForMember(x => x.UpdatedAt, opt => opt.Ignore())
                .ForMember(x => x.UpdatedBy, opt => opt.Ignore());
            CreateMap<ExchangeRate, MRes_ExchangeRate>();

            CreateMap<MReq_WorkType, WorkType>()
                .ForMember(x => x.Id, opt => opt.Ignore())
                .ForMember(x => x.CreatedAt, opt => opt.Ignore())
                .ForMember(x => x.CreatedBy, opt => opt.Ignore())
                .ForMember(x => x.UpdatedAt, opt => opt.Ignore())
                .ForMember(x => x.UpdatedBy, opt => opt.Ignore());
            CreateMap<WorkType, MRes_WorkType>();

            CreateMap<MReq_Holiday, Holiday>()
                .ForMember(x => x.Id, opt => opt.Ignore())
                .ForMember(x => x.CreatedAt, opt => opt.Ignore())
                .ForMember(x => x.CreatedBy, opt => opt.Ignore())
                .ForMember(x => x.UpdatedAt, opt => opt.Ignore())
                .ForMember(x => x.UpdatedBy, opt => opt.Ignore());
            CreateMap<Holiday, MRes_Holiday>();

            CreateMap<MReq_AdvancePayment, AdvancePayment>()
                .ForMember(x => x.Id, opt => opt.Ignore())
                .ForMember(x => x.CreatedAt, opt => opt.Ignore())
                .ForMember(x => x.CreatedBy, opt => opt.Ignore())
                .ForMember(x => x.UpdatedAt, opt => opt.Ignore())
                .ForMember(x => x.UpdatedBy, opt => opt.Ignore());
            CreateMap<AdvancePayment, MRes_AdvancePayment>()
                .ForMember(d => d.EmployeeMsnv, opt => opt.MapFrom(s => s.Employee != null ? s.Employee.Msnv : null))
                .ForMember(d => d.EmployeeName, opt => opt.MapFrom(s => s.Employee != null ? s.Employee.FullName : null))
                .ForMember(d => d.TramCode, opt => opt.MapFrom(s => s.Employee != null && s.Employee.Tram != null ? s.Employee.Tram.Code : null));

            CreateMap<MReq_TechnicalEvaluation, TechnicalEvaluation>()
                .ForMember(x => x.Id, opt => opt.Ignore())
                .ForMember(x => x.CreatedAt, opt => opt.Ignore())
                .ForMember(x => x.CreatedBy, opt => opt.Ignore())
                .ForMember(x => x.UpdatedAt, opt => opt.Ignore())
                .ForMember(x => x.UpdatedBy, opt => opt.Ignore());
            CreateMap<TechnicalEvaluation, MRes_TechnicalEvaluation>()
                .ForMember(d => d.EmployeeMsnv, opt => opt.MapFrom(s => s.Employee != null ? s.Employee.Msnv : null))
                .ForMember(d => d.EmployeeName, opt => opt.MapFrom(s => s.Employee != null ? s.Employee.FullName : null))
                .ForMember(d => d.TramCode, opt => opt.MapFrom(s => s.Employee != null && s.Employee.Tram != null ? s.Employee.Tram.Code : null));
            #endregion

            #region Payroll Extended Entities
            CreateMap<MReq_CareAdjustment, CareAdjustment>()
                .ForMember(x => x.Id, opt => opt.Ignore())
                .ForMember(x => x.CreatedAt, opt => opt.Ignore())
                .ForMember(x => x.CreatedBy, opt => opt.Ignore())
                .ForMember(x => x.UpdatedAt, opt => opt.Ignore())
                .ForMember(x => x.UpdatedBy, opt => opt.Ignore());
            CreateMap<CareAdjustment, MRes_CareAdjustment>()
                .ForMember(d => d.EmployeeMsnv, opt => opt.MapFrom(s => s.Employee != null ? s.Employee.Msnv : null))
                .ForMember(d => d.EmployeeName, opt => opt.MapFrom(s => s.Employee != null ? s.Employee.FullName : null))
                .ForMember(d => d.TramCode, opt => opt.MapFrom(s => s.Employee != null && s.Employee.Tram != null ? s.Employee.Tram.Code : null));

            CreateMap<MReq_PayrollReconciliation, PayrollReconciliation>()
                .ForMember(x => x.Id, opt => opt.Ignore())
                .ForMember(x => x.CreatedAt, opt => opt.Ignore())
                .ForMember(x => x.CreatedBy, opt => opt.Ignore())
                .ForMember(x => x.UpdatedAt, opt => opt.Ignore())
                .ForMember(x => x.UpdatedBy, opt => opt.Ignore());
            CreateMap<PayrollReconciliation, MRes_PayrollReconciliation>()
                .ForMember(d => d.TramCode, opt => opt.MapFrom(s => s.Tram != null ? s.Tram.Code : null))
                .ForMember(d => d.TramName, opt => opt.MapFrom(s => s.Tram != null ? s.Tram.Name : null));

            CreateMap<MReq_ZoneSupport, ZoneSupport>()
                .ForMember(x => x.Id, opt => opt.Ignore())
                .ForMember(x => x.CreatedAt, opt => opt.Ignore())
                .ForMember(x => x.CreatedBy, opt => opt.Ignore())
                .ForMember(x => x.UpdatedAt, opt => opt.Ignore())
                .ForMember(x => x.UpdatedBy, opt => opt.Ignore());
            CreateMap<ZoneSupport, MRes_ZoneSupport>()
                .ForMember(d => d.TramCode, opt => opt.MapFrom(s => s.Tram != null ? s.Tram.Code : null))
                .ForMember(d => d.TramName, opt => opt.MapFrom(s => s.Tram != null ? s.Tram.Name : null));

            CreateMap<MReq_EmployeeHistory, EmployeeHistory>()
                .ForMember(x => x.Id, opt => opt.Ignore())
                .ForMember(x => x.CreatedAt, opt => opt.Ignore())
                .ForMember(x => x.CreatedBy, opt => opt.Ignore())
                .ForMember(x => x.UpdatedAt, opt => opt.Ignore())
                .ForMember(x => x.UpdatedBy, opt => opt.Ignore());
            CreateMap<EmployeeHistory, MRes_EmployeeHistory>()
                .ForMember(d => d.EmployeeMsnv, opt => opt.MapFrom(s => s.Employee != null ? s.Employee.Msnv : null))
                .ForMember(d => d.EmployeeName, opt => opt.MapFrom(s => s.Employee != null ? s.Employee.FullName : null))
                .ForMember(d => d.TramCode, opt => opt.MapFrom(s => s.Employee != null && s.Employee.Tram != null ? s.Employee.Tram.Code : null));
            #endregion

            #region Config-Driven Payroll
            CreateMap<MReq_TaxBracket, TaxBracket>()
                .ForMember(x => x.Id, opt => opt.Ignore())
                .ForMember(x => x.CreatedAt, opt => opt.Ignore())
                .ForMember(x => x.CreatedBy, opt => opt.Ignore())
                .ForMember(x => x.UpdatedAt, opt => opt.Ignore())
                .ForMember(x => x.UpdatedBy, opt => opt.Ignore());
            CreateMap<TaxBracket, MRes_TaxBracket>();

            CreateMap<MReq_PayrollPolicy, PayrollPolicy>()
                .ForMember(x => x.Id, opt => opt.Ignore())
                .ForMember(x => x.CreatedAt, opt => opt.Ignore())
                .ForMember(x => x.CreatedBy, opt => opt.Ignore())
                .ForMember(x => x.UpdatedAt, opt => opt.Ignore())
                .ForMember(x => x.UpdatedBy, opt => opt.Ignore());
            CreateMap<PayrollPolicy, MRes_PayrollPolicy>()
                .ForMember(d => d.EmployeeTypeName, opt => opt.MapFrom(s => s.EmployeeType != null ? s.EmployeeType.Name : null))
                .ForMember(d => d.TramCode, opt => opt.MapFrom(s => s.Tram != null ? s.Tram.Code : null))
                .ForMember(d => d.PositionName, opt => opt.MapFrom(s => s.Position != null ? s.Position.Name : null));

            CreateMap<MReq_Position, Position>()
                .ForMember(x => x.Id, opt => opt.Ignore())
                .ForMember(x => x.CreatedAt, opt => opt.Ignore())
                .ForMember(x => x.CreatedBy, opt => opt.Ignore())
                .ForMember(x => x.UpdatedAt, opt => opt.Ignore())
                .ForMember(x => x.UpdatedBy, opt => opt.Ignore());
            CreateMap<Position, MRes_Position>();
            #endregion
        }
    }
}
