using System.Collections.Generic;

namespace Sales.API.Application.Response
{
    public class ApiErrorDto
    {
        public List<ErrorDto> Errors { get; set; } = new List<ErrorDto>();
    }
}
