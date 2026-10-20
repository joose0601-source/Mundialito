
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Mundialito.Application.Features.Equipos.Commands;
using Mundialito.Application.Features.Equipos.Queries;
using Mundialito.Application.Features.Goles;
using Mundialito.Application.Features.Jugadores.Commands;
using Mundialito.Application.Features.Jugadores.Queries;
using Mundialito.Application.Features.Partidos.Commands;
using Mundialito.Application.Features.Partidos.Queries;
using Mundialito.Application.Interfaces;
using Mundialito.Infrastructure.Persistence;
using Mundialito.Infrastructure.Repositories;
using Mundialito.Infrastructure.Services;


namespace Mundialito.Infrastructure.Extensions;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString =
            configuration.GetConnectionString("DefaultConnection");

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException(
                "No se encontró la cadena de conexión DefaultConnection.");
        }

        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(connectionString));




        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IEquipoRepository, EquipoRepository>();
        services.AddScoped<IEquipoQueryRepository, EquipoQueryRepository>();
        services.AddScoped<CrearEquipoCommandHandler>();
        services.AddScoped<ObtenerEquiposQueryHandler>();
        services.AddScoped<ObtenerEquipoPorIdQueryHandler>();
        services.AddScoped<ActualizarEquipoCommandHandler>();
        services.AddScoped<EliminarEquipoCommandHandler>();
        services.AddScoped<IJugadorRepository, JugadorRepository>();
        services.AddScoped<IJugadorQueryRepository, JugadorQueryRepository>();
        services.AddScoped<CrearJugadorCommandHandler>();
        services.AddScoped<ActualizarJugadorCommandHandler>();
        services.AddScoped<EliminarJugadorCommandHandler>();
        services.AddScoped<ObtenerJugadoresQueryHandler>();
        services.AddScoped<ObtenerJugadorPorIdQueryHandler>();
        services.AddScoped<IPartidoRepository, PartidoRepository>();
        services.AddScoped<IPartidoQueryRepository, PartidoQueryRepository>();
        services.AddScoped<CrearPartidoCommandHandler>();
        services.AddScoped<RegistrarResultadoCommandHandler>();
        services.AddScoped<ObtenerPartidosQueryHandler>();
        services.AddScoped<ObtenerPartidoPorIdQueryHandler>();

        services.AddScoped<IIdempotencyService, IdempotencyService>();
        services.AddScoped<IPosicionesQueryRepository, PosicionesQueryRepository>();
        services.AddScoped<IGolPartidoRepository, GolPartidoRepository>();
        services.AddScoped<RegistrarGolesCommandHandler>();
        services.AddScoped<IGoleadoresQueryRepository, GoleadoresQueryRepository>();
        return services;
    }
}
