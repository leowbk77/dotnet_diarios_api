using Moq;
using Microsoft.AspNetCore.Mvc;
using Diarios.Api.Controllers;
using Diarios.Api.Domain.Contracts.Service;
using Diarios.Api.Domain.Models.Entities;
using Diarios.Api.Domain.Models.Requests;
using Diarios.Api.Domain.Models.Responses;
using Diarios.Api.Application.Util.CustomException;

namespace Diarios.Api.Tests.Controllers
{
    public class DiarioControllerTests
    {
        [Fact]
        public void GetDiarioById_ReturnsOkWithDiario()
        {
            var mockService = new Mock<IDiarioService>();
            var esperado = new Diario { Id = 1, NmEdicao = "E1" };
            mockService.Setup(s => s.GetDiarioById(1, "SP")).Returns(esperado);

            var controller = new DiarioController(mockService.Object);

            var result = controller.GetDiarioById(1, "SP");

            var ok = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(esperado, ok.Value);
        }

        [Fact]
        public void GetDiarioById_WhenServiceThrowsDiarioCustomException_ReturnsStatusCode()
        {
            var mockService = new Mock<IDiarioService>();
            mockService.Setup(s => s.GetDiarioById(1, "SP"))
                       .Throws(new DiarioCustomException(404, "não encontrado"));

            var controller = new DiarioController(mockService.Object);

            var result = controller.GetDiarioById(1, "SP");

            var obj = Assert.IsType<ObjectResult>(result);
            Assert.Equal(404, obj.StatusCode);
            Assert.Equal("não encontrado", obj.Value);
        }

        [Fact]
        public async Task Search_ReturnsOkWithSearchResponse()
        {
            var mockService = new Mock<IDiarioService>();
            var req = new SearchRequest { Cidade = "SP", Terms = "teste" };
            var resp = new SearchResponse
            {
                SearchDiariosResults = { new SearchDiariosResultModel { Id = 1, NmEdicao = "E1" } },
                HasMore = false
            };

            mockService.Setup(s => s.SearchDiariosAsync(req)).ReturnsAsync(resp);

            var controller = new DiarioController(mockService.Object);

            var result = await controller.Search(req);

            var ok = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(resp, ok.Value);
        }

        [Fact]
        public async Task GetLatestDiario_ReturnsOk()
        {
            var mockService = new Mock<IDiarioService>();
            var esperado = new Diario { Id = 99, NmEdicao = "Último" };
            mockService.Setup(s => s.SearchForLatestAsync("SP")).ReturnsAsync(esperado);

            var controller = new DiarioController(mockService.Object);

            var result = await controller.GetLatestDiario("SP");

            var ok = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(esperado, ok.Value);
        }

        [Fact]
        public async Task GetLatestDiario_WhenServiceThrowsDiarioCustomException_ReturnsStatusCode()
        {
            var mockService = new Mock<IDiarioService>();
            mockService.Setup(s => s.SearchForLatestAsync("SP"))
                       .ThrowsAsync(new DiarioCustomException(404, "não encontrado"));

            var controller = new DiarioController(mockService.Object);

            var result = await controller.GetLatestDiario("SP");

            var obj = Assert.IsType<ObjectResult>(result);
            Assert.Equal(404, obj.StatusCode);
            Assert.Equal("não encontrado", obj.Value);
        }
    }
}