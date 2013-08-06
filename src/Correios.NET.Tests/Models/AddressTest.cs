using System;
using Correios.NET.Exceptions;
using Correios.NET.Models;
using FluentAssertions;
using Xunit;

namespace Correios.NET.Tests.Models
{
    public class AddressTest
    {
        private readonly string _addressHtml;
        private readonly string _addressErrorHtml;

        public AddressTest()
        {
            _addressHtml = ResourcesReader.GetResourceAsString("Endereco.html");
            _addressErrorHtml = ResourcesReader.GetResourceAsString("EnderecoNaoEncontrado.html");
        }

        [Fact]
        public void AddressParse_ShouldReturnAddress()
        {
            var result = Address.Parse(_addressHtml);
            result.ZipCode.Should().Be("15000000");
            result.Street.Should().Be("Rua de Teste");
            result.District.Should().Be("Bairro de Teste");
            result.City.Should().Be("São José do Rio Preto");
            result.State.Should().Be("SP");
        }

        [Fact]
        public void AddressParse_ShouldThrowAnParseException()
        {
            Action act = () => Address.Parse(_addressErrorHtml);
            act.ShouldThrow<ParseException>().WithMessage("Endereço inválido.");
        }
    }
}