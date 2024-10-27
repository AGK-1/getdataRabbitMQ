using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RabbitAmazon;

namespace getdatafromRabbit.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RabbitoController : ControllerBase
    {
        private readonly RabbitMqService _rabbitMqService;

        public RabbitoController()
        {
            _rabbitMqService = new RabbitMqService();
        }

        [HttpGet("receiveWithoutdeleting")]
        public ActionResult<string> ReceiveMessageWithoutDeleting()
        {
            var (message, deliveryTag) = _rabbitMqService.ReceiveMessageWithoutDeleting();

            if (message == "No messages")
            {
                return NotFound(message);
            }

            return Ok(new { message, deliveryTag });
        }

        [HttpGet("receiveVehicleWithoutDeleting")]
        public ActionResult<object> ReceiveMessageVehicleWithoutDeleting()
        {
            var (vehicles, deliveryTag) = _rabbitMqService.ReceiveMessageVehicleWithoutDeleting(); // Deconstruct the tuple

            if (vehicles.Count == 0)
            {
                return NotFound("No messages available.");
            }

            // Return the vehicles list and delivery tag in the required format
            return Ok(new { vehicles, deliveryTag });
        }
    }
}
