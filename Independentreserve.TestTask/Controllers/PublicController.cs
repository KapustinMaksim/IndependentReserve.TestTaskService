using IndependentReserve.DataContracts.OrderBook.Enums;
using IndependentReserve.DataProviders;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IndependentReserve.TestTaskService.Controllers
{
    [Route("/[Controller]/[Action]")]
    [ApiController]
    public class PublicController : ControllerBase
    {
        private readonly OrderBookProvider _orderBookProvider;

        public PublicController(OrderBookProvider orderBookProvider)
        {
            _orderBookProvider = orderBookProvider;
        }

        [HttpGet, AllowAnonymous]
        public IActionResult GetAllOrders(PrimaryCurrencyCode primaryCurrencyCode, SecondaryCurrencyCode secondaryCurrencyCode)
        {
            var (orderBook,errorMsg) =  _orderBookProvider.GetAllOrders(primaryCurrencyCode, secondaryCurrencyCode);

            if (orderBook == null || !string.IsNullOrEmpty(errorMsg))
                return BadRequest(errorMsg);

            return Ok(orderBook);
        }

    }
}