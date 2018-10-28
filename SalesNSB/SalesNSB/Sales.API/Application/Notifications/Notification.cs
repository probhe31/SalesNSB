using System.Collections.Generic;
using System.Linq;

namespace Sales.API.Application.Notifications
{
    public class Notification
    {
        private List<Error> _errors = new List<Error>();

        public void AddError(string message)
        {
            _errors.Add(new Error(message));
        }

        public string ErrorMessage(string separator = "£")
        {
            return string.Join(separator, _errors.Select(x => x.Message));
        }

        public bool HasErrors()
        {
            return _errors.Any();
        }
    }
}
