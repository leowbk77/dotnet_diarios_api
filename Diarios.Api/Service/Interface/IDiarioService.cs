using Diarios.Api.Models;

namespace Diarios.Api.Service.Interface
{
    public interface IDiarioService
    {
        DiarioModel GetDiarioById(int id, string cidade);
        List<DiarioModel> SearchDiarios(SearchQueryModel query, string cidade);
    }
}
