using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PplTracker.Core.DTOs;
using System.Net.Http.Json;

namespace PplTracker.Web.Pages.People;

public class EditModel : PageModel
{
    private readonly IHttpClientFactory _httpClientFactory;

    public EditModel(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    [BindProperty]
    public UpdatePersonDto Input { get; set; } = new();

    public bool IsNotFound { get; set; }
    public string? ErrorMessage { get; set; }

    public async Task OnGetAsync(int id)
    {
        var client = _httpClientFactory.CreateClient("PplTrackerApi");
        var person = await client.GetFromJsonAsync<PersonDto>($"api/people/{id}");
        if (person == null)
        {
            IsNotFound = true;
            return;
        }

        Input = new UpdatePersonDto
        {
            FirstName = person.FirstName,
            LastName = person.LastName,
            Email = person.Email,
            Phone = person.Phone,
            Notes = person.Notes
        };
    }

    public async Task<IActionResult> OnPostAsync(int id)
    {
        if (!ModelState.IsValid) return Page();

        var client = _httpClientFactory.CreateClient("PplTrackerApi");
        var response = await client.PutAsJsonAsync($"api/people/{id}", Input);

        if (response.IsSuccessStatusCode)
            return RedirectToPage("Index");

        ErrorMessage = "Failed to update person. Please try again.";
        return Page();
    }
}
