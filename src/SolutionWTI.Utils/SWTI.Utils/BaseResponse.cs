namespace SWTI.Utils
{
    public class BaseResponse
    {
        public bool HasError { get; set; }
        public string ErrorMessage { get; set; }
        public int ErrorCode { get; set; } = 0;
        public int StatusCode { get; set; } = 200;


        public BaseResponse()
        {
            HasError = false;
            ErrorCode = 0;
            ErrorMessage = "Success";
        }

        public BaseResponse(bool hasError, int errorCode, string message)
        {
            HasError = hasError;
            ErrorCode = errorCode;
            ErrorMessage = message;
        }

        public void ThrowExceptionIfHasError()
        {
            if (HasError)
            {
                throw new Exception($"{ErrorCode} - {ErrorMessage}");
            }
        }
    }

    public class BaseResponse<T> : BaseResponse
    {
        public T Data { get; set; }

        public BaseResponse()
        {
        }

        public BaseResponse(int errorCode, string message, T data)
        {
            base.HasError = ((errorCode != 0) ? true : false);
            base.StatusCode = 200;
            base.ErrorMessage = message;
            base.ErrorCode = errorCode;
            Data = data;
        }
    }

    public static class BaseResponseExt
    {
        public static BaseResponse<T> Success<T>(T data)
        {
            return new BaseResponse<T>
            {
                HasError = false,
                Data = data,
                ErrorCode = 0,
                ErrorMessage = "Success",
                StatusCode = 200
            };
        }

        public static BaseResponse<T> Success<T>(T data, BaseResponse baseResponse)
        {
            return new BaseResponse<T>
            {
                HasError = false,
                Data = data,
                ErrorCode = baseResponse.ErrorCode,
                ErrorMessage = baseResponse.ErrorMessage,
                StatusCode = 200
            };
        }

        public static BaseResponse Error(int errorCode, string message)
        {
            return new BaseResponse
            {
                HasError = true,
                ErrorCode = errorCode,
                ErrorMessage = message,
                StatusCode = 500
            };
        }

        public static BaseResponse Invalid(int errorCode, string message)
        {
            return new BaseResponse
            {
                HasError = true,
                ErrorCode = errorCode,
                ErrorMessage = message,
                StatusCode = 400
            };
        }
    }
}