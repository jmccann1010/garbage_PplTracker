using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace PplTracker.Web.Pages.Locations;

public class DeleteModel : PageModel
{
    private readonly IHttpClientFactory _httpClientFactory;

    public DeleteModel(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public async Task<IActionResult> OnPostAsync(int id)
    {
        var client = _httpClientFactory.CreateClient("PplTrackerApi");
        await client.DeleteAsync($"api/locations/{id}");
        return RedirectToPage("Index");
    }
}
