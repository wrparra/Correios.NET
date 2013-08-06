using System;
using Correios.NET.Exceptions;
using Correios.NET.Models;
using Correios.NET.Tests.Models;
using FluentAssertions;
using Xunit;

namespace Correios.NET.Tests
{
    public class ServicesTest
    {
        private readonly string _packageHtml;
        private readonly string _packageDeliveredHtml;
        private readonly string _addressHtml;

        public ServicesTest()
        {
            _packageHtml = ResourcesReader.GetResourceAsString("Pacote.html");
            _packageDeliveredHtml = ResourcesReader.GetResourceAsString("PacoteEntregue.html");
            _addressHtml = ResourcesReader.GetResourceAsString("Endereco.html");
        }

        [Fact(Timeout = 5000)]
        public void PackageTrackingService_ShouldReturnCodeAndStatuses()
        {
            const string packageCode = "SW552251158BR";
            IServices services = new Services();
            var result = services.GetPackageTracking(packageCode);

            result.Code.Should().Be(packageCode);
            result.TrackingHistory.Count.Should().BeGreaterThan(0);
        }

        [Fact]
        public void PackageTrackingService_ShouldReturnStatuses()
        {
            const string packageCode = "SW552251158BR";
            var services = new Moq.Mock<IServices>();
            services.Setup(s => s.GetPackageTracking(packageCode))
                .Returns(Package.Parse(_packageHtml));

            var result = services.Object.GetPackageTracking(packageCode);

            result.Code.Should().Be(packageCode);
            result.TrackingHistory.Should().HaveCount(3);
        }

        [Fact]
        public void PackageTrackingService_ShouldBeDelivered()
        {
            const string packageCode = "SW552251144BR";
            var services = new Moq.Mock<IServices>();
            services.Setup(s => s.GetPackageTracking(packageCode))
                .Returns(Package.Parse(_packageDeliveredHtml));

            var result = services.Object.GetPackageTracking(packageCode);

            result.Code.Should().Be(packageCode);
            result.IsDelivered.Should().BeTrue();
        }

        [Fact]
        public void AddressService_ShouldReturnAddress()
        {
            const string zipCode = "15000000";
            var services = new Moq.Mock<IServices>();

            services.Setup(s => s.GetAddress(zipCode))
                .Returns(Address.Parse(_addressHtml));

            var result = services.Object.GetAddress(zipCode);
            result.ZipCode.Should().Be(zipCode);
            result.Street.Should().Be("Rua de Teste");
            result.District.Should().Be("Bairro de Teste");
            result.City.Should().Be("São José do Rio Preto");
            result.State.Should().Be("SP");
        }
    }
}
