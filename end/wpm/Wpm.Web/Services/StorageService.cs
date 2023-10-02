using Azure.Storage.Blobs;

namespace Wpm.Web.Services;

public class StorageService
{
    private readonly IConfiguration configuration;

    public StorageService(IConfiguration configuration)
    {
        this.configuration = configuration;
    }
    public async Task<string> UploadAsync(Stream stream, string fileName)
    {
        stream.Position = 0;
        var connectionString = configuration.GetConnectionString("WpmStorage");
        var containerClient = new BlobContainerClient(connectionString, "photos");
        var blobClient = containerClient.GetBlobClient(fileName);
        await blobClient.UploadAsync(stream, true);
        return blobClient.Uri.ToString();
    }
}
