using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Wpm.Api.Dal;

namespace Wpm.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BreedsController : ControllerBase
{
    private readonly WpmDbContext dbContext;
    private readonly ILogger<BreedsController> logger;

    public BreedsController(WpmDbContext dbContext, ILogger<BreedsController> logger)
    {
        this.dbContext = dbContext;
        this.logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var breeds = await dbContext.Breeds.ToListAsync();
        return Ok(breeds);
    }
}