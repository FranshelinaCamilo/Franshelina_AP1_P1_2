
using Microsoft.Playwright;
using Xunit;

namespace Franshelina_AP1_P1_2.Tests;

public class UnitTest1
{
    private const string UrlBase = "http://localhost:5250";

    // PRUEBA 1: Mostrar el título del listado
    [Fact]
    public async Task AutoresIndex_DebeMostrarElTitulo()
    {
        using var playwright = await Playwright.CreateAsync();

        await using var browser = await playwright.Chromium.LaunchAsync(
            new BrowserTypeLaunchOptions { Headless = true });

        var pagina = await browser.NewPageAsync();

        await pagina.GotoAsync($"{UrlBase}/Autores/Index");

        await pagina.GetByText(
            "Registro de Autores",
            new PageGetByTextOptions { Exact = true }
        ).WaitForAsync();
    }

    // PRUEBA 2: Registrar un autor
    [Fact]
    public async Task AutoresCreate_DebeRegistrarUnAutor()
    {
        using var playwright = await Playwright.CreateAsync();

        await using var browser = await playwright.Chromium.LaunchAsync(
            new BrowserTypeLaunchOptions { Headless = true });

        var pagina = await browser.NewPageAsync();

        string nombre = $"Autor Prueba {Guid.NewGuid():N}";

        await CrearAutorAsync(pagina, nombre);

        await pagina.GetByText(
            nombre,
            new PageGetByTextOptions { Exact = true }
        ).WaitForAsync();
    }

    // PRUEBA 3: Modificar un autor
    [Fact]
    public async Task AutoresEdit_DebeModificarUnAutor()
    {
        using var playwright = await Playwright.CreateAsync();

        await using var browser = await playwright.Chromium.LaunchAsync(
            new BrowserTypeLaunchOptions { Headless = true });

        var pagina = await browser.NewPageAsync();

        string nombreOriginal = $"Autor Original {Guid.NewGuid():N}";
        string urlEdicion = await CrearAutorAsync(pagina, nombreOriginal);

        await pagina.GotoAsync(urlEdicion);

        var nombres = pagina.Locator("input[name='Autor.Nombres']");

        await nombres.WaitForAsync();

        await pagina.WaitForFunctionAsync(
            @"() => document.querySelector(""input[name='Autor.Nombres']"")?.value === " +
            System.Text.Json.JsonSerializer.Serialize(nombreOriginal));

        string nombreModificado = $"Autor Modificado {Guid.NewGuid():N}";

        await nombres.FillAsync(nombreModificado);

        await pagina.Locator("input[name='Autor.Nacionalidad']")
            .FillAsync("Española");

        await pagina.Locator("input[name='Autor.FechaNacimiento']")
            .FillAsync("1995-08-20");

        await pagina.Locator("input[name='Autor.Sueldo']")
            .FillAsync("30000");

        await pagina.GetByRole(
            AriaRole.Button,
            new PageGetByRoleOptions { Name = "Guardar" }
        ).ClickAsync();

        await pagina.WaitForURLAsync("**/Autores/Index");

        await pagina.GetByText(
            "Autor Modificado Exitosamente",
            new PageGetByTextOptions { Exact = true }
        ).WaitForAsync();

        await pagina.GetByText(
            nombreModificado,
            new PageGetByTextOptions { Exact = true }
        ).WaitForAsync();
    }

    // PRUEBA 4: Eliminar un autor
    [Fact]
    public async Task AutoresDelete_DebeEliminarUnAutor()
    {
        using var playwright = await Playwright.CreateAsync();

        await using var browser = await playwright.Chromium.LaunchAsync(
            new BrowserTypeLaunchOptions { Headless = true });

        var pagina = await browser.NewPageAsync();

        string nombre = $"Autor Eliminar {Guid.NewGuid():N}";
        string urlEdicion = await CrearAutorAsync(pagina, nombre);

        await pagina.GotoAsync(urlEdicion);

        var nombres = pagina.Locator("input[name='Autor.Nombres']");

        await nombres.WaitForAsync();

        await pagina.WaitForFunctionAsync(
            @"() => document.querySelector(""input[name='Autor.Nombres']"")?.value === " +
            System.Text.Json.JsonSerializer.Serialize(nombre));

        await pagina.GetByRole(
            AriaRole.Button,
            new PageGetByRoleOptions { Name = "Eliminar" }
        ).First.ClickAsync();

        var modal = pagina.Locator("#modalEliminar");

        await pagina.Locator("#modalEliminar.show").WaitForAsync();

        await modal.GetByRole(
            AriaRole.Button,
            new LocatorGetByRoleOptions { Name = "Eliminar" }
        ).ClickAsync();

        await pagina.WaitForURLAsync("**/Autores/Index");

        await pagina.GetByText(
            "Autor Eliminado Exitosamente",
            new PageGetByTextOptions { Exact = true }
        ).WaitForAsync();

        await pagina.GetByText(
            nombre,
            new PageGetByTextOptions { Exact = true }
        ).WaitForAsync(new LocatorWaitForOptions
        {
            State = WaitForSelectorState.Detached
        });
    }

    private static async Task<string> CrearAutorAsync(
        IPage pagina,
        string nombre)
    {
        await pagina.GotoAsync($"{UrlBase}/Autores/Create");

        await pagina.Locator("input[name='Autor.Nombres']")
            .WaitForAsync();

        await pagina.Locator("input[name='Autor.Nombres']")
            .FillAsync(nombre);

        await pagina.Locator("input[name='Autor.Nacionalidad']")
            .FillAsync("Dominicana");

        await pagina.Locator("input[name='Autor.FechaNacimiento']")
            .FillAsync("1990-05-15");

        await pagina.Locator("input[name='Autor.Sueldo']")
            .FillAsync("25000");

        await pagina.GetByRole(
            AriaRole.Button,
            new PageGetByRoleOptions { Name = "Guardar" }
        ).ClickAsync();

        await pagina.WaitForURLAsync("**/Autores/Index");

        var fila = pagina.GetByRole(AriaRole.Row).Filter(
            new LocatorFilterOptions { HasText = nombre });

        await fila.WaitForAsync();

        var enlace = fila.Locator("a[href*='/Autores/Edit/']");
        string? urlEdicion = await enlace.GetAttributeAsync("href");

        if (string.IsNullOrWhiteSpace(urlEdicion))
        {
            throw new Exception(
                "No se encontró el enlace de edición del autor creado. " +
                "Verifica el enlace de edición en Autores/Index.razor.");
        }

        return new Uri(new Uri($"{UrlBase}/"), urlEdicion).ToString();
    }
}
