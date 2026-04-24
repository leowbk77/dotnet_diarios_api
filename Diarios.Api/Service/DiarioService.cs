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

        public List<DiarioModel> SearchDiarios(SearchQueryModel search, string cidade)
        {
            string query = "";
            int lastId = search.lastId ?? 0;
            //List<DiarioModel> diarios;

            if (search.terms != null)
            {
                search.terms = search.terms?.Replace('+', ' ');

                if (search.edicao != null)
                {
                    query = QueryBuilder.SearchForTermsOnEdicao(search);
                    //diarios = _repository.SearchDiarios(QueryBuilder.SearchForTermsOnEdicao(search), cidade);
                } else
                {
                    query = QueryBuilder.SearchForTermsOnAllDocs(search);
                    //diarios = _repository.SearchDiarios(QueryBuilder.SearchForTermsOnAllDocs(search), cidade);
                }
                return _repository.SearchDiarios(query, cidade);
            }

            return new List<DiarioModel>();
        }
    }
}
