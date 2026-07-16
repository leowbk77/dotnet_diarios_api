
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Diarios.Api.Domain.Models.Requests
{
    public class SearchRequest
    {
        [Description("termos de busca textual")]
        public string? Terms { get; set; }
        //data inicial de filtragem
        public DateOnly? DtInicial { get; set; }
        //data final de filtragem
        public DateOnly? DtFinal { get; set; }
        //termo de busca por edicao especifica
        public string? Edicao { get; set; }
        //ultimo id de arquivo pesquisado - para paginação
        public int? LastDocId { get; set; }
        //limite de documentos a serem buscados
        public int Limit { get; set; } = 10;
        [Required]
        public string Cidade { get; set; } = String.Empty;
    }
}
