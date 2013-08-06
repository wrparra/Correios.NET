using System.Linq;
using System.Net.Http;
using Correios.NET.Exceptions;
using Correios.NET.Extensions;
using CsQuery;

namespace Correios.NET.Models
{
    public class Address
    {
        public string Street { get; set; }
        public string District { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string ZipCode { get; set; }

        /// <summary>
        /// Parse and converts the html page in a zip address
        /// </summary>
        /// <param name="html">html page</param>
        /// <returns>An Address</returns>
        /// <exception cref="Correios.NET.Exceptions.ParseException"></exception>
        public static Address Parse(string html)
        {
            CQ dom = html;
            var form = dom["form#frmCep"];
            var data = form["span.respostadestaque"];
            
            if (data.Length != 4)
                throw new ParseException("Endereço inválido.");

            var street = data[0].InnerText.RemoveLineEndings();
            var district = data[1].InnerText.RemoveLineEndings();
            var cityState = data[2].InnerText.RemoveLineEndings().Split(new []{'/'});
            
            if (cityState.Length != 2)
                throw  new ParseException("Cidade e Estados não puderam ser lidos.");
            
            var city = cityState[0].Trim();
            var state = cityState[1].Trim();
            var zipcode = data[3].InnerText.RemoveLineEndings();
            
            return new Address
            {
                Street = street,
                ZipCode = zipcode,
                District = district,
                City = city,
                State = state
            };
        }
    }
}