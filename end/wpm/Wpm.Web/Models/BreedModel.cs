namespace Wpm.Web.Models;

public class BreedModel
{
    public int Id { get; set; }
    public string Name { get; set; }
    public decimal IdealMaxWeight { get; set; }
    public int SpeciesId { get; set; }
    public SpeciesModel Species { get; set; }
}