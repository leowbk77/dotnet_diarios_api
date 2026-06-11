using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Diarios.Api.Models
{
    [Description("Modelo de busca")]
    public class SearchQueryModel
    {
        [Description("termos de busca textual")]
        public string? terms { get; set; }
        //data inicial de filtragem
        public DateOnly? dtInicial { get; set; }
        //data final de filtragem
        public DateOnly? dtFinal { get; set; }
        //termo de busca por edicao especifica
        public string? edicao { get; set; }
        //ultimo id de arquivo pesquisado - para paginação
        public int? lastId { get; set; }
        //limite de documentos a serem buscados
        public int limit { get; set; } = 10;
        public string cidade {get; set;} = String.Empty;
    }
}
