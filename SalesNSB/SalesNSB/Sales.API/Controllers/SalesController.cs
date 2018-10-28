using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NServiceBus;
using Sales.API.Application.Response;
using Sales.API.Application.Validations;
using Sales.Contracts.Commands;

namespace Sales.API.Controllers
{
    [Route("v1/[controller]")]
    [ApiController]
    public class SalesController : BaseController
    {
        private readonly IMessageSession _messageSession;
        private readonly PlaceOrderCommandValidator _placeOrderCommandValidator;
        private readonly ApiResponseHandler _apiResponseHandler;

        public SalesController(
            IMessageSession messageSession, 
            PlaceOrderCommandValidator placeOrderCommandValidator,
            ApiResponseHandler apiResponseHandler)
        {
            _messageSession = messageSession;
            _placeOrderCommandValidator = placeOrderCommandValidator;
            _apiResponseHandler = apiResponseHandler;
        }

        [HttpGet]
        public string Get()
        {
            return "Orders Get";
        }

        [HttpPost]
        public async Task<IActionResult> PlaceOrder([FromBody] PlaceOrderCommand placeOrderCommand)
        {
            try
            {
                placeOrderCommand.OrderId = Guid.NewGuid().ToString();
                var notification = _placeOrderCommandValidator.Validate(placeOrderCommand);
                throwErrors(notification);
                await _messageSession.Send(placeOrderCommand)
                    .ConfigureAwait(false);
                return Ok(placeOrderCommand);
            } catch (ArgumentException ex) {
                Console.WriteLine(ex.StackTrace);
                return BadRequest(_apiResponseHandler.AppErrorResponse(ex.Message));
            } catch(Exception ex) {
                Console.WriteLine(ex.StackTrace);
                return StatusCode(StatusCodes.Status500InternalServerError, _apiResponseHandler.InternalServerErrorResponse());
            }
        }
    }
}
