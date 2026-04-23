using API_Sample.Models.Common;

namespace API_Sample.Models.Response
{
    public class MRes_Product : BaseModel.History
    {
        public int Id { get; set; }

        public string Code { get; set; }

        public string Name { get; set; }

        public string NameSlug { get; set; }

        public int? Sort { get; set; }

        public double? RatioTransfer { get; set; }

        public string Remark { get; set; }
    }
}
