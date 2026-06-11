namespace Diarios.Api.Models
{
    public class ResponseModel
    {
        public List<SearchDiariosResultModel> SearchDiariosResults { get; set; } = new();
        public bool HasMore { get; set;} = false;
    }

    public class SearchDiariosResultModel
    {
        public int Id { get; set; }
        public string NmEdicao { get; set; } = String.Empty;
        public string Caminho {  get; set; } = String.Empty;
        public int Ano { get; set; }
        public int Mes { get; set; }
        public int Dia { get; set; }
        public List<PaginaModel> Paginas { get; set; }
    }

    public class PaginaModel
    {
        public int Numero { get; set; }
        public string Conteudo { get; set; } = String.Empty;
    }
}
