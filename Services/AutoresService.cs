using Aplicada1.Core;
using Franshelina_AP1_P1_2.Models;
using Franshelina_AP1_P1_2.Context;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Franshelina_AP1_P1_2.Services;

public class AutoresService(IDbContextFactory<Contexto> contextFactory) : IService<Autores, int>
{
    public async Task<bool> Guardar(Autores autor)
    {
        if (!await Existe(autor.IdAutor))
        {
            return await Insertar(autor);
        }
        else
        {
            return await Modificar(autor);
        }
    }

    public async Task<bool> Existe(int idAutor)
    {
        await using var contexto = await contextFactory.CreateDbContextAsync();
        return await contexto.Autores
            .AnyAsync(a => a.IdAutor == idAutor);
    }

    public async Task<bool> Insertar(Autores autor)
    {
        await using var contexto = await contextFactory.CreateDbContextAsync();
        contexto.Autores.Add(autor);
        return await contexto.SaveChangesAsync() > 0;
    }

    public async Task<bool> Modificar(Autores autor)
    {
        await using var contexto = await contextFactory.CreateDbContextAsync();
        contexto.Update(autor);
        return await contexto.SaveChangesAsync() > 0;
    }

    public async Task<Autores?> Buscar(int idAutor)
    {
        await using var contexto = await contextFactory.CreateDbContextAsync();
        return await contexto.Autores
            .FirstOrDefaultAsync(a => a.IdAutor == idAutor);
    }

    public async Task<List<Autores>> GetList(Expression<Func<Autores, bool>> criterio)
    {
        await using var contexto = await contextFactory.CreateDbContextAsync();
        return await contexto.Autores
            .Where(criterio)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<bool> Eliminar(int idAutor)
    {
        await using var contexto = await contextFactory.CreateDbContextAsync();
        return await contexto.Autores
            .Where(a => a.IdAutor == idAutor)
            .ExecuteDeleteAsync() > 0;
    }
}
