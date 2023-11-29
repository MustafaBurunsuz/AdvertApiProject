namespace advert.api.Models
{
    public class AdvertModel
    {
        public int Total { get; set; }
        public int Page { get; set; }
        public List<AdvertItem> Adverts { get; set; }
    }
}
