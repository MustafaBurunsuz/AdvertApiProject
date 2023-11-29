using advert.api.DbContext;
using advert.api.Models;
using advert.api.Services.abstractt;
using Dapper;
using System.Data;
using System.Data.Common;

namespace advert.api.Services.Concrete
{
    public class AdvertRepository : IAdvertRepository
    {
        private readonly IDbConnectionFactory _dbConnectionFactory;



        public AdvertRepository(IDbConnectionFactory dbConnectionFactory)
        {
            _dbConnectionFactory = dbConnectionFactory;
        }

        public async Task<AdvertModel> GetAllAdvertsAsync(FilterModel filters, SortModel sorting, int page)
        {
            using (var connection = _dbConnectionFactory.CreateConnection())
            {
                var sql = "SELECT Id, ModelName,Category, Year, Price, Title, Date, Km, Color, Gear, Fuel, FirstPhoto FROM Advert";

                // Filtreleme koşulları
                if (filters != null)
                {
                    if (filters.CategoryId != null)
                        sql += $" WHERE CategoryId = '{filters.CategoryId}'";

                    if (filters.FilterPrice != null)
                        sql += (sql.Contains("WHERE") ? " AND" : " WHERE") + $" Price = '{filters.FilterPrice}'";

                    if (!string.IsNullOrEmpty(filters.Gear))
                        sql += (sql.Contains("WHERE") ? " AND" : " WHERE") + $" Gear = '{filters.Gear}'";

                    if (!string.IsNullOrEmpty(filters.Fuel))
                        sql += (sql.Contains("WHERE") ? " AND" : " WHERE") + $" Fuel = '{filters.Fuel}'";
                }

                // Sıralama koşulları
                if (sorting.Price == false && sorting.Year == false && sorting.Km == false)
                    sql += $" ORDER BY Id";
                else
                {
                    if (sorting.Price != false)
                        sql += $" ORDER BY Price";

                    if (sorting.Year != false)
                        sql += $" ORDER BY Year";

                    if (sorting.Km != false)
                        sql += $" ORDER BY Km";
                }

                var totalCount = await connection.QueryAsync<AdvertItem>(sql);

                // sayfalama koşulları
                var pageSize = 2; // Sayfa başına reklam sayısı
                var offset = (page - 1) * pageSize;
                sql += $" OFFSET {offset} ROWS FETCH NEXT {pageSize} ROWS ONLY";
                var adverts = await connection.QueryAsync<AdvertItem>(sql);

                return  new AdvertModel() { Adverts = adverts.ToList(),Total = totalCount.Count() };
            }

        }
        public async Task<AdvertDetail> GetAdvertDetailsAsync(int id)
        {
            using (var connection = _dbConnectionFactory.CreateConnection())
            {
                var sql = "SELECT * FROM Advert WHERE Id = @Id";

                var advert = await connection.QueryFirstOrDefaultAsync<AdvertDetail>(sql, new { Id = id });

                return advert;
            }
        }
        public async Task InsertAdvertVisitAsync(AdvertVisit advertVisit)
        {
            using (var connection = _dbConnectionFactory.CreateConnection())
            {
                var sql = @"INSERT INTO AdvertVisit (AdvertId, IpAddress, VisitDate)
                        VALUES (@AdvertId, @IpAddress, @VisitDate);
                        SELECT CAST(SCOPE_IDENTITY() as int)";

                var advert = await connection.QueryFirstOrDefaultAsync<AdvertVisit>(sql);
            }

        }
    }
}
