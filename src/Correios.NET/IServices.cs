using System;
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


        Task<IEnumerable<DeliveryPrice>> GetDeliveryPricesAsync(DateTime postDate, string originalZipCode, string deliveryZipCode, DeliveryOptions deliveryOptions,
            int height, int width, int length, float weight);

        IEnumerable<DeliveryPrice> GetDeliveryPrices(DateTime postDate, string originalZipCode, string deliveryZipCode, DeliveryOptions deliveryOptions,
            int height, int width, int length, float weight);
    }
}