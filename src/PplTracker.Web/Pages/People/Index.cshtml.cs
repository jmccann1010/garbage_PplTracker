using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PplTracker.Core.DTOs;
using System.Net.Http.Json;

namespace PplTracker.Web.Pages.People;

public class IndexModel : PageModel
{
    private readonly IHttpClientFactory _httpClientFactory;

    public IndexModel(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public IList<PersonDto> People { get; set; } = new List<PersonDto>();

    [BindProperty(SupportsGet = true)]
    public string? Search { get; set; }

    public async Task OnGetAsync()
    {
        var client = _httpClientFactory.CreateClient("PplTrackerApi");
        try
        {
            if (!string.IsNullOrWhiteSpace(Search))
            {
                People = await client.GetFromJsonAsync<List<PersonDto>>($"api/people/search?q={Uri.EscapeDataString(Search)}")
                         ?? new List<PersonDto>();
            }
            else
            {
                People = await client.GetFromJsonAsync<List<PersonDto>>("api/people")
                         ?? new List<PersonDto>();
            }
        }
        catch
        {
            People = new List<PersonDto>();
        }
    }

    public async Task<IActionResult> OnPostDeleteAsync(int id)
    {
        var client = _httpClientFactory.CreateClient("PplTrackerApi");
        await client.DeleteAsync($"api/people/{id}");
        return RedirectToPage();
    }
}
