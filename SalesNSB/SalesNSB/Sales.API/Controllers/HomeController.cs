using Microsoft.AspNetCore.Mvc;
using System;

namespace Sales.API.Controllers
{
    [Route("")]
    [ApiController]
    public class HomeController : ControllerBase
    {
        public HomeController()
        {
        }

        [HttpGet]
        public Object Get()
        {
            return "Api Root";
        }
    }
}
