using DatabaseComparison.ProjectionsImplementation;
using Microsoft.AspNetCore.Mvc;

namespace DatabaseComparison.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ProjectionLatency : ControllerBase
{
    [HttpGet]
    public IActionResult Index()
    {
        return Ok(MemoryCollection.GetAverageTime());
    }
}