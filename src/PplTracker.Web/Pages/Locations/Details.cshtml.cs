using Microsoft.AspNetCore.Mvc.RazorPages;
using PplTracker.Core.DTOs;
using System.Net.Http.Json;

namespace PplTracker.Web.Pages.Locations;

public class DetailsModel : PageModel
{
    private readonly IHttpClientFactory _httpClientFactory;

    public DetailsModel(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public LocationDto? Location { get; set; }

    public async Task OnGetAsync(int id)
    {
        var client = _httpClientFactory.CreateClient("PplTrackerApi");
        try
        {
            Location = await client.GetFromJsonAsync<LocationDto>($"api/locations/{id}");
        }
        catch
        {
            Location = null;
        }
    }
}
