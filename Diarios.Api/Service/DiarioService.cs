using Diarios.Api.Models;
using Diarios.Api.Repository;
using Diarios.Api.Repository.Interface;
using Diarios.Api.Service.Interface;
using Diarios.Api.Util.CustomException;

namespace Diarios.Api.Service
{
    public class DiarioService : IDiarioService
    {
        private readonly IDiarioRepository _repository;

        public DiarioService(IDiarioRepository repository)
        {
            _repository = repository;
        }

        public DiarioModel GetDiarioById(int id, string cidade)
        {
            return _repository.GetDiarioById(id, cidade);
        }

        public async Task<ResponseModel> SearchDiariosAsync(SearchQueryModel search)
        {
            List<SearchDiariosResultModel> diarios = new List<SearchDiariosResultModel>();

            if (search.terms != null)
            {
                search.terms = search.terms?.Replace('+', ' ');

                List<int> ids = await _repository.SearchForDiariosIdsAsync(QueryBuilder.GetDocsIds(search), search.cidade);

                bool hasMore = ids.Count > search.limit;
                if (hasMore) ids = ids.Take(search.limit).ToList();
                var response = await _repository.SearchDiariosByIdListAsync(QueryBuilder.GetPaginasByDocIds(search, ids), search.cidade);
                return new ResponseModel
                {
                    SearchDiariosResults = response,
                    HasMore = hasMore
                };
            }

            return new();
        }

        public async Task<DiarioModel> SearchForLatestAsync(string cidade)
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
