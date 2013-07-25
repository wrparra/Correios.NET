using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Correios.NET.Exceptions;
using Correios.NET.Models;
using FluentAssertions;
using Xunit;

namespace Correios.NET.Tests.Models
{
    public class PackageTest
    {
        private readonly string _packageHtml;
        private readonly string _packageDeliveredHtml;
        private readonly string _packageErrorHtml;

        public PackageTest()
        {
            _packageHtml = ResourcesReader.GetResourceAsString("Pacote.html");
            _packageDeliveredHtml = ResourcesReader.GetResourceAsString("PacoteEntregue.html");
            _packageErrorHtml = ResourcesReader.GetResourceAsString("PacoteNaoEncontrado.html");
        }

        [Fact]
        public void PackageParse_ShouldBeReturnAValidPackage()
        {
            var package = Package.Parse(_packageHtml);
            package.Code.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public void PackageParse_ShouldBeReturnADeliveredPackage()
        {
            var package = Package.Parse(_packageDeliveredHtml);
            package.Code.Should().NotBeNullOrEmpty();
            package.IsDelivered.Should().BeTrue();
        }

        [Fact]
        public void PackageParse_ShouldThrowAnParseException()
        {
            Action act = () => Package.Parse(_packageErrorHtml);
            act.ShouldThrow<ParseException>()
                .WithMessage("Não foi possível converter o pacote/encomenda.");
        }
    }
}
