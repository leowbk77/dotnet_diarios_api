using Diarios.Api.Models;

namespace Diarios.Api.Repository.Interface
{
    public interface IDiarioRepository
    {
        public DiarioModel GetDiarioById(int id, string cidade);
        public Task<List<int>> SearchForDiariosIdsAsync(string query, string cidade);
        public Task<List<SearchDiariosResultModel>> SearchDiariosByIdListAsync(string query, string cidade);
        public Task<DiarioModel?> SearchForLatestAsync(string cidade);
    }
}
