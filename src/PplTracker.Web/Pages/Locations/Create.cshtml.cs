using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PplTracker.Core.DTOs;
using System.Net.Http.Json;

namespace PplTracker.Web.Pages.Locations;

public class CreateModel : PageModel
{
    private readonly IHttpClientFactory _httpClientFactory;

    public CreateModel(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    [BindProperty]
    public CreateLocationDto Input { get; set; } = new();

    public string? ErrorMessage { get; set; }

    public void OnGet() { }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid) return Page();

        var client = _httpClientFactory.CreateClient("PplTrackerApi");
        var response = await client.PostAsJsonAsync("api/locations", Input);

        if (response.IsSuccessStatusCode)
            return RedirectToPage("Index");

        ErrorMessage = "Failed to save location. Please try again.";
        return Page();
    }
}
