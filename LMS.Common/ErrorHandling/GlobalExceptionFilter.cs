using LMS.Common.ErrorHandling.CustomException;
using LMS.Common.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using NToastNotify;
using System.Net;

namespace LMS.Common.ErrorHandling;

public class GlobalExceptionFilter : IExceptionFilter
{
    private readonly ILogger<GlobalExceptionFilter> _logger;
    private readonly IToastNotification _toast;

    public GlobalExceptionFilter(ILogger<GlobalExceptionFilter> logger, IToastNotification toast)
    {
        _logger = logger;
        _toast = toast;
    }

    public void OnException(ExceptionContext context)
    {
        _logger.LogError(context.Exception, "An unhandled exception occurred.");

        var (statusCode, message, errorDetails) = GetExceptionDetails(context.Exception);

        var response = new ApiResponse(statusCode, statusCode == (int)HttpStatusCode.OK, message, errorDetails);

        if (statusCode == (int)HttpStatusCode.NotFound)
            _toast.AddInfoToastMessage(message);
        else
            _toast.AddErrorToastMessage(message);

        if (context.HttpContext.Request.Headers["X-Requested-With"] == "XMLHttpRequest")
        { 
            context.Result = new JsonResult(response);
        }
        else
        {
            string refererUrl = context.HttpContext.Request.Headers["Referer"].ToString();
            context.Result = !string.IsNullOrEmpty(refererUrl)
                ? new RedirectResult(refererUrl)
                : new RedirectToActionResult("Index", "Home", null);
        }

        context.ExceptionHandled = true;
    }

    private static (int StatusCode, string Message, object? Errors) GetExceptionDetails(Exception ex)
    {
        return ex switch
        {
            BadRequestException e => ((int)HttpStatusCode.BadRequest, e.Message, null),
            NotFoundException e => ((int)HttpStatusCode.NotFound, e.Message, null),
            CustomValidationException e => ((int)HttpStatusCode.BadRequest, e.Message, e.Errors.GroupBy(e => e.PropertyName).ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray())),
            _ => ((int)HttpStatusCode.InternalServerError, "An unexpected error occurred. Please try again later.", null)
        };
    }
}