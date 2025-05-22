using Microsoft.AspNetCore.Mvc;
using Moq;
using OdontofastAPI.Controllers;
using OdontofastAPI.Model;
using OdontofastAPI.Service.Interfaces;
using System;
using System.Threading.Tasks;
using Xunit;

namespace OdontofastAPI.Tests.Controllers
{
    public class LoginControllerTests
    {
        private readonly Mock<IUsuarioService> _usuarioServiceMock;
        private readonly LoginController _controller;

        public LoginControllerTests()
        {
            _usuarioServiceMock = new Mock<IUsuarioService>();
            _controller = new LoginController(_usuarioServiceMock.Object);
        }

        [Fact]
        public async Task Login_ValidCredentials_ReturnsOkWithUserData()
        {
            // Arrange
            var loginDto = new LoginDto { NrCarteira = "12345", Senha = "password" };
            var usuario = new Usuario
            {
                IdUsuario = 1,
                NomeUsuario = "Test User",
                SenhaUsuario = "password123", // Inicializar propriedade required
                EmailUsuario = "test@example.com",
                NrCarteira = "12345",
                TelefoneUsuario = 123456789
            };
            _usuarioServiceMock.Setup(s => s.Login(loginDto.NrCarteira, loginDto.Senha))
                .ReturnsAsync(usuario);

            // Act
            var result = await _controller.Login(loginDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = okResult.Value;
            Assert.NotNull(returnValue);
            Assert.Equal(usuario.IdUsuario, returnValue.GetType().GetProperty("IdUsuario")?.GetValue(returnValue));
            Assert.Equal(usuario.NomeUsuario, returnValue.GetType().GetProperty("NomeUsuario")?.GetValue(returnValue));
            Assert.Equal(usuario.EmailUsuario, returnValue.GetType().GetProperty("EmailUsuario")?.GetValue(returnValue));
            Assert.Equal(usuario.NrCarteira, returnValue.GetType().GetProperty("NrCarteira")?.GetValue(returnValue));
            Assert.Equal(usuario.TelefoneUsuario, returnValue.GetType().GetProperty("TelefoneUsuario")?.GetValue(returnValue));
        }

        [Fact]
        public async Task Login_InvalidCredentials_ReturnsUnauthorized()
        {
            // Arrange
            var loginDto = new LoginDto { NrCarteira = "12345", Senha = "wrong" };
            _usuarioServiceMock.Setup(s => s.Login(loginDto.NrCarteira, loginDto.Senha))
                .ReturnsAsync((Usuario?)null);

            // Act
            var result = await _controller.Login(loginDto);

            // Assert
            var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result);
            var returnValue = unauthorizedResult.Value;
            Assert.NotNull(returnValue);
            Assert.Equal("Credenciais inválidas.", returnValue.GetType().GetProperty("message")?.GetValue(returnValue));
        }

        [Fact]
        public async Task Login_ServiceThrowsException_ReturnsInternalServerError()
        {
            // Arrange
            var loginDto = new LoginDto { NrCarteira = "12345", Senha = "password" };
            _usuarioServiceMock.Setup(s => s.Login(loginDto.NrCarteira, loginDto.Senha))
                .ThrowsAsync(new Exception("Database error"));

            // Act
            var result = await _controller.Login(loginDto);

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, statusCodeResult.StatusCode);
            var returnValue = statusCodeResult.Value;
            Assert.NotNull(returnValue);
            Assert.Equal("Erro interno no servidor.", returnValue.GetType().GetProperty("message")?.GetValue(returnValue));
            Assert.Equal("Database error", returnValue.GetType().GetProperty("error")?.GetValue(returnValue));
        }
    }
}