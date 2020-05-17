using System.Collections.Generic;
using System.Threading.Tasks;
using Correios.NET.Models;

namespace Correios.NET
{
    public interface IServices
    {
        Task<Package> GetPackageTrackingAsync(string packageCode);
        Package GetPackageTracking(string packageCode);

        Task<IEnumerable<Address>> GetAddressesAsync(string zipCode);
        IEnumerable<Address> GetAddresses(string zipCode);        
    }
}