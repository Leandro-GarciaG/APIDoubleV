using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using APIDoubleV.Models;
using APIDoubleV.Data;

namespace APIDoubleV.Repositories
{
    public class TicketRepository : ITicketRepository
    {
        private readonly DataContext _context;
        public TicketRepository(DataContext context)
        {
            _context = context;
        }       

        public async Task<List<Ticket>> GetAllTicketAsync() => await _context.Tickets.OrderBy(t=> t.Id).ToListAsync();
       
        public async Task<Ticket> GetTicketByIdAsync(int id) => await _context.Tickets.FindAsync(id);
        
        public async Task<List<Ticket>> GetTicketByUserAsync(string user) => await _context.Tickets
            .Where(t => t.Usuario == user).ToListAsync();
        
        public async Task AddTicketAsync(Ticket ticket)
        {
            await _context.Tickets.AddAsync(ticket);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateTicketAsync(Ticket ticket)
        {
            _context.Entry(ticket).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteTicketAsync(Ticket ticket)
        {
            _context.Tickets.Remove(ticket);
            await _context.SaveChangesAsync();
        }

        public async Task<int> CountAsync()
        {
            return await _context.Tickets.CountAsync();
        }
    }
}

