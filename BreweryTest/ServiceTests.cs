using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using BreweryAPI.Services.Interfaces;
using System.Threading;
using BreweryAPI.Models;
using BreweryAPI.Services;
using Microsoft.Extensions.Logging;
using Moq;
using System.Linq;

namespace BreweryTest
{
    [TestClass]
    public class ServiceTests
    {
        [TestMethod]
        public async Task SortByDistance_WhenOriginProvided_ComputesDistance()
        {
            // Arrange
            var repo = new Mock<IBreweryRepository>();

            repo.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<OpenBreweryResponse> {
                    new OpenBreweryResponse { id="1", name="A", city="X", phone="", latitude="45.0", longitude="-122.0" },
                    new OpenBreweryResponse { id="2", name="B", city="Y", phone="", latitude="46.0", longitude="-123.0" }
                });

            var logger = Mock.Of<ILogger<BreweryService>>();
            var svc = new BreweryService(repo.Object, logger);

            // Act
            var result = await svc.GetBreweriesAsync(new QueryParams
            {
                SortBy = "distance",
                OriginLat = 45.5,
                OriginLon = -122.5
            }, CancellationToken.None);

            // Assert
            foreach (var item in result.Items)
            {
                Assert.IsNotNull(item.DistanceKm);
            }

            // MSTest uses Assert.AreEqual(expected, actual)
            Assert.AreEqual("B", result.Items.First().Name);
        }
    }
}


