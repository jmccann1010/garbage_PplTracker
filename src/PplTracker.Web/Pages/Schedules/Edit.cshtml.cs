using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using PplTracker.Core.DTOs;
using System.Net.Http.Json;

namespace PplTracker.Web.Pages.Schedules;

public class EditModel : PageModel
{
    private readonly IHttpClientFactory _httpClientFactory;

    public EditModel(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    [BindProperty]
    public UpdateScheduleDto Input { get; set; } = new();

    public bool IsNotFound { get; set; }
    public string? ErrorMessage { get; set; }
    public SelectList PeopleOptions { get; set; } = new SelectList(Enumerable.Empty<object>());
    public SelectList LocationsOptions { get; set; } = new SelectList(Enumerable.Empty<object>());

    public async Task OnGetAsync(int id)
    {
        var client = _httpClientFactory.CreateClient("PplTrackerApi");
        var schedule = await client.GetFromJsonAsync<ScheduleDto>($"api/schedules/{id}");
        if (schedule == null)
        {
            IsNotFound = true;
            return;
        }

        Input = new UpdateScheduleDto
        {
            Title = schedule.Title,
            Description = schedule.Description,
            StartTime = schedule.StartTime.ToLocalTime(),
            EndTime = schedule.EndTime?.ToLocalTime(),
            IsAllDay = schedule.IsAllDay,
            PersonId = schedule.PersonId,
            LocationId = schedule.LocationId
        };

        await LoadSelectListsAsync();
    }

    public async Task<IActionResult> OnPostAsync(int id)
    {
        if (!ModelState.IsValid)
        {
            await LoadSelectListsAsync();
            return Page();
        }

        var client = _httpClientFactory.CreateClient("PplTrackerApi");
        var response = await client.PutAsJsonAsync($"api/schedules/{id}", Input);

        if (response.IsSuccessStatusCode)
            return RedirectToPage("Index");

        ErrorMessage = "Failed to update event. Please try again.";
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
