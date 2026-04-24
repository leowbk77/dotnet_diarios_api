using Diarios.Api.Util.CustomException;
using Diarios.Api.Util.Provider.Interface;

namespace Diarios.Api.Util.Provider
{
    public class CidadeProvider : ICidadeProvider
    {
        private readonly IConfiguration _configuration;

        public CidadeProvider(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string GetConnectionString(string cidade)
        {
            var conn = _configuration.GetConnectionString(cidade);
            if (String.IsNullOrEmpty(conn))
                throw new CidadeInvalidaException(cidade);
            return conn;
        }
    }
}
