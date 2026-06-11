using Diarios.Api.Models;
using Diarios.Api.Repository;
using Diarios.Api.Repository.Interface;
using Diarios.Api.Service.Interface;

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

        public List<SearchDiariosResultModel> SearchDiarios(SearchQueryModel search)
        {
            string query = "";
            List<SearchDiariosResultModel> diarios = new List<SearchDiariosResultModel>();

            if (search.terms != null)
            {
                search.terms = search.terms?.Replace('+', ' ');

                List<int> ids = _repository.SearchForDiariosIds(QueryBuilder.GetDocsIds(search), search.cidade);

                // usar para o front saber se tem mais a ser buscado.
                // criar um response model com um hasMore e o Data dos diarios.
                bool hasMore = ids.Count > search.limit;
                if (hasMore) ids = ids.Take(search.limit).ToList();

                return _repository.SearchDiariosByIdList(QueryBuilder.GetPaginasByDocIds(search, ids), search.cidade);
            }

            return new List<SearchDiariosResultModel>();
        }

        //uso futuro
        public async Task<List<SearchDiariosResultModel>> SearchDiariosAsync(SearchQueryModel search)
        {
            return new List<SearchDiariosResultModel>();
        }
    }
}
