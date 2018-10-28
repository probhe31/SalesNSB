using NServiceBus;
using NServiceBus.Logging;
using Sales.Contracts.Commands;
using Sales.Contracts.Events;
using System;
using System.Threading.Tasks;

namespace Sales.EndPoint
{
    public class PlaceOrderHandler : IHandleMessages<PlaceOrderCommand>
    {
        static ILog log = LogManager.GetLogger<PlaceOrderHandler>();
        static Random random = new Random();

        public Task Handle(PlaceOrderCommand message, IMessageHandlerContext context)
        {
            log.Info($"Received PlaceOrder, OrderId = {message.OrderId}, OrderData = {message.OrderData}");
            //throw new Exception("BOOM");
            if (random.Next(0, 5) == 0)
            {
                //throw new Exception("Oops");
            }
            var orderPlacedEvent = new OrderPlacedEvent
            {
                OrderId = message.OrderId
            };
            return context.Publish(orderPlacedEvent);
        }
    }
}
