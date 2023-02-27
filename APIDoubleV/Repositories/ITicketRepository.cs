using APIDoubleV.Models;

namespace APIDoubleV.Repositories
{
    public interface ITicketRepository
    {
        Task<Ticket> GetTicketByIdAsync(int id);
        Task<List<Ticket>> GetAllTicketAsync();
        Task<List<Ticket>> GetTicketByUserAsync(string user);
        Task AddTicketAsync(Ticket ticket);
        Task DeleteTicketAsync(Ticket ticket);
        Task UpdateTicketAsync(Ticket ticket);
    }

}