using Diarios.Api.Domain.Models.Entities;
using Diarios.Api.Domain.Models.Responses;

namespace Diarios.Api.Domain.Contracts.Repository
{
    public interface IDiarioRepository
    {
        public Diario GetDiarioById(int id, string cidade);
        public Task<List<int>> SearchForDiariosIdsAsync(string query, string cidade);
        public Task<List<SearchDiariosResultModel>> SearchDiariosByIdListAsync(string query, string cidade);
        public Task<Diario?> SearchForLatestAsync(string cidade);
    }
}
