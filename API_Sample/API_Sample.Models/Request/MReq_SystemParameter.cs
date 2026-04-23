using API_Sample.Models.Common;
using System.ComponentModel.DataAnnotations;

namespace API_Sample.Models.Request
{
    public class MReq_SystemParameter : BaseModel.History
    {
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string ParamCode { get; set; }

        [Required]
        [StringLength(200)]
        public string ParamName { get; set; }

        public decimal ParamValue { get; set; }

        [StringLength(20)]
        public string DataType { get; set; }

        public DateTime EffectiveDate { get; set; }

        [StringLength(500)]
        public string Description { get; set; }
    }

    public class MReq_SystemParameter_FullParam : PagingRequestBase
    {
        public string SequenceStatus { get; set; }
        public string SearchText { get; set; }
        public string ParamCode { get; set; }
        public DateTime? EffectiveDate { get; set; }
    }
}
