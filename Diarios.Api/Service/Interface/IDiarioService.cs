using Diarios.Api.Models;

namespace Diarios.Api.Service.Interface
{
    public interface IDiarioService
    {
        DiarioModel GetDiarioById(int id, string cidade);
        Task<ResponseModel> SearchDiariosAsync(SearchQueryModel query);
        Task<DiarioModel> SearchForLatestAsync(string cidade);
    }
}
