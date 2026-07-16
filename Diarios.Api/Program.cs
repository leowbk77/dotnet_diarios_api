using Diarios.Api.Ioc;
using Serilog;
using Scalar.AspNetCore;

namespace Diarios.Api
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            Log.Information("Inicializando API de busca de diarios");
            Console.WriteLine("Inicializando API de busca de diarios");

            var builder = WebApplication.CreateBuilder(args);

            string env = GetEnvironmentVariable();
            Console.WriteLine(env);
            builder.Configuration.AddJsonFile($"appsettings.{env}.json", optional: true, reloadOnChange: true);

            builder.Host.UseSerilog((context, configuration) =>
            {
                configuration.ReadFrom.Configuration(context.Configuration);
                configuration.Enrich.WithProperty("AdditionalFields", $"env:{env}");
                configuration.Enrich.WithProperty("env", env);
            });

            builder.Services.ConfigureServices();

            var app = builder.Build();
            
            if (app.Environment.IsDevelopment())
            {
                Console.WriteLine("-------------------------------------development");

                app.MapOpenApi();
                app.MapScalarApiReference();
            }

            app.UseHttpsRedirection();

            app.UseCors(policy => policy.AllowAnyOrigin()
                                        .AllowAnyMethod()
                                        .AllowAnyHeader());

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }

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

        private static string GetEnvironmentVariable()
        {
            const string envVar = "ASPNETCORE_ENVIRONMENT";
            Log.Information($"Obtendo variável de ambiente '{envVar}'");
            Console.WriteLine($"Obtendo variável de ambiente '{envVar}'");
            return Environment.GetEnvironmentVariable(envVar, EnvironmentVariableTarget.User)
                ?? Environment.GetEnvironmentVariable(envVar, EnvironmentVariableTarget.Machine)
                ?? Environment.GetEnvironmentVariable(envVar, EnvironmentVariableTarget.Process)
                ?? string.Empty
                ;
        }
    }
}
