using Hangfire;
using Hangfire.Storage;
using LMS.Application.Contracts.DTOs;
using LMS.Application.Contracts.Interfaces.Services;
using LMS.Common.Models;
using LMS.Core.Enums;
using LMS.Presentation.Attributes;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Globalization;

namespace LMS.Presentation.Controllers;

public class HomeController : Controller
{
    private readonly IServiceManager _serviceManager;

    public HomeController(IServiceManager serviceManager)
    {
        _serviceManager = serviceManager;
    }

    public IActionResult Index()
    {
        return View();
    }

    [HttpGet]
    [Route("home/user-genre-visualization")]
    [AuthorizeRoles(RoleListEnum.User)]
    public async Task<IActionResult> UserGenreVisualization(int? visualizingMonth = null)
    {
        var genreVisualization = await _serviceManager.HomeService.GenreVisualization(forLogin: true, visualizingMonth: visualizingMonth ?? DateTimeOffset.UtcNow.Month);
        var result = new
        {
            Labels = genreVisualization.Select(x => x.Label).ToArray(),
            Data = genreVisualization.Select(x => x.Data).ToArray()
        };

        return Json(ApiResponse.Success(data: result));
    }

    [HttpGet]
    [Route("home/general-genre-visualization")]
    [AuthorizeRoles(RoleListEnum.Admin)]
    public async Task<IActionResult> GlobalGenreVisualization(int? visualizingMonth = null)
    {
        var genreVisualization = await _serviceManager.HomeService.GenreVisualization(forLogin: false, visualizingMonth: visualizingMonth ?? DateTimeOffset.UtcNow.Month);
        var result = new
        {
            Labels = genreVisualization.Select(x => x.Label).ToArray(),
            Data = genreVisualization.Select(x => x.Data).ToArray()
        };

        return Json(ApiResponse.Success(data: result));
    }

    [HttpGet]
    [Route("home/user-dashboard")]
    [AuthorizeRoles(RoleListEnum.User)]
    public async Task<IActionResult> UserDashboard()
    {
        var months = Enumerable.Range(1, 12)
        .Select(m => new SelectListItem
        {
            Text = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(m),
            Value = m.ToString(),
            Selected = m == DateTimeOffset.UtcNow.Month,
            Disabled = m > DateTimeOffset.UtcNow.Month
        }).ToList();

        ViewBag.Months = months;

        var userDashboardData = await _serviceManager.HomeService.UserDashboardDataAync();

        return View(userDashboardData);
    }

    [HttpGet]
    [Route("home/librarian-dashboard")]
    [AuthorizeRoles(RoleListEnum.Librarian)]
    public async Task<IActionResult> LibrarianDashboard()
    {
        var librarianDashboardData = await _serviceManager.HomeService.LibrarianDashboardDataAync();
        return View(librarianDashboardData);
    }

    [HttpGet]
    [Route("home/admin-dashboard")]
    [AuthorizeRoles(RoleListEnum.Admin)]
    public async Task<IActionResult> AdminDashboard()
    {
        var months = Enumerable.Range(1, 12)
        .Select(m => new SelectListItem
        {
            Text = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(m),
            Value = m.ToString(),
            Selected = m == DateTimeOffset.UtcNow.Month,
            Disabled = m > DateTimeOffset.UtcNow.Month
        }).ToList();

        ViewBag.Months = months;

        var adminDashboardData = await _serviceManager.HomeService.AdminDashboardDataAync();
        return View(adminDashboardData);
    }

    [HttpGet]
    [Route("home/hangfire/job/health")]
    [AuthorizeRoles(RoleListEnum.Admin)]
    public IActionResult GetDetailedJobHealth()
    {
        var storageConnection = JobStorage.Current.GetConnection();
        var monitoringApi = JobStorage.Current.GetMonitoringApi();
        var recurringJobs = storageConnection.GetRecurringJobs();

        var istZone = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");

        var jobHealthList = recurringJobs.Select(job =>
        {
            var jobDetails = job.LastJobId != null
                ? monitoringApi.JobDetails(job.LastJobId)
                : null;

            var history = jobDetails?.History;
            var startedUtc = history?.FirstOrDefault(h => h.StateName == "Processing")?.CreatedAt;
            var endedUtc = history?.FirstOrDefault(h => h.StateName == "Succeeded")?.CreatedAt;

            var startedIst = startedUtc.HasValue
                ? TimeZoneInfo.ConvertTimeFromUtc(startedUtc.Value, istZone)
                : (DateTime?)null;

            var endedIst = endedUtc.HasValue
                ? TimeZoneInfo.ConvertTimeFromUtc(endedUtc.Value, istZone)
                : (DateTime?)null;

            var lastExecutionIst = job.LastExecution.HasValue
                ? TimeZoneInfo.ConvertTimeFromUtc(job.LastExecution.Value, istZone)
                : (DateTime?)null;

            var nextExecutionIst = job.NextExecution.HasValue
                ? TimeZoneInfo.ConvertTimeFromUtc(job.NextExecution.Value, istZone)
                : (DateTime?)null;

            return new JobHealthDto
            {
                JobId = job.Id,
                JobName = job.Id,
                LastExecution = lastExecutionIst,
                NextExecution = nextExecutionIst,
                LastState = jobDetails?.History?.LastOrDefault()?.StateName ?? "Not yet executed",
                ErrorMessage = job.Error,
                Duration = (startedIst.HasValue && endedIst.HasValue) ? endedIst - startedIst : null
            };
        });

        return View(jobHealthList);
    }

    [HttpPost]
    [Route("home/hangfire/run-job/{jobId}")]
    [AuthorizeRoles(RoleListEnum.Admin)]
    public IActionResult RunJob(string jobId)
    {
        RecurringJob.TriggerJob(jobId);
        return Ok("Job triggered");
    }
}
