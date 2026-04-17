using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PplTracker.Core.DTOs;
using System.Net.Http.Json;

namespace PplTracker.Web.Pages.Schedules;

public class IndexModel : PageModel
{
    private readonly IHttpClientFactory _httpClientFactory;

    public IndexModel(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public IList<ScheduleDto> Schedules { get; set; } = new List<ScheduleDto>();

    public async Task OnGetAsync()
    {
        var client = _httpClientFactory.CreateClient("PplTrackerApi");
        try
        {
            Schedules = await client.GetFromJsonAsync<List<ScheduleDto>>("api/schedules")
                        ?? new List<ScheduleDto>();
        }
        catch
        {
            Schedules = new List<ScheduleDto>();
        }
    }

    public async Task<IActionResult> OnPostDeleteAsync(int id)
    {
        var client = _httpClientFactory.CreateClient("PplTrackerApi");
        await client.DeleteAsync($"api/schedules/{id}");
        return RedirectToPage();
    }
}
