using APIDoubleV.Controllers;
using APIDoubleV.Data;
using APIDoubleV.Models;
using APIDoubleV.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace ApiDoubleVTest
{
    [Collection("Sequential")]
    public class TicketTest
    {
        private readonly Mock<ITicketRepository> _mockTicketRepository;
        private readonly TicketController _ticketController;

        public TicketTest()
        {
            _mockTicketRepository = new Mock<ITicketRepository>();
            _ticketController = new TicketController(_mockTicketRepository.Object);
        }
        [Fact]
        public async Task ObtenerTickets_OK()
        {
            // Arrange
            var pageNumber = 1;
            var pageSize = 2;
            var tickets = new List<Ticket>
        {
            new Ticket { Id = 1 },
            new Ticket { Id = 2 },
            new Ticket { Id = 3 }
        };
            _mockTicketRepository.Setup(x => x.GetAllTicketAsync()).ReturnsAsync(tickets);

            // Act
            var resultado = await _ticketController.ObtenerTickets(pageNumber, pageSize);

            // Assert
            var actionResult = Assert.IsType<ActionResult<List<Ticket>>>(resultado);
            var okObjectResult = Assert.IsType<OkObjectResult>(actionResult.Result);
            var paginacionTickets = Assert.IsAssignableFrom<List<Ticket>>(okObjectResult.Value);
            Assert.Equal(2, paginacionTickets.Count); // Se espera que haya 3 tickets en la página 1 con un tamaño de página de 2.
        }

        [Fact]
        public async Task ObtenerTickets_PageInvalid()
        {
            // Arrange
            var pageNumber = 0;
            var pageSize = 10;

            // Act
            var resultado = await _ticketController.ObtenerTickets(pageNumber, pageSize);

            // Assert
            var actionResult = Assert.IsType<ActionResult<List<Ticket>>>(resultado);
            var badRequestObjectResult = Assert.IsType<BadRequestObjectResult>(actionResult.Result);
            Assert.Equal("pageNumber y pageSize deben ser números positivos", badRequestObjectResult.Value);
        }

        [Fact]
        public async Task ObtenerTickets_StatusCode500()
        {
            // Arrange
            var pageNumber = 1;
            var pageSize = 10;
            _mockTicketRepository.Setup(x => x.GetAllTicketAsync()).Throws(new Exception());
         
            // Act
            var resultado = await _ticketController.ObtenerTickets(pageNumber, pageSize);

            // Assert
            var actionResult = Assert.IsType<ActionResult<List<Ticket>>>(resultado);
            var objectResult = Assert.IsType<ObjectResult>(actionResult.Result);
            Assert.Equal(500, objectResult.StatusCode);
        }

        [Fact]
        public async Task ObtenerTicket_IdNoValido()
        {
            // Arrange
            var id = -1;

            // Act
            var resultado = await _ticketController.ObtenerTicket(id);

            // Assert
            var resultadoBadRequest = Assert.IsType<BadRequestObjectResult>(resultado.Result);
            Assert.Equal("El ID debe ser un número positivo", resultadoBadRequest.Value);
        }

        [Fact]
        public async Task ObtenerTicket_IdNotFound()
        {
            // Arrange
            var id = 1;
            _mockTicketRepository.Setup(x => x.GetTicketByIdAsync(id)).ReturnsAsync((Ticket)null);

            // Act
            var resultado = await _ticketController.ObtenerTicket(id);

            // Assert
            var resultadoNotFound = Assert.IsType<NotFoundObjectResult>(resultado.Result);
            Assert.Equal("No se encontró ningún ticket con el ID proporcionado", resultadoNotFound.Value);
        }

        [Fact]
        public async Task ObtenerTicket_OK()
        {
            // Arrange
            var id = 1;
            var ticket = new Ticket { Id = id };
            _mockTicketRepository.Setup(x => x.GetTicketByIdAsync(id)).ReturnsAsync(ticket);

            // Act
            var resultado = await _ticketController.ObtenerTicket(id);

            // Assert
            var resultadoOk = Assert.IsType<OkObjectResult>(resultado.Result);
            var ticketResult = Assert.IsType<Ticket>(resultadoOk.Value);
            Assert.Equal(id, ticketResult.Id);
        }

        [Fact]
        public async Task FiltrarTickets_InternalServerError()
        {
            // Arrange
            var mockRepository = new Mock<ITicketRepository>();
            mockRepository.Setup(repo => repo.GetTicketByUserAsync("john.doe")).ThrowsAsync(new Exception("Repository exception"));
            var controller = new TicketController(mockRepository.Object);

            // Act
            var result = await controller.FiltrarTickets("john.doe");

            // Assert
            var internalServerErrorResult = Assert.IsType<ObjectResult>(result.Result);
            Assert.Equal(StatusCodes.Status500InternalServerError, internalServerErrorResult.StatusCode);
        }

        [Fact]
        public async Task CrearTicket_TicketIsNull()
        {
            // Arrange
            var mockRepository = new Mock<ITicketRepository>();
            var controller = new TicketController(mockRepository.Object);

            // Act
            var result = await controller.CrearTicket(null);

            // Assert
            var actionResult = Assert.IsType<ActionResult<Ticket>>(result);
            var badRequestResult = Assert.IsType<BadRequestResult>(actionResult.Result);
            Assert.Equal(StatusCodes.Status400BadRequest, badRequestResult.StatusCode);
        }

        [Fact]
        public async Task CrearTicket_TicketHasId()
        {
            // Arrange
            var ticket = new Ticket { Id = 1 };
            var mockRepository = new Mock<ITicketRepository>();
            var controller = new TicketController(mockRepository.Object);

            // Act
            var result = await controller.CrearTicket(ticket);

            // Assert
            var actionResult = Assert.IsType<ActionResult<Ticket>>(result);
            var badRequestResult = Assert.IsType<BadRequestResult>(actionResult.Result);
            Assert.Equal(StatusCodes.Status400BadRequest, badRequestResult.StatusCode);
        }

        [Fact]
        public async Task CrearTicket_ModelStateIsInvalid()
        {
            // Arrange
            var ticket = new Ticket { };
            var mockRepository = new Mock<ITicketRepository>();
            var controller = new TicketController(mockRepository.Object);
            controller.ModelState.AddModelError("Usuario", "El campo Usuario es obligatorio");

            // Act
            var result = await controller.CrearTicket(ticket);

            // Assert
            var actionResult = Assert.IsType<ActionResult<Ticket>>(result);
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(actionResult.Result);
            Assert.Equal(StatusCodes.Status400BadRequest, badRequestResult.StatusCode);
            var errors = controller.ModelState;
            Assert.True(errors.ContainsKey("Usuario"));
        }

        [Fact]
        public async Task CrearTicket_Created()
        {
            // Arrange
            var ticket = new Ticket { Usuario = "john.doe" };
            var mockRepository = new Mock<ITicketRepository>();
            mockRepository.Setup(repo => repo.AddTicketAsync(ticket)).Returns(Task.CompletedTask);
            var controller = new TicketController(mockRepository.Object);

            // Act
            var result = await controller.CrearTicket(ticket);

            // Assert
            var actionResulte = Assert.IsType<ActionResult<Ticket>>(result);
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(actionResulte.Result);
            Assert.Equal(StatusCodes.Status201Created, createdAtActionResult.StatusCode);
            Assert.Equal(nameof(TicketController.ObtenerTicket), createdAtActionResult.ActionName);
            Assert.Equal(ticket.Id, createdAtActionResult.RouteValues["id"]);
            Assert.Equal(ticket, createdAtActionResult.Value);
        }

        [Fact]
        public async Task CrearTicket_InternalServerError()
        {
            // Arrange
            var ticket = new Ticket { Usuario = "john.doe" };
            var mockRepository = new Mock<ITicketRepository>();
            mockRepository.Setup(repo => repo.AddTicketAsync(ticket)).ThrowsAsync(new InvalidOperationException("Database connection failed"));
            var controller = new TicketController(mockRepository.Object);

            // Act
            var result = await controller.CrearTicket(ticket);

            // Assert
            var internalServerErrorResult = Assert.IsType<ObjectResult>(result.Result);
            Assert.Equal(StatusCodes.Status500InternalServerError, internalServerErrorResult.StatusCode);

        }

        [Fact]
        public async Task UpdateTicket_Ok()
        {
            // Arrange
            var ticket = new Ticket { Id = 1, Usuario = "johndoe", Estatus = "Abierto", FechaCreacion = DateTime.UtcNow };
            _mockTicketRepository.Setup(repo => repo.GetTicketByIdAsync(ticket.Id)).ReturnsAsync(ticket);
            _mockTicketRepository.Setup(repo => repo.UpdateTicketAsync(It.IsAny<Ticket>())).Returns(Task.CompletedTask);

            // Act
            var result = await _ticketController.UpdateTicket(ticket);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var model = Assert.IsType<Ticket>(okResult.Value);
            Assert.Equal(ticket.Usuario, model.Usuario);
            Assert.Equal(ticket.Estatus, model.Estatus);
            Assert.Equal(ticket.FechaCreacion, model.FechaCreacion);
        }

        [Fact]
        public async Task UpdateTicket_TicketNotFound()
        {
            // Arrange
            var ticket = new Ticket { Id = 1, Usuario = "johndoe", Estatus = "Abierto", FechaCreacion = DateTime.UtcNow };
            _mockTicketRepository.Setup(repo => repo.GetTicketByIdAsync(ticket.Id)).ReturnsAsync((Ticket)null);

            // Act
            var result = await _ticketController.UpdateTicket(ticket);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task UpdateTicket_InvalidModel()
        {
            // Arrange
            var ticket = new Ticket { Id = 1, Usuario = "johndoe", Estatus = "Abierto", FechaCreacion = DateTime.UtcNow };
            _ticketController.ModelState.AddModelError("Usuario", "The Usuario field is required");

            // Act
            var result = await _ticketController.UpdateTicket(ticket);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task UpdateTicket_NullTicket()
        {
            // Arrange
            Ticket ticket = null;

            // Act
            var result = await _ticketController.UpdateTicket(ticket);

            // Assert
            Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public async Task UpdateTicket_InternalServerError()
        {
            // Arrange
            var ticket = new Ticket { Id = 1, Usuario = "johndoe", Estatus = "Abierto", FechaCreacion = DateTime.UtcNow };
            _mockTicketRepository.Setup(repo => repo.GetTicketByIdAsync(ticket.Id)).ThrowsAsync(new InvalidOperationException("Database connection failed"));

            // Act
            var result = await _ticketController.UpdateTicket(ticket);

            // Assert
            var internalServerErrorResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(StatusCodes.Status500InternalServerError, internalServerErrorResult.StatusCode);
        }

        [Fact]
        public async Task Delete_TicketNotFound()
        {
            // Arrange
            int id = 1;
            _mockTicketRepository.Setup(repo => repo.GetTicketByIdAsync(id)).ReturnsAsync(null as Ticket);

            // Act
            var result = await _ticketController.Delete(id);

            // Assert
            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task Delete_TicketDeleted()
        {
            // Arrange
            int id = 1;
            var existingTicket = new Ticket { Id = id };
            _mockTicketRepository.Setup(repo => repo.GetTicketByIdAsync(id)).ReturnsAsync(existingTicket);

            // Act
            var result = await _ticketController.Delete(id);

            // Assert
            _mockTicketRepository.Verify(repo => repo.DeleteTicketAsync(existingTicket), Times.Once);
            Assert.IsType<NoContentResult>(result.Result);
        }

        [Fact]
        public async Task Delete_Exception()
        {
            // Arrange
            int id = 1;
            _mockTicketRepository.Setup(repo => repo.GetTicketByIdAsync(id)).ThrowsAsync(new Exception());

            // Act
            var result = await _ticketController.Delete(id);

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result.Result);
            Assert.Equal(StatusCodes.Status500InternalServerError, statusCodeResult.StatusCode);
        }
    }
}

