using Franshelina_AP1_P1_2.Models;
using Microsoft.EntityFrameworkCore;

namespace Franshelina_AP1_P1_2.Context;

public class Contexto : DbContext
{
    public Contexto(DbContextOptions<Contexto> options) : base(options) { }

    public DbSet<Autores> Autores { get; set; } = null!;
}
