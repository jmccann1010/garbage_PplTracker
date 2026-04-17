using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PplTracker.Core.DTOs;
using System.Net.Http.Json;

namespace PplTracker.Web.Pages.Locations;

public class EditModel : PageModel
{
    private readonly IHttpClientFactory _httpClientFactory;

    public EditModel(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    [BindProperty]
    public UpdateLocationDto Input { get; set; } = new();

    public bool IsNotFound { get; set; }
    public string? ErrorMessage { get; set; }

    public async Task OnGetAsync(int id)
    {
        var client = _httpClientFactory.CreateClient("PplTrackerApi");
        var location = await client.GetFromJsonAsync<LocationDto>($"api/locations/{id}");
        if (location == null)
        {
            IsNotFound = true;
            return;
        }

        Input = new UpdateLocationDto
        {
            Name = location.Name,
            Address = location.Address,
            City = location.City,
            State = location.State,
            ZipCode = location.ZipCode,
            Country = location.Country,
            Latitude = location.Latitude,
            Longitude = location.Longitude,
            Notes = location.Notes
        };
    }

    public async Task<IActionResult> OnPostAsync(int id)
    {
        if (!ModelState.IsValid) return Page();

        var client = _httpClientFactory.CreateClient("PplTrackerApi");
        var response = await client.PutAsJsonAsync($"api/locations/{id}", Input);

        if (response.IsSuccessStatusCode)
            return RedirectToPage("Index");

        ErrorMessage = "Failed to update location. Please try again.";
        return Page();
    }
}
