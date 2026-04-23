using API_Sample.Models.Common;
using System.ComponentModel.DataAnnotations;

namespace API_Sample.Models.Request
{
    public class MReq_Position : BaseModel.History
    {
        public int Id { get; set; }

        [Required]
        [StringLength(20)]
        public string Code { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [StringLength(20)]
        public string? Type { get; set; }
    }

    public class MReq_Position_FullParam : PagingRequestBase
    {
        public string? SequenceStatus { get; set; }
        public string? Code { get; set; }
        public string? Name { get; set; }
        public string? Type { get; set; }
    }
}
