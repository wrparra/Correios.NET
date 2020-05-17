using System;
using System.Collections.Generic;
using System.Linq;
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
            var result = Parser.ParseAddresses(_addressHtml);
            result.Should().HaveCount(2);
            var resultAddress1 = result.FirstOrDefault();
            resultAddress1.ZipCode.Should().Be("15000010");
            resultAddress1.Street.Should().Be("Rua de Teste 1");
            resultAddress1.District.Should().Be("Bairro de Teste 1");
            resultAddress1.City.Should().Be("São José do Rio Preto");
            resultAddress1.State.Should().Be("SP");

            var resultAddress2 = result.LastOrDefault();
            resultAddress2.ZipCode.Should().Be("15000020");
            resultAddress2.Street.Should().Be("Rua de Teste 2");
            resultAddress2.District.Should().Be("Bairro de Teste 2");
            resultAddress2.City.Should().Be("São José do Rio Preto");
            resultAddress2.State.Should().Be("SP");
        }

        [Fact]
        public void AddressParse_ShouldThrowAnParseException()
        {
            Action act = () => Parser.ParseAddresses(_addressErrorHtml);
            act.Should().Throw<ParseException>().WithMessage("Endereço não encontrado.");
        }
    }
}