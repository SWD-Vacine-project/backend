using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Vaccine.API.Models.OcrModel;
using Vaccine.API.Services;


namespace Vaccine.API.Controllers
{
    [EnableCors("MyPolicy")]
    [Route("[controller]")]
    [ApiController]
    public class OcrController : ControllerBase
    {
        private readonly IOcrService _ocrService;

        public OcrController(IOcrService ocrService)
        {
            _ocrService = ocrService;
        }

        [HttpPost("extract")]
        public async Task<IActionResult> ExtractText([FromForm] OcrRequest request)
        {
            if (request.Image == null || request.Image.Length == 0)
                return BadRequest("No file uploaded.");

            using var stream = request.Image.OpenReadStream();
            var text = _ocrService.ExtractText(stream);
            return Ok(new { text });
        }
    }
}
