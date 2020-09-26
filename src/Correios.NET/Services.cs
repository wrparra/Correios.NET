using Correios.NET.Models;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Correios.NET
{
    public class Services : IServices
    {
        private const string PACKAGE_TRACKING_URL = "https://www2.correios.com.br/sistemas/rastreamento/ctrl/ctrlRastreamento.cfm";
        private const string ZIP_ADDRESS_URL = "http://www.buscacep.correios.com.br/sistemas/buscacep/resultadoBuscaCepEndereco.cfm";

        private readonly HttpClient _httpClient;

        public Services()
        {
            _httpClient = new HttpClient();
        }

        public async Task<Package> GetPackageTrackingAsync(string packageCode)
        {
            using (var response = await _httpClient.PostAsync(PACKAGE_TRACKING_URL, CreatePackageTrackingRequest(packageCode)))
            {
                var html = await response.Content.ReadAsStringAsync();
                return await Task.Run(() => Parser.ParsePackage(html));
            }
        }

        private FormUrlEncodedContent CreatePackageTrackingRequest(string packageCode)
        {
            var content = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("objetos", packageCode),
                new KeyValuePair<string, string>("p_tipo", "001"),
                new KeyValuePair<string, string>("p_lingua", "001")
            });
            return content;
        }

        public Package GetPackageTracking(string packageCode)
        {
            return GetPackageTrackingAsync(packageCode).Result;
        }


        public async Task<IEnumerable<Address>> GetAddressesAsync(string zipCode)
        {
            using (var response = await _httpClient.PostAsync(ZIP_ADDRESS_URL, CreateAddressRequest(zipCode)))
            {
                var html = await response.Content.ReadAsStringAsync();
                return await Task.Run(() => Parser.ParseAddresses(html));
            }
        }

        public IEnumerable<Address> GetAddresses(string zipCode)
        {
            using (var response = _httpClient.PostAsync(ZIP_ADDRESS_URL, CreateAddressRequest(zipCode)).Result)
            {
                var html = response.Content.ReadAsStringAsync().Result;
                return Parser.ParseAddresses(html);
            }
        }

        private FormUrlEncodedContent CreateAddressRequest(string zipCode)
        {
            var content = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("relaxation", zipCode),
                new KeyValuePair<string, string>("tipoCep", "ALL"),
                new KeyValuePair<string, string>("semelhante", "N")
            });
            return content;
        }
    }
}
