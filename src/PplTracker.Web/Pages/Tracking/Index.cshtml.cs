using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PplTracker.Core.DTOs;
using System.Net.Http.Json;

namespace PplTracker.Web.Pages.Tracking;

public class IndexModel : PageModel
{
    private readonly IHttpClientFactory _httpClientFactory;

    public IndexModel(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public IList<PersonLocationDto> CurrentlyCheckedIn { get; set; } = new List<PersonLocationDto>();
    public IList<PersonLocationDto> AllCheckIns { get; set; } = new List<PersonLocationDto>();

    public async Task OnGetAsync()
    {
        var client = _httpClientFactory.CreateClient("PplTrackerApi");
        try
        {
            CurrentlyCheckedIn = await client.GetFromJsonAsync<List<PersonLocationDto>>("api/personlocations/current")
                                  ?? new List<PersonLocationDto>();
            AllCheckIns = await client.GetFromJsonAsync<List<PersonLocationDto>>("api/personlocations")
                          ?? new List<PersonLocationDto>();
        }
        catch
        {
            CurrentlyCheckedIn = new List<PersonLocationDto>();
            AllCheckIns = new List<PersonLocationDto>();
        }
    }

    public async Task<IActionResult> OnPostCheckOutAsync(int id)
    {
        var client = _httpClientFactory.CreateClient("PplTrackerApi");
        await client.PutAsync($"api/personlocations/{id}/checkout", null);
        return RedirectToPage();
    }
}
