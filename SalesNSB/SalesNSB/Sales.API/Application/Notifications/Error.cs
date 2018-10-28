namespace Sales.API.Application.Notifications
{
    public class Error
    {
        public string Message { get; set; }

        public Error(string message)
        {
            Message = message;
        }
    }
}
