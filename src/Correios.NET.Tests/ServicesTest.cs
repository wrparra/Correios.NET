using System;
using FluentAssertions;
using Xunit;

namespace Correios.NET.Tests
{
    public class ServicesTest
    {
        [Fact]
        public void PackageService_ShouldReturn_Statuses()
        {
            const string packageCode = "SW552251144BR";
            var services = new Services();
            var result = services.GetPackageTracking(packageCode);

            foreach (var status in result.Statuses)
            {
                Console.WriteLine("{0:dd/MM/yyyy HH:mm} - {1} - {2} - {3}", status.Date, status.Location, status.Situation, status.Details);
            }


            result.Code.Should().Be(packageCode);
            result.Statuses.Should().HaveCount(6);
        }
    }
}
