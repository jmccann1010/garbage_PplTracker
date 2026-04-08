using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using PplTracker.Core.DTOs;
using System.Net.Http.Json;

namespace PplTracker.Web.Pages.Tracking;

public class CheckInModel : PageModel
{
    private readonly IHttpClientFactory _httpClientFactory;

    public CheckInModel(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    [BindProperty]
    public CreatePersonLocationDto Input { get; set; } = new();

    public string? ErrorMessage { get; set; }
    public SelectList PeopleOptions { get; set; } = new SelectList(Enumerable.Empty<object>());
    public SelectList LocationsOptions { get; set; } = new SelectList(Enumerable.Empty<object>());

    public async Task OnGetAsync()
    {
        await LoadSelectListsAsync();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            await LoadSelectListsAsync();
            return Page();
        }

        Input.CheckInTime = DateTime.UtcNow;
        var client = _httpClientFactory.CreateClient("PplTrackerApi");
        var response = await client.PostAsJsonAsync("api/personlocations", Input);

        if (response.IsSuccessStatusCode)
            return RedirectToPage("Index");

        ErrorMessage = "Failed to check in. Please try again.";
        await LoadSelectListsAsync();
        return Page();
    }

    private async Task LoadSelectListsAsync()
    {
        var client = _httpClientFactory.CreateClient("PplTrackerApi");
        try
        {
            var people = await client.GetFromJsonAsync<List<PersonDto>>("api/people") ?? new();
            PeopleOptions = new SelectList(people, "Id", "FullName");

            var locations = await client.GetFromJsonAsync<List<LocationDto>>("api/locations") ?? new();
            LocationsOptions = new SelectList(locations, "Id", "Name");
        }
        catch
        {
            PeopleOptions = new SelectList(Enumerable.Empty<object>());
            LocationsOptions = new SelectList(Enumerable.Empty<object>());
        }
    }
}
