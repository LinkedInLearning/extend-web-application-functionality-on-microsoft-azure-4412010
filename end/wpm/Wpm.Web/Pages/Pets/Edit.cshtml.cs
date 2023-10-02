using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Wpm.Web.Models;
using Wpm.Web.Services;

namespace Wpm.Web.Pages.Pets;

public class EditModel : PageModel
{
    private readonly ApiService apiService;
    private readonly StorageService storageService;

    [BindProperty]
    public PetModel? Pet { get; set; }

    [BindProperty]
    public IFormFile FileUpload { get; set; }

    public SelectList? Breeds { get; set; }

    public EditModel(ApiService apiService, StorageService storageService)
    {
        this.apiService = apiService;
        this.storageService = storageService;
    }
    public async Task OnGetAsync(int id)
    {
        var breeds = await apiService.GetAllBreedsAsync();
        var items = breeds
                    .Select(b => new SelectListItem(b.Name, b.Id.ToString()))
                    .ToList();
        Breeds = new SelectList(items, "Value", "Text");

        var pet = await apiService.GetPetAsync(id);
        Pet = pet;
    }

    public async Task<IActionResult> OnPost()
    {
        if (FileUpload != null)
        {
            var fileStream = new MemoryStream();
            FileUpload.CopyTo(fileStream);

            var result 
                = await storageService.UploadAsync(fileStream, FileUpload.FileName);
            Pet.PhotoUrl = result;
        }

        await apiService.UpdatePetAsync(Pet.Id, Pet);
        return RedirectToPage("Index");
    }
}