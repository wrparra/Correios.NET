using Correios.NET.Models;
using FluentAssertions;
using Moq;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Correios.NET.Tests
{
    public class ServicesTest
    {
        private readonly string _packageHtml;
        private readonly string _packageDeliveredHtml;
        private readonly string _addressHtml;
        private readonly ICorreiosService _services;

        public ServicesTest()
        {
            _packageHtml = ResourcesReader.GetResourceAsString("Pacote.html");
            _packageDeliveredHtml = ResourcesReader.GetResourceAsString("PacoteEntregue.html");
            _addressHtml = ResourcesReader.GetResourceAsString("Endereco.html");
            _services = new CorreiosService();
        }

        [Fact]
        public void PackageTrackingService_Live_ShouldReturnCodeAndStatuses()
        {
            const string packageCode = "QJ764113707BR";
            var result = _services.GetPackageTracking(packageCode);
            result.Code.Should().Be(packageCode);
            result.TrackingHistory.Count.Should().BeGreaterThan(0);
        }

        [Fact]
        public void AddressService_Live_ShouldReturnAddress()
        {
            const string zipCode = "15025000";
            var result = _services.GetAddresses(zipCode);
            result.Should().HaveCount(1);
            var resultAddress = result.FirstOrDefault();
            resultAddress.ZipCode.Should().Be(zipCode);
            resultAddress.Street.Should().Be("Avenida Bady Bassitt - lado par");
            resultAddress.District.Should().Be("Boa Vista");
            resultAddress.City.Should().Be("São José do Rio Preto");
            resultAddress.State.Should().Be("SP");
        }

        [Fact]
        public async void AddressServiceAsync_Live_ShouldReturnAddress()
        {
            const string zipCode = "15025000";
            var result = await _services.GetAddressesAsync(zipCode);           
            result.Should().HaveCount(1);
            var resultAddress = result.FirstOrDefault();
            resultAddress.ZipCode.Should().Be(zipCode);
            resultAddress.Street.Should().Be("Avenida Bady Bassitt - lado par");
            resultAddress.District.Should().Be("Boa Vista");
            resultAddress.City.Should().Be("São José do Rio Preto");
            resultAddress.State.Should().Be("SP");
        }

        [Fact]
        public void AddressService_ShouldReturnAddress()
        {
            const string zipCode = "15000010";
            var services = new Mock<ICorreiosService>();
            
            services.Setup(s => s.GetAddresses(zipCode))
                .Returns(Parser.ParseAddresses(_addressHtml));

            var result = services.Object.GetAddresses(zipCode);
            result.Should().HaveCount(2);
            var resultAddress = result.FirstOrDefault();
            resultAddress.ZipCode.Should().Be(zipCode);
            resultAddress.Street.Should().Be("Rua de Teste 1");
            resultAddress.District.Should().Be("Bairro de Teste 1");
            resultAddress.City.Should().Be("São José do Rio Preto");
            resultAddress.State.Should().Be("SP");
        }

        [Fact]
        public void PackageTrackingService_ShouldReturnStatuses()
        {
            const string packageCode = "OQ293998542BR";
            var services = new Moq.Mock<ICorreiosService>();
            services.Setup(s => s.GetPackageTracking(packageCode))
                .Returns(Parser.ParsePackage(_packageHtml));

            var result = services.Object.GetPackageTracking(packageCode);

            result.Code.Should().Be(packageCode);
            result.TrackingHistory.Should().HaveCount(4);
        }

        [Fact]
        public void PackageTrackingService_ShouldBeDelivered()
        {
            const string packageCode = "QJ764113707BR";
            var services = new Moq.Mock<ICorreiosService>();
            services.Setup(s => s.GetPackageTracking(packageCode))
                .Returns(Parser.ParsePackage(_packageDeliveredHtml));

            var result = services.Object.GetPackageTracking(packageCode);

            result.Code.Should().Be(packageCode);
            result.IsDelivered.Should().BeTrue();
        }        
    }
}
