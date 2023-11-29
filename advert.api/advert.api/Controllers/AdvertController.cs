using advert.api.Models;
using advert.api.Services.abstractt;
using advert.api.Services.Concrete;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace advert.api.Controllers
{
    [Route("advert")]
    [ApiController]
    public class AdvertController : ControllerBase
    {
        private readonly IAdvertRepository _advertRepository;
        private readonly IRabbitMqService _rabbitMqService;
        private readonly IRabbitMqConsumer _rabbitMqConsumer;

        public AdvertController(IAdvertRepository advertRepository, IRabbitMqService rabbitMqService, IRabbitMqConsumer rabbitMqConsumer)
        {
            _advertRepository = advertRepository;
            _rabbitMqService = rabbitMqService;
            _rabbitMqConsumer = rabbitMqConsumer;
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAllAdverts(
            [FromQuery] FilterModel filters,
            [FromQuery] SortModel sorting,
            int page = 1)
        {
            try
            {
                // Veritabanından reklam verilerini çekme işlemleri.
                AdvertModel fetchedAdverts = await _advertRepository.GetAllAdvertsAsync(filters, sorting, page);

                if (fetchedAdverts != null)
                {
                    // Başarılı işlem durumu (200 OK)

                    return Ok(new
                    {
                        total = fetchedAdverts.Total, // Toplam reklam sayısı
                        page,
                        adverts = fetchedAdverts.Adverts // Çekilen reklam verileri
                    });
                }
                else
                {
                    // Reklam bulunamadı durumu (204 No Content)
                    return NoContent();
                }
            }
            catch (Exception ex)
            {
                // Hata durumu (500 Internal Server Error)
                return StatusCode(500, new
                {
                    error = "Internal error occurred",
                    details = ex.Message
                });
            }
        }
        [HttpGet("get")]
        public async Task<IActionResult> GetAdvertDetails(int id)
        {
            try
            {
                // Veritabanından reklam detaylarını çekme işlemleri...
                var advertDetails = await _advertRepository.GetAdvertDetailsAsync(id);

                if (advertDetails != null)
                {
                    // Başarılı işlem durumu (200 OK)
                    return Ok(advertDetails);
                }
                else
                {
                    // Reklam bulunamadı durumu (204 No Content)
                    return NoContent();
                }
            }
            catch (Exception ex)
            {
                // Hata durumu (500 Internal Server Error)
                return StatusCode(500, new
                {
                    error = "Internal error occurred",
                    details = ex.Message
                });
            }
        }

        [HttpPost("visit")]

        public async Task<IActionResult> AddAdvertVisit([FromBody] AdvertVisitRequestModel requestModel)
        {
            try
            { var ipAdress = HttpContext.Connection.RemoteIpAddress; 
                // RabbitMQ'ya mesaj gönder
                var message = $"AdvertId: {requestModel.AdvertId}, IpAddress: {ipAdress}, VisitDate: {System.DateTime.Now}";
                _rabbitMqService.SendMessage(message);

                _rabbitMqConsumer.Consume();
                return Ok("Visit recorded successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An internal error occurred.");
            }
        }
    }

}