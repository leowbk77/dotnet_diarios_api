
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Diarios.Api.Domain.Models.Requests
{
    public class SearchRequest
    {
        [Description("termos de busca textual")]
        public string? Terms { get; set; }
        public DateOnly? DtInicial { get; set; } //data inicial de filtragem
        public DateOnly? DtFinal { get; set; } //data final de filtragem
        public string? Edicao { get; set; } //termo de busca por edicao especifica
        public int? LastDocId { get; set; } //ultimo id de arquivo pesquisado - para paginação
        public DateOnly LastDocDtEdicao { get; set; } = DateOnly.FromDateTime(DateTime.Now); //data da ultima edicao - para paginação
        public int Limit { get; set; } = 10; //limite de documentos a serem buscados
        [Required]
        public string Cidade { get; set; } = String.Empty;
    }
}
