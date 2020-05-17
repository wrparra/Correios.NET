using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Correios.NET.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Correios.NET.Api.Controllers
{
    [ApiController]
    [Route("cep")]
    public class ZipCodeController : ControllerBase
    {
        private readonly Correios.NET.IServices _services;
        private readonly ILogger<ZipCodeController> _logger;

        public ZipCodeController(Correios.NET.IServices services, ILogger<ZipCodeController> logger)
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
