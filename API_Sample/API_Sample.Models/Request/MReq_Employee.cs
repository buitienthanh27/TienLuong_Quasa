using API_Sample.Models.Common;
using System.ComponentModel.DataAnnotations;

namespace API_Sample.Models.Request
{
    public class MReq_Employee : BaseModel.History
    {
        public int Id { get; set; }

        [Required]
        [StringLength(20)]
        public string Msnv { get; set; }

        [Required]
        [StringLength(100)]
        public string FullName { get; set; }

        [StringLength(100)]
        public string FullNameLocal { get; set; }

        [Required]
        public int TramId { get; set; }

        public int? DepartmentId { get; set; }

        public int? PositionId { get; set; }

        [StringLength(5)]
        public string TechnicalGrade { get; set; }

        public DateTime? HireDate { get; set; }

        public DateTime? BirthDate { get; set; }

        [StringLength(10)]
        public string Gender { get; set; }

        public bool InsuranceIncluded { get; set; } = true;
    }

    public class MReq_Employee_FullParam : PagingRequestBase
    {
        public string SequenceStatus { get; set; }
        public string SearchText { get; set; }
        public int? TramId { get; set; }
        public int? DepartmentId { get; set; }
        public int? PositionId { get; set; }
        public string TechnicalGrade { get; set; }
    }
}
