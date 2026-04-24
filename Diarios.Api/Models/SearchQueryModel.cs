using System.ComponentModel;

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
        //filtrar edicoes extras (true por default)
        //removido - ainda nao definitivo
        //public bool? edextras { get; set; }

        //ultimo id de arquivo pesquisado - para paginação
        public int? lastId { get; set; }
    }
}
