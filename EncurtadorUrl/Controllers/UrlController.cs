using EncurtadorUrl.Models.DTOs;
using EncurtadorUrl.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace EncurtadorUrl.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UrlController : ControllerBase
    {
        private readonly IUrlShortService _urlshortService;

        public UrlController(IUrlShortService urlShortService)
        {
            _urlshortService = urlShortService;
        }

        [HttpPost("shorten")]
        [EnableRateLimiting("fixo")]
        public async Task<IActionResult> ShortenUrl([FromBody] UrlRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Url) || !Uri.TryCreate(request.Url, UriKind.Absolute, out Uri? validatedUri))
            {
                return BadRequest("Formato de URL inválido.");
            }

            string shortCode = await _urlshortService.CreateShortUrlAsync(request.Url);

            return Ok(new { Code = shortCode });
        }

        [HttpGet("code")]
        public async Task<IActionResult> RedirectUrlOriginal(string code)
        {
            string? originalUrl = await _urlshortService.GetOriginalUrlAsync(code);

            if (originalUrl == null)
            {
                return NotFound("Código de redirecionamento não encontrado.");
            }

            // Retorna status 302(Found) mudando o navegador para a página
            return Redirect(originalUrl);
        }
    }
}
