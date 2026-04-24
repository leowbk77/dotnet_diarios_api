using System.Net;

namespace Diarios.Api.Util.CustomException
{
    public class DatabaseNotFoundException : DiarioCustomException
    {
        public DatabaseNotFoundException() 
        {
        }

        public DatabaseNotFoundException(string dataBase) : base( ((int)HttpStatusCode.NotFound), $"DataBase: {dataBase} não foi inicializado: não há registros")
        { 
        }

        public DatabaseNotFoundException(string? dataBase, Exception? innerException) : base(((int)HttpStatusCode.NotFound), $"DataBase: {dataBase} não foi inicializado: não há registros", innerException)
        {
        }
    }
}
