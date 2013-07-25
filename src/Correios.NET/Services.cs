using System.Net.Http;
using System.Threading.Tasks;
using Correios.NET.Models;

namespace Correios.NET
{
    public class Services : IServices
    {
        private const string PACKAGE_TRACKING_URL =
            "http://websro.correios.com.br/sro_bin/txect01$.QueryList?P_LINGUA=001&P_TIPO=001&P_COD_UNI={0}";

        private readonly HttpClient _httpClient;

        public Services()
        {
            _httpClient = new HttpClient();
        }

        public async Task<Package> GetPackageTrackingAsync(string packageCode)
        {
            var url = string.Format(PACKAGE_TRACKING_URL, packageCode);
            var html = await _httpClient.GetStringAsync(url);
            return await Task.Run(() =>Package.Parse(html));
        }

        public Package GetPackageTracking(string packageCode)
        {
            return GetPackageTrackingAsync(packageCode).Result;
        }
    }
}
