
using Diarios.Api.Domain.Models.Entities;

namespace Diarios.Api.Domain.Models.Responses
{
    public class SearchResponse
    {
        public List<SearchDiariosResultModel> SearchDiariosResults { get; set; } = new();
        public bool HasMore { get; set; } = false;
    }

    public class SearchDiariosResultModel : Diario
    {
        public List<PaginaModel> Paginas { get; set; } = new();
    }

    public class PaginaModel
    {
        public int Numero { get; set; }
        public string Conteudo { get; set; } = String.Empty;
    }
}
