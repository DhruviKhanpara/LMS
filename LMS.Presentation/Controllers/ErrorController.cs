using LMS.Presentation.Models;
using Microsoft.AspNetCore.Mvc;

namespace LMS.Presentation.Controllers;

public class ErrorController : Controller
{
    [Route("Error/{statusCode}")]
    public IActionResult Index(int statusCode)
    {
        Response.Clear();
        Response.StatusCode = statusCode;

        var model = new ErrorViewModel
        {
            ErrorMessage = statusCode switch
            {
                401 => "You don't have clearance for this memory file. Try logging in first!",
                403 => "That memory is locked away in a vault. Access denied!",
                404 => "What you're looking for may have been misplaced in Long Term Memory.",
                500 => "Oops! Headquarters got overwhelmed. We’re working to calm the emotions.",
                503 => "The memory workers are on a break. Please try again in a bit.",
                _ => "Looks like there’s a glitch in the system. Try again or come back later."
            },
            StatusCode = statusCode
        };

        return View("ErrorPage", model);
    }
}
