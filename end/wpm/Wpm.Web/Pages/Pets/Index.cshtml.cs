using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Wpm.Web.Models;
using Wpm.Web.Services;

namespace Wpm.Web.Pages.Pets;

[Authorize]
public class IndexModel : PageModel
{
    private readonly ApiService apiService;

    public IEnumerable<PetModel>? Pets { get; private set; }

    [BindProperty(SupportsGet = true)]
    public string? Search { get; set; }

    public IndexModel(ApiService apiService)
    {
        this.apiService = apiService;
    }
    public async Task OnGet()
    {
        var pets = await apiService.GetAllPetsAsync();
        Pets = string.IsNullOrWhiteSpace(Search) ?
            pets : pets.Where(p => p.Name.ToLower().Contains(Search));
    }
}