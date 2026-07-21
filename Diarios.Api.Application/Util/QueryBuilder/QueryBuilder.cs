using Diarios.Api.Domain.Models;
using Diarios.Api.Domain.Models.Requests;
using System.Diagnostics.CodeAnalysis;

namespace Diarios.Api.Application.Util.QueryBuilder
{
    [ExcludeFromCodeCoverage]
    public static class QueryBuilder
    {
        /// <summary>
        /// Obtém a lista de IDs dos documentos que correspondem aos filtros da busca
        /// </summary>
        /// <param name="searchModel"></param>
        /// <returns></returns>
        public static QueryDefinition GetDocsIds(SearchRequest request)
        {
            QueryDefinition query = new QueryDefinition();

            int lastId = request.LastDocId ?? 0;
            int fetchLimit = request.Limit + 1;

            string termosDeBusca = request.Terms == null ? "1" : $"f.conteudo MATCH @termos";
            if (!termosDeBusca.Equals("1")) query.Parameters.Add("@termos", request.Terms ?? String.Empty);

            string edicaoDeBusca = request.Edicao == null ? "1" : $"d.nm_edicao LIKE @nm_edicao";
            if (!edicaoDeBusca.Equals("1")) query.Parameters.Add("@nm_edicao", $"%{request.Edicao}%");

            query.Sql = $"""
                SELECT DISTINCT f.doc_id
                FROM docs_fts f
                INNER JOIN ({AddDateFiltering(request)}) d 
                ON f.doc_id = d.id
                WHERE {termosDeBusca}
                AND {edicaoDeBusca}
                AND f.doc_id > {lastId}
                ORDER BY f.doc_id ASC
                LIMIT {fetchLimit}
                """;

            return query;
        }

        public static QueryDefinition GetPaginasByDocIds(SearchRequest request, IEnumerable<int> docIds)
        {
            QueryDefinition query = new QueryDefinition();

            string idList = string.Join(",", docIds);
            string conteudoDeBusca = request.Terms == null ? "f.pagina = 1" : $"f.conteudo MATCH @termos";
            if (!conteudoDeBusca.Equals("f.pagina = 1")) query.Parameters.Add("@termos", request.Terms ?? String.Empty);

            query.Sql = $"""
                    SELECT f.pagina, f.conteudo, d.id, d.nm_edicao, d.caminho, d.dt_edicao
                    FROM docs_fts f
                    INNER JOIN docs d ON f.doc_id = d.id
                    WHERE f.doc_id IN ({idList})
                    AND {conteudoDeBusca}
                    ORDER BY d.id ASC, f.pagina ASC
                    """;
            return query;
        }

        private static string AddDateFiltering(SearchRequest request)
        {
            string filterQuery = String.Empty;
            DateOnly dataInicial = request.DtInicial ?? new DateOnly();
            DateOnly dataFinal = request.DtFinal ?? dataInicial;
            // usuario selecionou um range de datas
            if (request.DtInicial != null && request.DtFinal != null)
            {
                filterQuery = $"""
                    SELECT * FROM docs dd WHERE dd.dt_edicao BETWEEN '{dataInicial.FormatDataForSqlite()}' AND '{dataFinal.FormatDataForSqlite()}'
                    """;
            }
            else
            {
                //usuario definiu uma data especifica
                if (request.DtInicial != null)
                {
                    filterQuery = $"""
                        SELECT * FROM docs dd WHERE dd.dt_edicao = '{dataInicial.FormatDataForSqlite()}'
                        """;
                }
                else
                {
                    //nenhuma data foi especificada
                    filterQuery = $"docs";
                }
            }
            return filterQuery;
        }

        private static string FormatDataForSqlite(this DateOnly data)
        {
            int ano = data.Year;
            int mes = data.Month;
            int dia = data.Day;
            if (mes < 10)
            {
                if (dia < 10)
                {
                    return $"{ano}-0{mes}-0{dia}";
                }
                return $"{ano}-0{mes}-{dia}";
            }
            return $"{ano}-{mes}-{dia}";
        }
    }
}
