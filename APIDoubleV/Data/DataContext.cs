using APIDoubleV.Models;
using Microsoft.EntityFrameworkCore;

namespace APIDoubleV.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base (options){ }

        public DbSet<Ticket> Tickets { get; set; }
    }
}
