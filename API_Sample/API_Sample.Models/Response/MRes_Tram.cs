using API_Sample.Models.Common;

namespace API_Sample.Models.Response
{
    public class MRes_Tram : BaseModel.History
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
