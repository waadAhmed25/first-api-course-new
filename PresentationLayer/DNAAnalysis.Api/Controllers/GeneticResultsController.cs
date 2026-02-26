using DNAAnalysis.Services.Abstraction;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DNAAnalysis.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GeneticResultsController : ControllerBase
    {
        private readonly IGeneticResultService _resultService;

        public GeneticResultsController(IGeneticResultService resultService)
        {
            _resultService = resultService;
        }

        [Authorize]
        [HttpGet("{requestId}")]
        public async Task<IActionResult> GetResult(int requestId)
        {
            var result = await _resultService.GetResultByRequestIdAsync(requestId);

            if (result == null)
                return NotFound();

            return Ok(result);
        }
    }
}