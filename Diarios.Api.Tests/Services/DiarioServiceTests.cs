using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using Moq;
using Diarios.Api.Application.Services;
using Diarios.Api.Domain.Contracts.Repository;
using Diarios.Api.Domain.Models.Entities;
using Diarios.Api.Domain.Models.Requests;
using Diarios.Api.Domain.Models.Responses;
using Diarios.Api.Domain.Models;
using Diarios.Api.Application.Util.CustomException;

namespace Diarios.Api.Tests.Services
{
    public class DiarioServiceTests
    {
        [Fact]
        public void GetDiarioById_DelegatesToRepository()
        {
            var mockRepo = new Mock<IDiarioRepository>();
            var esperado = new Diario { Id = 5, NmEdicao = "E5" };
            mockRepo.Setup(r => r.GetDiarioById(5, "SP")).Returns(esperado);

            var service = new DiarioService(mockRepo.Object);

            var result = service.GetDiarioById(5, "SP");

            Assert.Equal(esperado, result);
        }

        [Fact]
        public async Task SearchDiariosAsync_WithMoreResults_ReturnsHasMoreTrueAndResults()
        {
            var mockRepo = new Mock<IDiarioRepository>();
            var search = new SearchRequest { Cidade = "SP", Limit = 1, Terms = "abc+" };

            // IDs returned exceed the limit -> hasMore = true
            mockRepo.Setup(r => r.SearchForDiariosIdsAsync(It.IsAny<QueryDefinition>(), "SP"))
                    .ReturnsAsync(new List<int> { 1, 2 });

            var expectedResults = new List<SearchDiariosResultModel>
            {
                new SearchDiariosResultModel { Id = 1, NmEdicao = "E1" }
            };

            mockRepo.Setup(r => r.SearchDiariosByIdListAsync(It.IsAny<QueryDefinition>(), "SP"))
                    .ReturnsAsync(expectedResults);

            var service = new DiarioService(mockRepo.Object);

            var response = await service.SearchDiariosAsync(search);

            Assert.True(response.HasMore);
            Assert.Equal(expectedResults, response.SearchDiariosResults);
            // also ensure terms '+' replaced by space
            Assert.Equal("abc ", search.Terms);
        }

        [Fact]
        public async Task SearchForLatestAsync_WhenRepositoryReturnsNull_ThrowsDiarioCustomException()
        {
            var mockRepo = new Mock<IDiarioRepository>();
            mockRepo.Setup(r => r.SearchForLatestAsync("SP")).ReturnsAsync((Diario?)null);

            var service = new DiarioService(mockRepo.Object);

            await Assert.ThrowsAsync<DiarioCustomException>(() => service.SearchForLatestAsync("SP"));
        }

        [Fact]
        public async Task SearchForLatestAsync_ReturnsDiarioWhenFound()
        {
            var mockRepo = new Mock<IDiarioRepository>();
            var esperado = new Diario { Id = 10, NmEdicao = "Último" };
            mockRepo.Setup(r => r.SearchForLatestAsync("SP")).ReturnsAsync(esperado);

            var service = new DiarioService(mockRepo.Object);

            var result = await service.SearchForLatestAsync("SP");

            Assert.Equal(esperado, result);
        }
    }
}