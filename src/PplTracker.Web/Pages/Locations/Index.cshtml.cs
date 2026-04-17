using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PplTracker.Core.DTOs;
using System.Net.Http.Json;

namespace PplTracker.Web.Pages.Locations;

public class IndexModel : PageModel
{
    private readonly IHttpClientFactory _httpClientFactory;

    public IndexModel(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public IList<LocationDto> Locations { get; set; } = new List<LocationDto>();

    [BindProperty(SupportsGet = true)]
    public string? Search { get; set; }

    public async Task OnGetAsync()
    {
        var client = _httpClientFactory.CreateClient("PplTrackerApi");
        try
        {
            if (!string.IsNullOrWhiteSpace(Search))
            {
                Locations = await client.GetFromJsonAsync<List<LocationDto>>($"api/locations/search?q={Uri.EscapeDataString(Search)}")
                            ?? new List<LocationDto>();
            }
            else
            {
                Locations = await client.GetFromJsonAsync<List<LocationDto>>("api/locations")
                            ?? new List<LocationDto>();
            }
        }
        catch
        {
            Locations = new List<LocationDto>();
        }
    }

    public async Task<IActionResult> OnPostDeleteAsync(int id)
    {
        var client = _httpClientFactory.CreateClient("PplTrackerApi");
        await client.DeleteAsync($"api/locations/{id}");
        return RedirectToPage();
    }
}
