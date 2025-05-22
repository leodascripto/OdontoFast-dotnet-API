using Microsoft.AspNetCore.Mvc;
using Moq;
using OdontofastAPI.Controllers;
using OdontofastAPI.DTO;
using OdontofastAPI.Service.Interfaces;
using System;
using System.Threading.Tasks;
using Xunit;

namespace OdontofastAPI.Tests.Controllers
{
    public class ProgressoControllerTests
    {
        private readonly Mock<IProgressoService> _progressoServiceMock;
        private readonly ProgressoController _controller;

        public ProgressoControllerTests()
        {
            _progressoServiceMock = new Mock<IProgressoService>();
            _controller = new ProgressoController(_progressoServiceMock.Object);
        }

        [Fact]
        public async Task Post_ValidProgressoDto_ReturnsOk()
        {
            // Arrange
            var progressoDto = new ProgressoDTO { IdUsuario = 1, Progresso = 75.5 };
            _progressoServiceMock.Setup(s => s.ProcessarProgressoAsync(progressoDto))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.Post(progressoDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = okResult.Value;
            Assert.NotNull(returnValue);
            Assert.Equal("Progresso recebido e processado.", returnValue.GetType().GetProperty("message")?.GetValue(returnValue));
        }

        [Fact]
        public async Task Post_ServiceThrowsException_ReturnsInternalServerError()
        {
            // Arrange
            var progressoDto = new ProgressoDTO { IdUsuario = 1, Progresso = 75.5 };
            _progressoServiceMock.Setup(s => s.ProcessarProgressoAsync(progressoDto))
                .ThrowsAsync(new Exception("Processing error"));

            // Act
            var result = await _controller.Post(progressoDto);

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, statusCodeResult.StatusCode);
            var returnValue = statusCodeResult.Value;
            Assert.NotNull(returnValue);
            Assert.Equal("Erro interno no servidor.", returnValue.GetType().GetProperty("message")?.GetValue(returnValue));
            Assert.Equal("Processing error", returnValue.GetType().GetProperty("error")?.GetValue(returnValue));
        }
    }
}