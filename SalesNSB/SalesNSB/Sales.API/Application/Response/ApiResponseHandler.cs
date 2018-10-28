using System.Collections.Generic;
using System.Linq;

namespace Sales.API.Application.Response
{
    public class ApiResponseHandler
    {
        public ApiErrorDto AppErrorResponse(string errorMessages, string separator = "£")
        {
            ApiErrorDto apiErrorDto = new ApiErrorDto();
            List<ErrorDto> errorList = new List<ErrorDto>();            
            List<string> errors = errorMessages.Split(separator).ToList();
            foreach (string error in errors) {
			    errorList.Add(new ErrorDto(error));
            }
            apiErrorDto.Errors = errorList;
            return apiErrorDto;
        }

        public ApiErrorDto InternalServerErrorResponse()
        {
            ApiErrorDto apiErrorDto = new ApiErrorDto();
            List<ErrorDto> errorList = new List<ErrorDto>();
            errorList.Add(new ErrorDto("Internal Server Error"));
            apiErrorDto.Errors = errorList;
            return apiErrorDto;
        }
    }
}
