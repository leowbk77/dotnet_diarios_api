using Diarios.Api.Models;

namespace Diarios.Api.Repository.Interface
{
    public interface IDiarioRepository
    {
        public DiarioModel GetDiarioById(int id, string cidade);
        public List<DiarioModel> SearchDiarios(string query, string cidade);
        //public List<DiarioModel> SearchForTermsOnEdicao(SearchQueryModel search, string cidade);
        //public List<DiarioModel> SearchForTermsOnAllDocs(SearchQueryModel search, string cidade);
    }
}
