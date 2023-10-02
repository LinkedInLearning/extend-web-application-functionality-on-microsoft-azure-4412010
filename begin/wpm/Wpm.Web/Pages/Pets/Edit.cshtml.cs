using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Wpm.Web.Dal;
using Wpm.Web.Domain;
using Wpm.Web.Services;

namespace Wpm.Web.Pages.Pets;

public class EditModel : PageModel
{
    private readonly WpmDbContext dbContext;
    private readonly StorageService storageService;

    [BindProperty]
    public Pet? Pet { get; set; }

    [BindProperty]
    public IFormFile FileUpload { get; set; }

    public SelectList? Breeds { get; set; }

    public EditModel(WpmDbContext dbContext, StorageService storageService)
    {
        this.dbContext = dbContext;
        this.storageService = storageService;

        var breeds = dbContext
            .Breeds
            .Select(b => new SelectListItem(b.Name, b.Id.ToString())).ToList();
        Breeds = new SelectList(breeds, "Value", "Text");
    }
    public void OnGet(int id)
    {
        Pet = dbContext.Pets
                    .Where(p => p.Id == id)
                    .First();
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

        dbContext.Update(Pet);
        await dbContext.SaveChangesAsync();
        return RedirectToPage("Index");
    }
}