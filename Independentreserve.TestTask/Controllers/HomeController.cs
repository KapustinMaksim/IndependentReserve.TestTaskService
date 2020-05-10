using IndependentReserve.Worker.Worker;
using Microsoft.AspNetCore.Mvc;

namespace IndependentReserve.TestTaskService.Controllers
{

    [Route("")]
    [ApiController]
    public class HomeController : ControllerBase
    {
        private readonly OrderBookWorker _orderBookWorker;

        public HomeController([FromServices] OrderBookWorker orderBookWorker)
        {
            _orderBookWorker = orderBookWorker;
        }

        public string Index()
        {
            return "Running!";
        }

    }
}
