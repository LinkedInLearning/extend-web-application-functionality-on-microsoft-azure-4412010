using Azure.Storage.Blobs;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Sql;
using Microsoft.Extensions.Logging;
using PuppeteerSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace PdfCreator;

public record Pet (int Id, string Name, int Age, double Weight, string PhotoUrl);

public static class PdfCreator
{

    private static HttpClient client = new();

[FunctionName(nameof(PdfCreator))]
    public static async Task Run(
        [SqlTrigger("dbo.Pets", "Wpm")] IReadOnlyList<SqlChange<Pet>> changes,
        [Blob("records", FileAccess.Write, Connection = "WpmStorage")] BlobContainerClient blobContainerClient,
        ILogger logger)
    {
        foreach (var change in changes)
        {
            var pet = change.Item;
            logger.LogInformation($"{change.Operation}");
            logger.LogInformation($"Id: {pet.Id} Name: {pet.Name}");

            var imageBytes = await client.GetByteArrayAsync(pet.PhotoUrl);

            var html = GenerateHtmlContentForPet(pet.Name, pet.Age, 
                pet.Weight, imageBytes);

            var pdf = await ConvertHtmlToPdfAsync(html);

            var blobName = $"{pet.Name}.pdf";
            blobContainerClient.DeleteBlobIfExists(blobName);
            blobContainerClient.UploadBlob(blobName, pdf);

            logger.LogInformation(html);
            logger.LogInformation(pdf.Length.ToString());
        }
    }

    private static string GenerateHtmlContentForPet(string name,
                                                    int age,
                                                    double weight,
                                                    byte[] imageData)
    {
        var mainTitle = "Wisdom Pet Medicine";
        var petTitle = $"{name}, {age} years old";

        var imageBase64 = Convert.ToBase64String(imageData);

        var imageDataUri = $"data:image/jpeg;base64,{imageBase64}";
        var logoDataUri = WpmHelpers.GetLogo();

        var html = $@"
        <!DOCTYPE html>
        <html>
        <head>
            <title>{mainTitle}</title>
            <style>
                body {{
                    font-family: Arial, sans-serif;
                }}
                h1 {{
                    color: #402b22;
                    font-size: 3em;
                }}
                h2 {{
                    color: #916544;
                    font-size: 1.5em;
                    font-style: italic;
                }}
                .info {{
                    margin: 1em 0;
                    font-size: 1.2em;
                    text-align: left; 
                    padding: 10px;
                }}
                img {{
                    max-width: 200px;
                    height: auto;
                }}
                table {{
                    max-width: 50%;
                }}
                td {{
                    vertical-align: top;
                }}
                .main-title {{
                    display: flex;
                    align-items: center;
                }}
                .main-title img {{
                    max-width: 100px;
                    height: auto;
                    margin-right: 10px;
                }}
            </style>            
        </head>
        <body>
            <div class='main-title'>
                <img src='{logoDataUri}' alt='Wisdom Pet Medicine'>
                <h1>{mainTitle}</h1>
            </div>
            <h2>{petTitle}</h2>
            <table>
                <tr>
                    <td>
                        <img src='{imageDataUri}' alt='{name}'>
                    </td>
                    <td>
                        <div class='info'>
                            <p><strong>Age:</strong> {age} years</p>
                            <p><strong>Weight:</strong> {weight} kg</p>
                        </div>
                    </td>
                </tr>
            </table>
        </body>
        </html>";

        return html;
    }

    private static async Task<Stream> ConvertHtmlToPdfAsync(string html)
    {
        var browserFetcherOptions = new BrowserFetcherOptions()
        {
            Path = Path.GetTempPath()
        };
        using var browserFetcher = new BrowserFetcher(browserFetcherOptions);
        await browserFetcher.DownloadAsync(BrowserFetcher.DefaultChromiumRevision);

        var launchOptions = new LaunchOptions()
        {
            Headless = true,
            ExecutablePath = browserFetcher
            .RevisionInfo(BrowserFetcher.DefaultChromiumRevision.ToString()).ExecutablePath
        };
        using var browser = await Puppeteer.LaunchAsync(launchOptions);
        using var page = await browser.NewPageAsync();
        await page.SetContentAsync(html);
        var pdfOptions = new PdfOptions() { Format = PuppeteerSharp.Media.PaperFormat.Letter };
        var result = await page.PdfStreamAsync(pdfOptions);
        return result;
    }
}