using System.Diagnostics.CodeAnalysis;
using System.Net;

namespace Diarios.Api.Application.Util.CustomException
{
    [ExcludeFromCodeCoverage]
    public class DatabaseNotFoundException : DiarioCustomException
    {
        public DatabaseNotFoundException() 
        {
        }

        public DatabaseNotFoundException(string dataBase) : base( ((int)System.Net.HttpStatusCode.NotFound), $"DataBase: {dataBase} não foi inicializado: não há registros")
        { 
        }

        public DatabaseNotFoundException(string? dataBase, Exception? innerException) : base(((int)System.Net.HttpStatusCode.NotFound), $"DataBase: {dataBase} não foi inicializado: não há registros", innerException)
        {
        }
    }
}
