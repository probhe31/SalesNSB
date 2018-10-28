using Microsoft.AspNetCore.Mvc;
using Sales.API.Application.Notifications;
using System;

namespace Sales.API.Controllers
{
    public class BaseController : ControllerBase
    {
        public void throwErrors(Notification notification)
        {
            if (notification.HasErrors()) {
                throw new ArgumentException(notification.ErrorMessage());
            }
        }
    }
}
