using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Wpm.Web.Models;
using Wpm.Web.Services;

namespace Wpm.Web.Pages.Pets;

public class CreateModel : PageModel
{
    private readonly ApiService apiService;

    [BindProperty]
    public PetModel? Pet { get; set; }

    public SelectList? Breeds { get; set; }

    public CreateModel(ApiService apiService)
    {
        this.apiService = apiService;
    }

    public async Task OnGetAsync()
    {
        var breeds = await apiService.GetAllBreedsAsync();
        var items = breeds
            .Select(b => new SelectListItem(b.Name, b.Id.ToString()))
            .ToList();
        Breeds = new SelectList(items, "Value", "Text");
    }

    public async Task<IActionResult> OnPostAsync()
    {
        await apiService.CreatePetAsync(Pet);
        return RedirectToPage("Index");
    }
}