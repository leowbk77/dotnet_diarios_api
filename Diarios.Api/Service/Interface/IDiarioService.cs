using Diarios.Api.Models;

namespace Diarios.Api.Service.Interface
{
    public interface IDiarioService
    {
        DiarioModel GetDiarioById(int id, string cidade);
        ResponseModel SearchDiarios(SearchQueryModel query);
    }
}
