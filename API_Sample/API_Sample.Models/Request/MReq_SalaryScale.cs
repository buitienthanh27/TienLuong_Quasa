using API_Sample.Models.Common;
using System.ComponentModel.DataAnnotations;

namespace API_Sample.Models.Request
{
    public class MReq_SalaryScale : BaseModel.History
    {
        public int Id { get; set; }

        [Required]
        public int TramId { get; set; }

        [Required]
        [StringLength(5)]
        public string Grade { get; set; }

        public decimal Coefficient { get; set; }

        public decimal BaseRate { get; set; }

        public DateTime EffectiveDate { get; set; }
    }

    public class MReq_SalaryScale_FullParam : PagingRequestBase
    {
        public string SequenceStatus { get; set; }
        public int? TramId { get; set; }
        public string Grade { get; set; }
        public DateTime? EffectiveDate { get; set; }
    }
}
