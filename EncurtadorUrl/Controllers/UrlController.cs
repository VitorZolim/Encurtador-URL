using EncurtadorUrl.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

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
    }
}
