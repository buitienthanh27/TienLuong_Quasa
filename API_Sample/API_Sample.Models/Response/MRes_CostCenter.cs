using API_Sample.Models.Common;

namespace API_Sample.Models.Response
{
    public class MRes_CostCenter : BaseModel.History
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public decimal? AllocationRate { get; set; }
        public string AccountingCode { get; set; }
    }
}
