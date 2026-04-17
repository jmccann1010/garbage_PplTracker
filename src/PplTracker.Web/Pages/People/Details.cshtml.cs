using Microsoft.AspNetCore.Mvc.RazorPages;
using PplTracker.Core.DTOs;
using System.Net.Http.Json;

namespace PplTracker.Web.Pages.People;

public class DetailsModel : PageModel
{
    private readonly IHttpClientFactory _httpClientFactory;

    public DetailsModel(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public PersonDto? Person { get; set; }

    public async Task OnGetAsync(int id)
    {
        var client = _httpClientFactory.CreateClient("PplTrackerApi");
        try
        {
            Person = await client.GetFromJsonAsync<PersonDto>($"api/people/{id}");
        }
        catch
        {
            Person = null;
        }
    }
}
