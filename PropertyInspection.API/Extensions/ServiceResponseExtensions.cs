using Microsoft.AspNetCore.Mvc;
using PropertyInspection.Shared;

namespace PropertyInspection.API.Extensions
{
    public static class ServiceResponseExtensions
    {
        public static ApiResponse<T> ToApiResponse<T>(this ServiceResponse<T> result, object? meta = null)
        {
            return new ApiResponse<T>
            {
                Success = result.Success,
                Message = result.Message ?? string.Empty,
                Data = result.Data!,
                Meta = meta,
                ErrorCode = result.ErrorCode
            };
        }

        public static ActionResult<ApiResponse<T>> ToActionResult<T>(
            this ControllerBase controller,
            ServiceResponse<T> result,
            object? meta = null)
        {
            var response = result.ToApiResponse(meta);

            if (result.Success)
            {
                return controller.Ok(response);
            }

            return result.ErrorCode switch
            {
                ServiceErrorCodes.NotFound => controller.NotFound(response),
                ServiceErrorCodes.Conflict => controller.Conflict(response),
                ServiceErrorCodes.Unauthorized => controller.Unauthorized(response),
                ServiceErrorCodes.Forbidden => controller.StatusCode(StatusCodes.Status403Forbidden, response),
                ServiceErrorCodes.ServerError => controller.StatusCode(StatusCodes.Status500InternalServerError, response),
                _ => controller.BadRequest(response)
            };
        }

        public static ActionResult<ApiResponse<T>> ToCreatedAtActionResult<T>(
            this ControllerBase controller,
            string actionName,
            object routeValues,
            ServiceResponse<T> result,
            object? meta = null)
        {
            if (!result.Success)
            {
                return controller.ToActionResult(result, meta);
            }

            return controller.CreatedAtAction(actionName, routeValues, result.ToApiResponse(meta));
        }
    }
}
