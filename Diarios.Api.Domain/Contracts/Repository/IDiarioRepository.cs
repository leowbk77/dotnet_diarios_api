using Diarios.Api.Domain.Models;
using Diarios.Api.Domain.Models.Entities;
using Diarios.Api.Domain.Models.Responses;

namespace Diarios.Api.Domain.Contracts.Repository
{
    public interface IDiarioRepository
    {
        public Task<Diario> GetDiarioById(int id, string cidade);
        public Task<List<int>> SearchForDiariosIdsAsync(QueryDefinition query, string cidade);
        public Task<List<SearchDiariosResultModel>> SearchDiariosByIdListAsync(QueryDefinition query, string cidade);
        public Task<Diario?> SearchForLatestAsync(string cidade);
        public Task<IndexStatusResponse> SearchForIndexStatusAsync(string cidade);
    }
}
