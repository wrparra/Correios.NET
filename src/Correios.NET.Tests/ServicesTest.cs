using Correios.NET.Exceptions;
using Correios.NET.Models;
using FluentAssertions;
using FluentAssertions.Primitives;
using Moq;
using System;
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

        public ServicesTest()
        {
            _packageHtml = ResourcesReader.GetResourceAsString("Pacote.html");
            _packageDeliveredHtml = ResourcesReader.GetResourceAsString("PacoteEntregue.html");
            _addressHtml = ResourcesReader.GetResourceAsString("Endereco.html");
        }

        [Fact]
        public void PackageTrackingService_Live_ShouldReturnCodeAndStatuses()
        {
            const string packageCode = "OD747761508BR";
            IServices services = new Services();
            var result = services.GetPackageTracking(packageCode);

            result.Code.Should().Be(packageCode);
            result.TrackingHistory.Count.Should().BeGreaterThan(0);
        }

        [Fact]
        public void AddressService_Live_ShouldReturnAddress()
        {
            const string zipCode = "15025000";
            IServices services = new Services();
            var result = services.GetAddresses(zipCode);
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
            IServices services = new Services();
            var result = await services.GetAddressesAsync(zipCode);
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
            var services = new Mock<IServices>();

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
            const string packageCode = "DU713842539BR";
            var services = new Moq.Mock<IServices>();
            services.Setup(s => s.GetPackageTracking(packageCode))
                .Returns(Parser.ParsePackage(_packageHtml));

            var result = services.Object.GetPackageTracking(packageCode);

            result.Code.Should().Be(packageCode);
            result.TrackingHistory.Should().HaveCount(3);
        }

        [Fact]
        public void PackageTrackingService_ShouldBeDelivered()
        {
            const string packageCode = "DV248292626BR";
            var services = new Moq.Mock<IServices>();
            services.Setup(s => s.GetPackageTracking(packageCode))
                .Returns(Parser.ParsePackage(_packageDeliveredHtml));

            var result = services.Object.GetPackageTracking(packageCode);

            result.Code.Should().Be(packageCode);
            result.IsDelivered.Should().BeTrue();
        }


        [Fact]
        public void DeliveryPriceService_ShouldReturn1Item()
        {
            IServices services = new Services();
            var result = services.GetDeliveryPrices(DateTime.Now, "08226-021", "71070-654", DeliveryOptions.PAC, 15, 15, 16, 1);
            result.Should().HaveCount(1);
        }

        [Fact]
        public void DeliveryPriceService_ShouldReturn2Items()
        {
            IServices services = new Services();
            var result = services.GetDeliveryPrices(DateTime.Now, "08226-021", "71070-654", DeliveryOptions.PAC | DeliveryOptions.SEDEX, 15, 15, 16, 1);
            result.Should().HaveCount(2);
        }

        [Fact]
        public async Task DeliveryPriceService_ShouldFailWrongHeigth()
        {
            IServices services = new Services();


            var exception1 = await Assert.ThrowsAsync<PackageSizeException>(() =>
                    services.GetDeliveryPricesAsync(DateTime.Now, "08226-021", "71070-654", DeliveryOptions.PAC | DeliveryOptions.SEDEX, 1, 15, 15, 1)
                );

            Assert.Equal("A altura da caixa deve ter no mínimo 2cm e no máximo 100cm", exception1.Message);

            var exception2 = await Assert.ThrowsAsync<PackageSizeException>(() =>
                services.GetDeliveryPricesAsync(DateTime.Now, "08226-021", "71070-654", DeliveryOptions.PAC | DeliveryOptions.SEDEX, 101, 15, 15, 1)
            );

            Assert.Equal("A altura da caixa deve ter no mínimo 2cm e no máximo 100cm", exception2.Message);

        }

        [Fact]
        public async Task DeliveryPriceService_ShouldFailWrongWidth()
        {
            IServices services = new Services();

            var exception1 = await Assert.ThrowsAsync<PackageSizeException>(() =>
                    services.GetDeliveryPricesAsync(DateTime.Now, "08226-021", "71070-654", DeliveryOptions.PAC | DeliveryOptions.SEDEX, 15, 10, 15, 1)
                );

            Assert.Equal("A largura da caixa deve ter no mínimo 11cm e no máximo 100cm", exception1.Message);

            var exception2 = await Assert.ThrowsAsync<PackageSizeException>(() =>
                services.GetDeliveryPricesAsync(DateTime.Now, "08226-021", "71070-654", DeliveryOptions.PAC | DeliveryOptions.SEDEX, 15, 101, 15, 1)
            );

            Assert.Equal("A largura da caixa deve ter no mínimo 11cm e no máximo 100cm", exception2.Message);
        }

        [Fact]
        public async Task DeliveryPriceService_ShouldFailWrongLength()
        {
            IServices services = new Services();


            var exception1 = await Assert.ThrowsAsync<PackageSizeException>(() =>
                    services.GetDeliveryPricesAsync(DateTime.Now, "08226-021", "71070-654", DeliveryOptions.PAC | DeliveryOptions.SEDEX, 15, 15, 15, 1)
                );

            Assert.Equal("O comprimento da caixa deve ter no mínimo 16cm e no máximo 100cm", exception1.Message);

            var exception2 = await Assert.ThrowsAsync<PackageSizeException>(() =>
                services.GetDeliveryPricesAsync(DateTime.Now, "08226-021", "71070-654", DeliveryOptions.PAC | DeliveryOptions.SEDEX, 15, 15, 101, 1)
            );

            Assert.Equal("O comprimento da caixa deve ter no mínimo 16cm e no máximo 100cm", exception2.Message);

        }

    }
}
