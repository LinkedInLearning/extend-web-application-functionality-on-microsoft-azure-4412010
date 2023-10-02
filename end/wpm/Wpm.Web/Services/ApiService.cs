using Wpm.Web.Models;

namespace Wpm.Web.Services;

public class ApiService
{
    private readonly HttpClient httpClient;

    public ApiService(HttpClient httpClient)
    {
        this.httpClient = httpClient;
    }
    public async Task<IEnumerable<PetModel>> GetAllPetsAsync()
    {
        var all = await httpClient.GetFromJsonAsync<IEnumerable<PetModel>>("api/pets");
        return all;
    }

    public async Task<PetModel> GetPetAsync(int id)
    {
        var pet = await httpClient.GetFromJsonAsync<PetModel>($"api/pets/{id}");
        return pet;
    }

    public async Task<IEnumerable<BreedModel>> GetAllBreedsAsync()
    {
        var all = await httpClient.GetFromJsonAsync<IEnumerable<BreedModel>>("api/breeds");
        return all;
    }

    public async Task CreatePetAsync(PetModel pet)
    {
        var body = new PetCreateOrUpdateModel(pet.Name, pet.Age, pet.Weight, pet.PhotoUrl, pet.BreedId);
        await httpClient.PostAsJsonAsync($"api/pets", body);
    }

    public async Task UpdatePetAsync(int id, PetModel pet)
    {
        var body = new PetCreateOrUpdateModel(pet.Name, pet.Age, pet.Weight, pet.PhotoUrl, pet.BreedId);
        await httpClient.PutAsJsonAsync($"api/pets/{id}", body);
    }
}