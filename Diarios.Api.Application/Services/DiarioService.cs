using Diarios.Api.Application.Util.QueryBuilder;
using Diarios.Api.Application.Util.CustomException;
using Diarios.Api.Domain.Contracts.Repository;
using Diarios.Api.Domain.Contracts.Service;
using Diarios.Api.Domain.Models.Entities;
using Diarios.Api.Domain.Models.Requests;
using Diarios.Api.Domain.Models.Responses;
using Microsoft.AspNetCore.Http;

namespace Diarios.Api.Application.Services
{
    public class DiarioService : IDiarioService
    {
        private readonly IDiarioRepository _repository;

        public DiarioService(IDiarioRepository repository)
        {
            _repository = repository;
        }

        public Diario GetDiarioById(int id, string cidade)
        {
            return _repository.GetDiarioById(id, cidade);
        }

        public async Task<SearchResponse> SearchDiariosAsync(SearchRequest search)
        {
            if (search.Terms != null) search.Terms = search.Terms?.Replace('+', ' ');

            List<int> ids = await _repository.SearchForDiariosIdsAsync(QueryBuilder.GetDocsIds(search), search.Cidade);

            bool hasMore = ids.Count > search.Limit;
            if (hasMore) ids = ids.Take(search.Limit).ToList();

            var response = await _repository.SearchDiariosByIdListAsync(QueryBuilder.GetPaginasByDocIds(search, ids), search.Cidade);

            return new SearchResponse
            {
                SearchDiariosResults = response,
                HasMore = hasMore
            };
        }

        public async Task<Diario> SearchForLatestAsync(string cidade)
        {
            var response = await _repository.SearchForLatestAsync(cidade);
            if (response == null)
            {
                throw new DiarioCustomException(StatusCodes.Status404NotFound);
            }
            return response;
        }

    }
}
