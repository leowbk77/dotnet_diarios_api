using Diarios.Api.Domain.Models.Entities;
using Diarios.Api.Domain.Models.Requests;
using Diarios.Api.Domain.Models.Responses;

namespace Diarios.Api.Domain.Contracts.Service
{
    public interface IDiarioService
    {
        Diario GetDiarioById(int id, string cidade);
        Task<SearchResponse> SearchDiariosAsync(SearchRequest query);
        Task<Diario> SearchForLatestAsync(string cidade);
    }
}
