namespace advert.api.Models
{
    public class FilterModel
    {
        public int? CategoryId { get; set; }
        public int? FilterPrice { get; set; }
        public string? Gear { get; set; } 
        public string? Fuel { get; set; }
    }
}
