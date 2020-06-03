using AngleSharp.Dom;
using Correios.NET.Attributes;
using Correios.NET.Exceptions;
using Correios.NET.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Correios.NET
{
    public class Services : IServices
    {
        private const string PACKAGE_TRACKING_URL = "http://sro.micropost.com.br/consulta.php?objetos={0}";
        private const string ZIP_ADDRESS_URL = "http://www.buscacep.correios.com.br/sistemas/buscacep/resultadoBuscaCepEndereco.cfm";
        private const string DELIVERY_PRICES_ADDRESS_URL = "http://www.buscacep.correios.com.br/sistemas/precosPrazos/prazos.cfm";


        private readonly HttpClient _httpClient;

        public Services()
        {
            _httpClient = new HttpClient();
        }

        public async Task<Package> GetPackageTrackingAsync(string packageCode)
        {
            var url = string.Format(PACKAGE_TRACKING_URL, packageCode);
            var response = await _httpClient.GetByteArrayAsync(url);
            var html = Encoding.GetEncoding("ISO-8859-1").GetString(response, 0, response.Length - 1);
            return await Task.Run(() => Parser.ParsePackage(html));
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



        public async Task<IEnumerable<DeliveryPrice>> GetDeliveryPricesAsync(DateTime postDate, string originalZipCode, string deliveryZipCode, DeliveryOptions deliveryOptions,
            int height, int width, int length, float weight)
        {
            originalZipCode = originalZipCode.Replace(".", "").Replace("-", "");
            deliveryZipCode = originalZipCode.Replace(".", "").Replace("-", "");

            if (height < 2 || height > 100)
                throw new PackageSizeException("A altura da caixa deve ter no mínimo 2cm e no máximo 100cm");

            if (width < 11 || width > 100)
                throw new PackageSizeException("A largura da caixa deve ter no mínimo 11cm e no máximo 100cm");

            if (length < 16 || length > 100)
                throw new PackageSizeException("O comprimento da caixa deve ter no mínimo 16cm e no máximo 100cm");

            if (height + width + length > 200)
                throw new PackageSizeException("A soma resultante do comprimento + largura + altura não deve superar 200 cm");


            if (weight < 0.3f)
                weight = 0.3f;
            else if (weight > 0.3f && weight < 1)
                weight = 1f;
            else
                weight = (float)Math.Ceiling(weight);

            Array values = Enum.GetValues(typeof(DeliveryOptions));
            List<DeliveryPrice> deliveryPrices = new List<DeliveryPrice>();

            foreach (DeliveryOptions option in values)
            {
                if (deliveryOptions.HasFlag(option))
                {

                    var body = CreateDeliveryPriceRequest(postDate, originalZipCode, deliveryZipCode, option, height, width, length, weight);

                    using (var response = await _httpClient.PostAsync(DELIVERY_PRICES_ADDRESS_URL, body))
                    {
                        var html = response.Content.ReadAsStringAsync().Result;
                        var price = Parser.ParseDeliveryPrices(option.GetAttributeDescription(), html);

                        deliveryPrices.Add(price);
                    }
                }
            }

            return deliveryPrices;


        }

        public IEnumerable<DeliveryPrice> GetDeliveryPrices(DateTime postDate, string originalZipCode, string deliveryZipCode, DeliveryOptions deliveryOptions,
            int height, int width, int length, float weight)
        {
            return GetDeliveryPricesAsync(postDate, originalZipCode, deliveryZipCode, deliveryOptions, height, width, length, weight).Result;
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

        private FormUrlEncodedContent CreateDeliveryPriceRequest(DateTime postDate, string originalZipCode, string deliveryZipCode, DeliveryOptions deliveryOption,
            int height, int width, int length, float weight)
        {
            string deliveryOptionValue = deliveryOption.GetAttributeValue();

            var content = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("data", postDate.ToString("dd/MM/yyyy")),
                new KeyValuePair<string, string>("dataAtual", DateTime.Now.ToString("dd/MM/yyyy")),
                new KeyValuePair<string, string>("cepOrigem", originalZipCode),
                new KeyValuePair<string, string>("cepDestino", deliveryZipCode),

                new KeyValuePair<string, string>("servico", deliveryOptionValue),

                new KeyValuePair<string, string>("embalagem1", "outraEmbalagem1"), //TODO: Give to user the options
                
                new KeyValuePair<string, string>("Altura", height.ToString()),
                new KeyValuePair<string, string>("Largura", width.ToString()),
                new KeyValuePair<string, string>("Comprimento", length.ToString()),
                new KeyValuePair<string, string>("peso", weight.ToString()),


                new KeyValuePair<string, string>("Selecao", ""),
                new KeyValuePair<string, string>("embalagem2", ""),
                new KeyValuePair<string, string>("Selecao1", ""),
                new KeyValuePair<string, string>("proCod_in_1", ""),
                new KeyValuePair<string, string>("nomeEmbalagemCaixa", ""),
                new KeyValuePair<string, string>("TipoEmbalagem1", ""),
                new KeyValuePair<string, string>("Selecao2", ""),
                new KeyValuePair<string, string>("proCod_in_2", ""),
                new KeyValuePair<string, string>("TipoEmbalagem2", ""),
                new KeyValuePair<string, string>("Selecao3", ""),
                new KeyValuePair<string, string>("proCod_in_3", ""),
                new KeyValuePair<string, string>("TipoEmbalagem3", ""),
                new KeyValuePair<string, string>("Selecao4", ""),
                new KeyValuePair<string, string>("proCod_in_4", ""),
                new KeyValuePair<string, string>("TipoEmbalagem4", ""),
                new KeyValuePair<string, string>("Selecao5", ""),
                new KeyValuePair<string, string>("proCod_in_5", ""),
                new KeyValuePair<string, string>("TipoEmbalagem5", ""),
                new KeyValuePair<string, string>("Selecao6", ""),
                new KeyValuePair<string, string>("proCod_in_6", ""),
                new KeyValuePair<string, string>("TipoEmbalagem6", ""),
                new KeyValuePair<string, string>("Selecao7", ""),
                new KeyValuePair<string, string>("proCod_in_7", ""),
                new KeyValuePair<string, string>("TipoEmbalagem7", ""),
                new KeyValuePair<string, string>("Selecao8", ""),
                new KeyValuePair<string, string>("proCod_in_8", ""),
                new KeyValuePair<string, string>("nomeEmbalagemEnvelope", ""),
                new KeyValuePair<string, string>("TipoEmbalagem8", ""),
                new KeyValuePair<string, string>("Selecao9", ""),
                new KeyValuePair<string, string>("proCod_in_9", ""),
                new KeyValuePair<string, string>("TipoEmbalagem9", ""),
                new KeyValuePair<string, string>("Selecao10", ""),
                new KeyValuePair<string, string>("proCod_in_10", ""),
                new KeyValuePair<string, string>("Selecao11", ""),
                new KeyValuePair<string, string>("proCod_in_11", ""),
                new KeyValuePair<string, string>("Selecao12", ""),
                new KeyValuePair<string, string>("proCod_in_12", ""),
                new KeyValuePair<string, string>("TipoEmbalagem12", ""),
                new KeyValuePair<string, string>("Selecao13", ""),
                new KeyValuePair<string, string>("proCod_in_13", ""),
                new KeyValuePair<string, string>("TipoEmbalagem13", ""),
                new KeyValuePair<string, string>("valorDeclarado", ""),
                new KeyValuePair<string, string>("Calcular", "Calcular"),

            });

            return content;

        }

    }

}
