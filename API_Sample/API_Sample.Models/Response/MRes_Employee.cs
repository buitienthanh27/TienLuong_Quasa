using API_Sample.Models.Common;

namespace API_Sample.Models.Response
{
    public class MRes_Employee : BaseModel.History
    {
        public int Id { get; set; }
        public string Msnv { get; set; }
        public string FullName { get; set; }
        public string FullNameLocal { get; set; }
        public int TramId { get; set; }
        public int? DepartmentId { get; set; }
        public int? PositionId { get; set; }
        public string TechnicalGrade { get; set; }
        public DateTime? HireDate { get; set; }
        public DateTime? BirthDate { get; set; }
        public string Gender { get; set; }
        public bool InsuranceIncluded { get; set; }

        public string TramCode { get; set; }
        public string TramName { get; set; }
        public string DepartmentName { get; set; }
        public string PositionName { get; set; }
    }
}
