using LMS.Common.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace LMS.Application.Contracts.DTOs;

public class PaginatedResponseDto<T> where T : class
{
    public PaginationModel Pagination { get; set; } = null!;
    public Dictionary<string, List<SelectListItem>?> Selections { get; set; } = new Dictionary<string, List<SelectListItem>?>();
    public List<T> Data { get; set; } = new List<T>();
    public List<T> Data1 { get; set; } = new List<T>();
}
