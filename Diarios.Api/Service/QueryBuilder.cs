using Diarios.Api.Models;

namespace Diarios.Api.Service
{
    public static class QueryBuilder
    {
        public static string SearchForTermsOnEdicao(SearchQueryModel searchModel)
        {
            string documentsByEdicaoQuery = $"""
                                            SELECT *
                                            FROM docs
                                            WHERE docs.nm_edicao LIKE '%{searchModel.edicao}%'
                                            """;

            string query = $"""
                            SELECT f.pagina, f.conteudo, d.id, d.nm_edicao, d.caminho, d.ano, d.mes, d.dia
                            FROM docs_fts f
                            INNER JOIN ({documentsByEdicaoQuery}) d
                            ON f.doc_id = d.id
                            WHERE f.conteudo MATCH '{searchModel.terms}'
                            """;
            return query;
        }

        public static string SearchForTermsOnAllDocs(SearchQueryModel searchModel)
        {
            string newQueryDtFilter = "";

            string newQuery = $"""
                                SELECT f.pagina, f.conteudo, d.id, d.nm_edicao, d.caminho, d.ano, d.mes, d.dia
                                FROM (
                                    SELECT ft.pagina, ft.conteudo, ft.doc_id 
                                    FROM docs_fts ft 
                                    WHERE ft.conteudo MATCH '{searchModel.terms}'
                                ) f
                                INNER JOIN ({newQueryDtFilter.AddDate(searchModel)}) d
                                ON f.doc_id = d.id
                                """;
            return newQuery;
        }

        private static string AddDate(this String query, SearchQueryModel queryModel)
        {
            string filterQuery = String.Empty;
            DateOnly dataInicial = queryModel.dtInicial ?? new DateOnly();
            DateOnly dataFinal = queryModel.dtFinal ?? dataInicial;
            // usuario selecionou um range de datas
            if (queryModel.dtInicial != null && queryModel.dtFinal != null)
            {
                switch (dataFinal.Year - dataInicial.Year)
                {
                    case 1:
                        filterQuery = $"""
                                        SELECT * FROM (
                                                        SELECT d1.id, d1.nm_edicao, d1.caminho, d1.ano, d1.mes, d1.dia 
                                                        FROM docs d1 
                                                        WHERE d1.ano = {dataInicial.Year}
                                                        AND d1.mes > {dataInicial.Month}
                                                        UNION
                                                        SELECT d2.id, d2.nm_edicao, d2.caminho, d2.ano, d2.mes, d2.dia
                                                        FROM docs d2 
                                                        WHERE d2.ano = {dataFinal.Year}
                                                        AND d2.mes < {dataFinal.Month}
                                                        )
                                        """;
                        break;
                    case > 1:
                        filterQuery = $"""
                                        SELECT * FROM (
                                        SELECT d1.id, d1.nm_edicao, d1.caminho, d1.ano, d1.mes, d1.dia FROM docs d1 WHERE d1.ano = {dataInicial.Year} AND d1.mes > {dataInicial.Month}
                                        UNION
                                        SELECT d3.id, d3.nm_edicao, d3.caminho, d3.ano, d3.mes, d3.dia FROM docs d3 WHERE d3.ano BETWEEN {dataInicial.Year + 1} AND {dataFinal.Year - 1}
                                        UNION
                                        SELECT d2.id, d2.nm_edicao, d2.caminho, d2.ano, d2.mes, d2.dia FROM docs d2 WHERE d2.ano = {dataFinal.Year} AND d2.mes < {dataFinal.Month})
                                        """;
                        break;
                }
            }
            else
            {
                //usuario definiu uma data especifica
                if (queryModel.dtInicial != null)
                { 
                    filterQuery = $"""
                                    SELECT d1.id, d1.nm_edicao, d1.caminho, d1.ano, d1.mes, d1.dia 
                                    FROM docs d1
                                    WHERE ano = {dataInicial.Year}
                                    AND mes = {dataInicial.Month}
                                    AND dia = {dataInicial.Day}
                                    """;
                } else
                {
                    //nenhuma data foi especificada
                    filterQuery = $"""
                                    SELECT d1.id, d1.nm_edicao, d1.caminho, d1.ano, d1.mes, d1.dia 
                                    FROM docs d1
                                    """;
                }
            }
            return filterQuery;
        }
    }
}
