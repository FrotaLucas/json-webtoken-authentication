using Microsoft.EntityFrameworkCore;
using WebAPI_JsonWeb_Token_Auth.Models;

namespace WebAPI_JsonWeb_Token_Auth.DataContext
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<UsuarioModel> Usuario { get; set; }

    }
}
