using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Correios.NET.Api.Controllers
{
    [ApiController]
    [Route("cep")]
    public class ZipCodeController : ControllerBase
    {
        private readonly ICorreiosService _services;
        private readonly ILogger<ZipCodeController> _logger;

        public ZipCodeController(ICorreiosService services, ILogger<ZipCodeController> logger)
        {
            _services = services;
            _logger = logger;
        }

        [Route("{zipCode}")]
        [HttpGet]
        public async Task<IEnumerable<Correios.NET.Models.Address>> Get(string zipCode)
        {
            return await _services.GetAddressesAsync(zipCode);
        }
    }
}
