using Diarios.Api.Domain.Models.Requests;

namespace Diarios.Api.Application.Util.QueryBuilder
{
    public static class QueryBuilder
    {
        /// <summary>
        /// Obtém a lista de IDs dos documentos que correspondem aos filtros da busca
        /// </summary>
        /// <param name="searchModel"></param>
        /// <returns></returns>
        public static string GetDocsIds(SearchRequest searchModel)
        {
            int lastId = searchModel.LastDocId ?? 0;
            int fetchLimit = searchModel.Limit + 1;

            // possibilidade de SQL Injection !!!!!!!!!
            string termosDeBusca = searchModel.Terms == null ? "1" : $"f.conteudo MATCH '{searchModel.Terms}'";
            string edicaoDeBusca = searchModel.Edicao == null ? "1" : $"d.nm_edicao LIKE '%{searchModel.Edicao}%'";

            string query = $"""
                SELECT DISTINCT f.doc_id
                FROM docs_fts f
                INNER JOIN ({AddDateFiltering(searchModel)}) d 
                ON f.doc_id = d.id
                WHERE {termosDeBusca}
                AND {edicaoDeBusca}
                AND f.doc_id > {lastId}
                ORDER BY f.doc_id ASC
                LIMIT {fetchLimit}
                """;

            return query;
        }

        public static string GetPaginasByDocIds(SearchRequest searchModel, IEnumerable<int> docIds)
        {
            string idList = string.Join(",", docIds);
            string conteudoDeBusca = searchModel.Terms == null ? "f.pagina = 1" : $"f.conteudo MATCH '{searchModel.Terms}'";

            return $"""
                    SELECT f.pagina, f.conteudo, d.id, d.nm_edicao, d.caminho, d.ano, d.mes, d.dia
                    FROM docs_fts f
                    INNER JOIN docs d ON f.doc_id = d.id
                    WHERE f.doc_id IN ({idList})
                    AND {conteudoDeBusca}
                    ORDER BY d.id ASC, f.pagina ASC
                    """;
        }

        private static string AddDateFiltering(SearchRequest queryModel)
        {
            string filterQuery = String.Empty;
            DateOnly dataInicial = queryModel.DtInicial ?? new DateOnly();
            DateOnly dataFinal = queryModel.DtFinal ?? dataInicial;
            // usuario selecionou um range de datas
            if (queryModel.DtInicial != null && queryModel.DtFinal != null)
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
                if (queryModel.DtInicial != null)
                {
                    filterQuery = $"""
                                    SELECT d1.id, d1.nm_edicao, d1.caminho, d1.ano, d1.mes, d1.dia 
                                    FROM docs d1
                                    WHERE ano = {dataInicial.Year}
                                    AND mes = {dataInicial.Month}
                                    AND dia = {dataInicial.Day}
                                    """;
                }
                else
                {
                    //nenhuma data foi especificada
                    filterQuery = $"""
                                    SELECT d1.id, d1.nm_edicao, d1.caminho, d1.ano, d1.mes, d1.dia 
                                    FROM docs d1
                                    """;
                    filterQuery = $""""docs""""; // temporario para testes
                }
            }
            return filterQuery;
        }
    }
}
