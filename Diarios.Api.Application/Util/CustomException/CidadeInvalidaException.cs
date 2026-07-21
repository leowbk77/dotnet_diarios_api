using System.Diagnostics.CodeAnalysis;
using System.Net;

namespace Diarios.Api.Application.Util.CustomException
{
    [ExcludeFromCodeCoverage]
    public class CidadeInvalidaException : DiarioCustomException
    {
        protected string? cidade;
        public CidadeInvalidaException() 
        { 
            this.cidade = null;
        }

        public CidadeInvalidaException(string cidade) : base(((int)HttpStatusCode.BadRequest),$"Cidade: {cidade} invalida")
        {
            this.cidade = cidade;
        }

        public CidadeInvalidaException(string? cidade, Exception? innerException) : base(((int)HttpStatusCode.BadRequest),$"Cidade: {cidade} invalida", innerException)
        {
            this.cidade = cidade;
        }
    }
}
