using System;
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
        private readonly string _packageErrorHtml;

        public ServicesTest()
        {
            _packageHtml = ResourcesReader.GetResourceAsString("Pacote.html");
            _packageDeliveredHtml = ResourcesReader.GetResourceAsString("PacoteEntregue.html");
            _packageErrorHtml = ResourcesReader.GetResourceAsString("PacoteNaoEncontrado.html");
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
            result.Statuses.Should().HaveCount(6);
        }

        [Fact]
        public void PackageTrackingService_ShouldNotReturnStatuses()
        {
            const string packageCode = "SW000000000BR";
            var services = new Moq.Mock<IServices>();
            services.Setup(s => s.GetPackageTracking(packageCode))
                .Returns(Package.Parse(_packageErrorHtml));

            var result = services.Object.GetPackageTracking(packageCode);
            
            result.Code.Should().Be(packageCode);
            result.Statuses.Should().HaveCount(6);
        }
    }
}
