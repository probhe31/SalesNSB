namespace Sales.API.Application.Response
{
    public class ErrorDto
    {
        public string Message { get; set; }

        public ErrorDto(string message)
        {
            Message = message;
        }
    }
}
