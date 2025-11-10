namespace LMS.Common.Models;

public class ApiResponse
{
    public int StatusCode { get; set; }
    public bool IsSuccess { get; set; }
    public string Message { get; set; }
    public object? Data { get; set; }

    public ApiResponse(int statusCode, bool isSuccess, string message, object? data = default)
    {
        StatusCode = statusCode;
        IsSuccess = isSuccess;
        Message = message;
        Data = data;
    }

    public static ApiResponse Success(object data, string message = "Success")
    {
        return new ApiResponse(200, true, message, data);
    }

    public static ApiResponse Success(string message = "Success")
    {
        return new ApiResponse(200, true, message, null);
    }

    public static ApiResponse Failure(int statusCode, string message)
    {
        return new ApiResponse(statusCode, false, message);
    }
}
