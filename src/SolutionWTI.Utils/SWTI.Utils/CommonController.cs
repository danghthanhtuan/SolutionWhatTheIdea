using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace SWTI.Utils
{
    public class CommonController : ControllerBase
    {
        [NonAction]
        public ActionResult BaseResponseOk<T>(T data)
        {
            return StatusCode(200, BaseResponseExt.Success(data));
        }

        [NonAction]
        public ActionResult BaseResponseError(int htttpStatusCode, int errorCode, string message)
        {
            return StatusCode(htttpStatusCode, BaseResponseExt.Error(errorCode, message));
        }

        [NonAction]
        public ActionResult BaseResponseInternalError(int errorCode, string message)
        {
            return StatusCode(500, BaseResponseExt.Error(errorCode, message));
        }

        [NonAction]
        public ActionResult BaseResponse<T>(T data, BaseResponse? baseResponse)
        {
            if (baseResponse == null || !baseResponse!.HasError)
            {
                if (baseResponse != null)
                {
                    return StatusCode(200, BaseResponseExt.Success(data, baseResponse));
                }

                return StatusCode(200, BaseResponseExt.Success(data));
            }

            return StatusCode(baseResponse!.StatusCode, baseResponse);
        }

        [NonAction]
        public ActionResult BaseResponseInvalid(int errorCode, string message)
        {
            return StatusCode(400, BaseResponseExt.Invalid(errorCode, message));
        }

        //
        // Summary:
        //     Determines whether the specified request is invalid. true là có vấn đề false
        //     là pass
        //
        // Parameters:
        //   request:
        //     The request.
        [NonAction]
        public (string, BaseResponse) IsInvalid(object request)
        {
            if (request != null && base.ModelState.IsValid)
            {
                return (string.Empty, null);
            }

            string item = string.Join("; ", from x in base.ModelState.Values.SelectMany((ModelStateEntry x) => x.Errors)
                                            select x.ErrorMessage) + string.Join("; ", base.ModelState.Values.SelectMany((ModelStateEntry x) => x.Errors));
            return (item, BaseResponseExt.Invalid(500001, "Dữ liệu không hợp lệ"));
        }
    }
}
