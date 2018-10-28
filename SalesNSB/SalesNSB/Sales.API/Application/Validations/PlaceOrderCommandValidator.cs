using Sales.API.Application.Notifications;
using Sales.Contracts.Commands;

namespace Sales.API.Application.Validations
{
    public class PlaceOrderCommandValidator
    {
        public Notification Validate(PlaceOrderCommand placeOrderCommand)
        {
            Notification notification = new Notification();
            if (placeOrderCommand == null)
            {
                notification.AddError("Missing command parameters");
                return notification;
            }
            if (string.IsNullOrEmpty(placeOrderCommand.OrderId))
            {
                notification.AddError("OrderId is missing");
            }
            if (string.IsNullOrEmpty(placeOrderCommand.OrderData))
            {
                notification.AddError("OrderData is missing");
            }
            return notification;
        }
    }
}
