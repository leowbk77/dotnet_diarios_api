
namespace Diarios.Api.Domain.Models.Responses
{
    public class SearchResponse
    {
        public List<SearchDiariosResultModel> SearchDiariosResults { get; set; } = new();
        public bool HasMore { get; set; } = false;
    }

    public class SearchDiariosResultModel
    {
        public int Id { get; set; }
        public string NmEdicao { get; set; } = String.Empty;
        public string Caminho { get; set; } = String.Empty;
        public int Ano { get; set; }
        public int Mes { get; set; }
        public int Dia { get; set; }
        public DateOnly Data { get; set; } = new(); // adição futura
        public List<PaginaModel> Paginas { get; set; } = new();
    }

    public class PaginaModel
    {
        public int Numero { get; set; }
        public string Conteudo { get; set; } = String.Empty;
    }
}
