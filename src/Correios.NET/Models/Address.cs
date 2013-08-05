using System.Net.Http;
namespace Correios.NET.Models
{
    public class Address
    {
        public string Street { get; set; }
        public string District { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string ZipCode { get; set; }

        public static Address Parse(string html)
        {
            return null;
        }
    }
}