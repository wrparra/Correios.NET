using Correios.NET.Exceptions;
using Correios.NET.Extensions;
using FluentAssertions;
using System;
using System.Globalization;
using Xunit;

namespace Correios.NET.Tests.Models
{
    public class PackageTest
    {
        private readonly string _packageHtml;
        private readonly string _packageDeliveredHtml;
        private readonly string _packageErrorHtml;
        private readonly string _packageCodeNotFound;

        public PackageTest()
        {
            CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("pt-BR");
            _packageHtml = ResourcesReader.GetResourceAsString("Pacote.html");
            _packageDeliveredHtml = ResourcesReader.GetResourceAsString("PacoteEntregue.html");
            _packageErrorHtml = ResourcesReader.GetResourceAsString("PacoteNaoEncontrado.html");
            _packageCodeNotFound = ResourcesReader.GetResourceAsString("PacoteSemCodigo.html");
        }

        [Fact]
        public void ExtractPackageDateTime() {

            var line = "Data : 28/01/2022 | Hora: 22:02";
            const string pattern = @"[\w\s\:]*(\d{2}\/\d{2}\/\d{4})[\w\|\s\:]*(\d{2}\:\d{2})";
            var dateTime = line.ExtractDateTime(pattern);
            var expectedDateTime = DateTime.Parse("28/01/2022 22:02", CultureInfo.GetCultureInfo("pt-BR"));
            dateTime.Should().Be(expectedDateTime);
        }

        [Fact]
        public void PackageParse_ShouldBeReturnAValidPackage()
        {
            var package = Parser.ParsePackage(_packageHtml);
            package.Code.Should().NotBeNullOrEmpty();
            package.IsValid.Should().BeTrue();
            package.ShipDate.Should().Be(DateTime.Parse("28/01/2022 15:05", CultureInfo.GetCultureInfo("pt-BR")));
            package.IsDelivered.Should().BeFalse();
            package.DeliveryDate.HasValue.Should().BeFalse();
        }

        [Fact]
        public void PackageParse_ShouldBeReturnADeliveredPackage()
        {
            var package = Parser.ParsePackage(_packageDeliveredHtml);
            package.Code.Should().NotBeNullOrEmpty();
            package.IsValid.Should().BeTrue();
            package.IsDelivered.Should().BeTrue();
            package.ShipDate.Should().Be(DateTime.Parse("23/12/2021 15:19", CultureInfo.GetCultureInfo("pt-BR")));
            package.DeliveryDate.Should().Be(DateTime.Parse("28/12/2021 16:37", CultureInfo.GetCultureInfo("pt-BR")));
        }

        [Fact]
        public void PackageParse_ShouldThrowAnParseException()
        {
            Action act = () => Parser.ParsePackage(_packageErrorHtml);
            act.Should().Throw<ParseException>();
        }

        [Fact]
        public void PackageParse_ShouldThrowAnParseExceptionCodeNotFound()
        {
            Action act = () => Parser.ParsePackage(_packageCodeNotFound);
            act.Should().Throw<ParseException>().WithMessage("Código da encomenda/pacote não foi encontrado.");
        }
    }
}
