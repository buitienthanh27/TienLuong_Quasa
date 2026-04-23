using API_Sample.Models.Common;

namespace API_Sample.Models.Response
{
    public class MRes_Image : BaseModel.History
    {
        public int Id { get; set; }

        public string RefId { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string RelativeUrl { get; set; }

        public string SmallUrl { get; set; }

        public string MediumUrl { get; set; }
    }
}
