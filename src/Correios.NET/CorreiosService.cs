using Correios.NET.Extensions;
using Correios.NET.Models;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Correios.NET
{
    public class CorreiosService : ICorreiosService
    {
        private const string PACKAGE_TRACKING_URL = "https://www.linkcorreios.com.br";
        private const string ZIP_ADDRESS_URL = "https://buscacepinter.correios.com.br/app/endereco/carrega-cep-endereco.php";

        private readonly HttpClient _httpClient;

        public CorreiosService()
        {
            _httpClient = new HttpClient();
        }

        #region Packages

        public async Task<Package> GetPackageTrackingAsync(string packageCode)
        {
            var url = $"{PACKAGE_TRACKING_URL}/?id={packageCode}";
            var requestMessage = new HttpRequestMessage(HttpMethod.Post, url);
            var response = await _httpClient.SendAsync(requestMessage);
            var html = await response.Content.ReadAsStringAsync();
            return Parser.ParsePackage(html);
        }

        public Package GetPackageTracking(string packageCode)
        {
            return GetPackageTrackingAsync(packageCode).RunSync();
        }

        #endregion

        #region ZipCodes

        private FormUrlEncodedContent CreateZipCodeRequest(string zipCode)
        {
            var content = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("endereco", zipCode),
                new KeyValuePair<string, string>("tipoCEP", "ALL")

            });
            return content;
        }

        public async Task<IEnumerable<Address>> GetAddressesAsync(string zipCode)
        {
            var request = CreateZipCodeRequest(zipCode);
            var response = await _httpClient.PostAsync(ZIP_ADDRESS_URL, request);

            var jsonResponse = await response.Content.ReadAsStringAsync();
            var correiosAddressResponse = JsonConvert.DeserializeObject<CorreiosAddresResponse>(jsonResponse);
            if (correiosAddressResponse != null && !correiosAddressResponse.Erro)
            {
                return correiosAddressResponse.Dados.Select(a => new Address
                {
                    Street = a.LogradouroDNEC,
                    District = a.Bairro,
                    City = a.Localidade,
                    State = a.Uf,
                    ZipCode = a.Cep
                });
            }

            return null;
        }

        public IEnumerable<Address> GetAddresses(string zipCode)
        {
            return GetAddressesAsync(zipCode).RunSync();
        }

        #endregion
    }
}
