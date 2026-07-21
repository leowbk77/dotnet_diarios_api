using Diarios.Api.Infra.Repository;
using Diarios.Api.Domain.Contracts.Repository;
using Diarios.Api.Application.Services;

using Diarios.Api.Domain.Contracts.Service;
using Diarios.Api.Application.Util.Provider.Interface;
using Diarios.Api.Application.Util.Provider;

using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;

namespace Diarios.Api.Ioc
{
    [ExcludeFromCodeCoverage]
    public static class ServiceExtensions
    {
        public static void AddDependencies(this IServiceCollection services)
        {
            services.AddScoped<ICidadeProvider, CidadeProvider>();
            services.AddScoped<IDiarioRepository, DiarioRepository>();
            services.AddScoped<IDiarioService, DiarioService>();
        }
    }
}
