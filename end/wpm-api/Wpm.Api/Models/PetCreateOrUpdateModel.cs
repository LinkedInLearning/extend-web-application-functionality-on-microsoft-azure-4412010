namespace Wpm.Api.Models;

public record PetCreateOrUpdateModel(string Name,
                        int? Age,
                        decimal? Weight,
                        string? PhotoUrl,
                        int BreedId);