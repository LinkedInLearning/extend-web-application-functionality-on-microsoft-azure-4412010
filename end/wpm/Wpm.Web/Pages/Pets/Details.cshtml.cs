using Microsoft.AspNetCore.Mvc.RazorPages;
using Wpm.Web.Models;
using Wpm.Web.Services;

namespace Wpm.Web.Pages.Pets;

public class DetailsModel : PageModel
{
    private readonly ApiService apiService;

    public PetModel? Pet { get; set; }
    public DetailsModel(ApiService apiService)
    {
        this.apiService = apiService;
    }
    public async Task OnGetAsync(int? id)
    {
        if (id == null)
        {
            return;
        }
        var pet = await apiService.GetPetAsync(id.Value);
        Pet = pet;
    }
}