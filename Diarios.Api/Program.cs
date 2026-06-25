using Diarios.Api.Controllers;
using Diarios.Api.Repository;
using Diarios.Api.Repository.Interface;
using Diarios.Api.Service;
using Diarios.Api.Service.Interface;
using Diarios.Api.Util.Provider;
using Diarios.Api.Util.Provider.Interface;
using Diarios.Api.IoC;
using Serilog;
using Scalar.AspNetCore;

namespace Diarios.Api
{
    public class Program
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
