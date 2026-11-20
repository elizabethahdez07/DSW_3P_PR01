using Microsoft.EntityFrameworkCore;
using DSW_3P_PR01.Models;

namespace DSW_3P_PR01.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(
        DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Mision> Misiones => Set<Mision>();
    
}