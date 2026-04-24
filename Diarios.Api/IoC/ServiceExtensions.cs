using Diarios.Api.Repository;
using Diarios.Api.Repository.Interface;
using Diarios.Api.Service;
using Diarios.Api.Service.Interface;
using Diarios.Api.Util.Provider;
using Diarios.Api.Util.Provider.Interface;
using Serilog;

namespace Diarios.Api.IoC
{
    public static class ServiceExtensions
    {
        public static void ConfigureServices(this IServiceCollection services)
        {
            Log.Information("Configurando serviços da aplicação");
            Console.WriteLine("Configurando servicos da aplicacao");
            
            services.AddControllers(); 
            services.AddOpenApi(options =>
            {
                options.AddDocumentTransformer((document, context, _) =>
                {
                    document.Info = new()
                    {
                        Title = "Diarios API",
                        Version = "0.1",
                        Description = """
                        API de busca dos diários oficiais municipais indexados.
                        """,
                        Contact = new()
                        {
                            Name = "",
                            Email = "",
                            Url = new Uri("https://documentoapi.example")
                        }
                    };
                    return Task.CompletedTask;
                });
            });
            services.AddDependencies();
        }

        public static void AddDependencies(this IServiceCollection services)
        {
            services.AddScoped<ICidadeProvider, CidadeProvider>();
            services.AddScoped<IDiarioRepository, DiarioRepository>();
            services.AddScoped<IDiarioService, DiarioService>();
        }
    }
}
