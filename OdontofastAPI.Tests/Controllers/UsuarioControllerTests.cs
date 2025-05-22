using Microsoft.AspNetCore.Mvc;
using Moq;
using OdontofastAPI.Controllers;
using OdontofastAPI.DTO;
using OdontofastAPI.Service.Interfaces;
using System.Threading.Tasks;
using Xunit;

namespace OdontofastAPI.Tests.Controllers
{
    public class UsuarioControllerTests
    {
        private readonly Mock<IUsuarioService> _usuarioServiceMock;
        private readonly UsuarioController _controller;

        public UsuarioControllerTests()
        {
            _usuarioServiceMock = new Mock<IUsuarioService>();
            _controller = new UsuarioController(_usuarioServiceMock.Object);
        }

        [Fact]
        public async Task GetUsuarioById_ExistingId_ReturnsOkWithUsuario()
        {
            // Arrange
            var id = 1;
            var usuarioDto = new UsuarioDTO
            {
                IdUsuario = id,
                NomeUsuario = "Test User",
                SenhaUsuario = "password123", // Inicializar propriedade required
                EmailUsuario = "test@example.com",
                NrCarteira = "12345",
                TelefoneUsuario = 123456789
            };
            _usuarioServiceMock.Setup(s => s.GetUsuarioByIdAsync(id))
                .ReturnsAsync(usuarioDto);

            // Act
            var result = await _controller.GetUsuarioById(id);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(usuarioDto, okResult.Value);
        }

        [Fact]
        public async Task GetUsuarioById_NonExistingId_ReturnsNotFound()
        {
            // Arrange
            var id = 999;
            _usuarioServiceMock.Setup(s => s.GetUsuarioByIdAsync(id))
                .ReturnsAsync((UsuarioDTO?)null);

            // Act
            var result = await _controller.GetUsuarioById(id);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            var returnValue = notFoundResult.Value;
            Assert.NotNull(returnValue);
            Assert.Equal("Usuário não encontrado.", returnValue.GetType().GetProperty("Message")?.GetValue(returnValue));
        }

        [Fact]
        public async Task UpdateUsuario_ExistingId_ReturnsOkWithUpdatedUsuario()
        {
            // Arrange
            var id = 1;
            var usuarioDto = new UsuarioDTO
            {
                IdUsuario = id,
                NomeUsuario = "Updated User",
                SenhaUsuario = "updated123", // Inicializar propriedade required
                EmailUsuario = "updated@example.com",
                NrCarteira = "12345",
                TelefoneUsuario = 987654321
            };
            _usuarioServiceMock.Setup(s => s.UpdateUsuarioAsync(id, usuarioDto))
                .ReturnsAsync(usuarioDto);

            // Act
            var result = await _controller.UpdateUsuario(id, usuarioDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(usuarioDto, okResult.Value);
        }

        [Fact]
        public async Task UpdateUsuario_NonExistingId_ReturnsNotFound()
        {
            // Arrange
            var id = 999;
            var usuarioDto = new UsuarioDTO
            {
                IdUsuario = id,
                NomeUsuario = "Updated User",
                SenhaUsuario = "updated123", // Inicializar propriedade required
                EmailUsuario = "updated@example.com",
                NrCarteira = "12345",
                TelefoneUsuario = 987654321
            };
            _usuarioServiceMock.Setup(s => s.UpdateUsuarioAsync(id, usuarioDto))
                .ReturnsAsync((UsuarioDTO?)null);

            // Act
            var result = await _controller.UpdateUsuario(id, usuarioDto);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            var returnValue = notFoundResult.Value;
            Assert.NotNull(returnValue);
            Assert.Equal("Usuário não encontrado.", returnValue.GetType().GetProperty("Message")?.GetValue(returnValue));
        }
    }
}