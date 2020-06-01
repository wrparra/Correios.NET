using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace Correios.NET.Api.Controllers
{
    [ApiController]
    [Route("encomendas")]
    public class PackageTrackingController : ControllerBase
    {
        private readonly Correios.NET.IServices _services;
        private readonly ILogger<PackageTrackingController> _logger;

        public PackageTrackingController(Correios.NET.IServices services, ILogger<PackageTrackingController> logger)
        {
            _services = services;
            _logger = logger;
        }

        [Route("{code}")]
        [HttpGet]
        public async Task<Correios.NET.Models.Package> Get(string code)
        {
            return await _services.GetPackageTrackingAsync(code);
        }        
    }
}
