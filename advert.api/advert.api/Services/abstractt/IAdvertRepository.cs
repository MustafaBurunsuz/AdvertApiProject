using advert.api.Models;

namespace advert.api.Services.abstractt
{
    public interface IAdvertRepository
    {
        Task<AdvertModel> GetAllAdvertsAsync(FilterModel filters, SortModel sorting, int page);
        Task<AdvertDetail> GetAdvertDetailsAsync(int id);
        Task InsertAdvertVisitAsync(AdvertVisit advertVisit);


    }
}
