namespace Wpm.Web.Models;

public class PetModel
{
    public int Id { get; set; }
    public string Name { get; set; }
    public int? Age { get; set; }
    public decimal? Weight { get; set; }
    public string? PhotoUrl { get; set; }
    public int BreedId { get; set; }
    public BreedModel Breed { get; set; }
    public IEnumerable<OwnerModel> Owners { get; set; }
        = Array.Empty<OwnerModel>();
}