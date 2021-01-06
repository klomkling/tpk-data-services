namespace Tpk.DataServices.Shared.Data.Models
{
    public class AppSettings
    {
        public string Secret { get; set; }
        public string Salt { get; set; }
        public string ConnectionName { get; set; }
        public int PageSize { get; set; }
        public string BaseUrl { get; set; }
    }
}