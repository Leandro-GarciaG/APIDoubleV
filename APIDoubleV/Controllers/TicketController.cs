using APIDoubleV.Data;
using APIDoubleV.Models;
using APIDoubleV.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace APIDoubleV.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class TicketController : ControllerBase
    {
        
        private readonly ITicketRepository _ticketRepository;
        

        public TicketController(ITicketRepository ticketRepository)
        {
            _ticketRepository = ticketRepository;
        }

        

        [HttpGet]
        public async Task<ActionResult<List<Ticket>>> ObtenerTickets(int pageNumber = 1, int pageSize =10)
        {
            try
            {
                if (pageNumber <= 0 || pageSize <= 0)
                {
                    return BadRequest("pageNumber y pageSize deben ser números positivos");
                }

                var tickets = await _ticketRepository.GetAllTicketAsync();
                var paginacionTickets = tickets.Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();
                return Ok(paginacionTickets);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Ocurrió un error al obtener los tickets. Por favor, inténtelo de nuevo más tarde {ex.Message}");
            }
        }

        [HttpGet]
        public async Task<ActionResult<Ticket>> ObtenerTicket(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest("El ID debe ser un número positivo");
                }

                var ticket = await _ticketRepository.GetTicketByIdAsync(id);

                if (ticket == null)
                {
                    return NotFound("No se encontró ningún ticket con el ID proporcionado");
                }

                return Ok(ticket);
            }
            catch (Exception ex)
            {
                var errorMessage = $"Ocurrió un error al obtener el ticket. Por favor, inténtelo de nuevo más tarde. Detalles del error: {ex.Message}";
                return StatusCode(StatusCodes.Status500InternalServerError, errorMessage);
            }
        }

        [HttpGet]
        public async Task<ActionResult<List<Ticket>>> FiltrarTickets(string usuario)
        {
            try
            {
                if (string.IsNullOrEmpty(usuario))
                {
                    return BadRequest("El parámetro de usuario no puede ser nulo o vacío");
                }
                var tickets = await _ticketRepository.GetTicketByUserAsync(usuario);

                if (tickets == null || tickets.Count == 0)
                {
                    return NotFound();
                }

                var response = new Dictionary<string, object>
                {
                    { "Usuario", usuario },
                    { "Tickets", tickets }
                };

                return Ok(response);
            }
            
            catch(Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex);
            }
        }
        
        [HttpPost]
        public async Task<ActionResult<Ticket>> CrearTicket(Ticket ticket)
        {
            if (ticket == null || ticket.Id != 0)
            {
                return BadRequest();
            }

            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                ticket.FechaCreacion = DateTime.UtcNow;
                ticket.FechaActualizacion = ticket.FechaCreacion;

                await _ticketRepository.AddTicketAsync(ticket);

                return CreatedAtAction(nameof(ObtenerTicket), new { id = ticket.Id }, ticket);
            }
            catch(Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex);
            }
        }

        [HttpPut]
        public async Task<IActionResult> UpdateTicket(Ticket ticket)
        {
            if (ticket == null)
            {
                return BadRequest();
            }

            try
            {
                var existingTicket = await _ticketRepository.GetTicketByIdAsync(ticket.Id);

                if (existingTicket == null)
                {
                    return NotFound();
                }

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                existingTicket.Usuario = ticket.Usuario;
                existingTicket.FechaActualizacion = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Utc);
                existingTicket.FechaCreacion = DateTime.SpecifyKind(ticket.FechaCreacion, DateTimeKind.Utc);
                existingTicket.Estatus = ticket.Estatus;

                await _ticketRepository.UpdateTicketAsync(existingTicket);

                return Ok(existingTicket);
            }

            catch(Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex);
            }         
        }
        
        [HttpDelete]
        public async Task<ActionResult<Ticket>> Delete(int id)
        {
            try
            {
                var existingTicket = await _ticketRepository.GetTicketByIdAsync(id);
                if (existingTicket == null)
                {
                    return NotFound();
                }
                await _ticketRepository.DeleteTicketAsync(existingTicket);
                return NoContent();
            }
            catch(Exception ex)
            {
                var problemDetails = new ProblemDetails
                {
                    Status = StatusCodes.Status500InternalServerError,
                    Title = "An error occurred while deleting the ticket.",
                    Detail = ex.Message
                };

                return StatusCode(StatusCodes.Status500InternalServerError, problemDetails);
            }
        }
    }
}
