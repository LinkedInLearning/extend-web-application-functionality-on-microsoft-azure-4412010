using Azure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Wpm.Api.Dal;
using Wpm.Api.Domain;
using Wpm.Api.Models;

namespace Wpm.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PetsController : ControllerBase
{

    private readonly ILogger<PetsController> logger;
    private readonly WpmDbContext dbContext;

    public PetsController(ILogger<PetsController> logger,
        WpmDbContext dbContext)
    {
        this.logger = logger;
        this.dbContext = dbContext;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await dbContext
            .Pets
            .Select(p => new PetModel
            {
                Id = p.Id,
                Name = p.Name,
                Age = p.Age,
                Weight = p.Weight,
                PhotoUrl = p.PhotoUrl,
                BreedId = p.BreedId,
                Breed = new BreedModel()
                {
                    Id = p.Breed.Id,
                    Name = p.Breed.Name,
                    IdealMaxWeight = p.Breed.IdealMaxWeight,
                    SpeciesId = p.Breed.SpeciesId,
                    Species = new SpeciesModel()
                    {
                        Id = p.Breed.Species.Id,
                        Name = p.Breed.Species.Name
                    }
                },
                Owners = p
                    .Owners
                    .Select(o => new OwnerModel
                    {
                        Id = o.Id,
                        Name = o.Name
                    })
            })
            .AsNoTracking()
            .ToListAsync();

        return Ok(result);
    }


    [HttpGet("{id}")]
    public async Task<IActionResult> Get(int id)
    {
        var result = await dbContext
            .Pets
            .Select(p => new PetModel
            {
                Id = p.Id,
                Name = p.Name,
                Age = p.Age,
                Weight = p.Weight,
                PhotoUrl = p.PhotoUrl,
                BreedId = p.BreedId,
                Breed = new BreedModel() 
                    {  
                        Id = p.Breed.Id, 
                        Name = p.Breed.Name,
                        IdealMaxWeight = p.Breed.IdealMaxWeight,
                        SpeciesId = p.Breed.SpeciesId,
                        Species = new SpeciesModel()
                        {
                            Id = p.Breed.Species.Id,
                            Name = p.Breed.Species.Name
                        }
                    },
                Owners = p
                    .Owners
                    .Select(o => new OwnerModel
                    {
                        Id = o.Id,
                        Name = o.Name
                    })
            })
            .Where(p => p.Id == id)
            .AsNoTracking()
            .FirstOrDefaultAsync();

        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Add(PetCreateOrUpdateModel pet)
    {
        var newPet = new Pet()
        {
            Name = pet.Name,
            Age = pet.Age,
            Weight = pet.Weight,
            BreedId = pet.BreedId,
            PhotoUrl = pet.PhotoUrl
        };
        
        dbContext.Pets.Add(newPet);
        await dbContext.SaveChangesAsync();
        return Ok(pet);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, PetCreateOrUpdateModel pet)
    {
        var petToUpdate = dbContext.Pets
                        .Where(p => p.Id == id)
                        .FirstOrDefault();
        if (petToUpdate == null)
        {
            return BadRequest(id);
        }

        petToUpdate.Name = pet.Name;
        petToUpdate.Age = pet.Age;
        petToUpdate.BreedId = pet.BreedId;
        petToUpdate.Weight = pet.Weight;
        petToUpdate.PhotoUrl = pet.PhotoUrl;

        dbContext.Pets.Update(petToUpdate);
        await dbContext.SaveChangesAsync();
        return Ok(pet);
    }
}