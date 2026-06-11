using Diarios.Api.Models;
using Diarios.Api.Repository.Interface;
using Diarios.Api.Service;
using Diarios.Api.Util.CustomException;
using Diarios.Api.Util.Provider.Interface;
using Microsoft.Data.Sqlite;

namespace Diarios.Api.Repository
{
    public class DiarioRepository : IDiarioRepository
    {
        private readonly ICidadeProvider _connectionProvider;

        public DiarioRepository(ICidadeProvider connectionProvider)
        {
            _connectionProvider = connectionProvider;
        }

        public DiarioModel GetDiarioById(int id, string cidade)
        {
            string connectionString = GetConnectionStringValidada(cidade);
            SqliteConnection connection = new SqliteConnection(connectionString);
            DiarioModel diario = new DiarioModel();

            connection.Open();
            var command = connection.CreateCommand();
            command.CommandText =
                                """
                                SELECT * 
                                FROM docs
                                WHERE id = $id
                                """;

            command.Parameters.AddWithValue("$id", id);
            var reader = command.ExecuteReader();

            while (reader.Read())
            {
                diario.id = reader.IsDBNull(reader.GetOrdinal("id")) ? 0 : reader.GetInt32(reader.GetOrdinal("id"));
                diario.nmEdicao = reader.IsDBNull(reader.GetOrdinal("nm_edicao")) ? "" : reader.GetString(reader.GetOrdinal("nm_edicao"));
                diario.caminho = reader.IsDBNull(reader.GetOrdinal("caminho")) ? "" : reader.GetString(reader.GetOrdinal("caminho"));
                diario.ano = reader.IsDBNull(reader.GetOrdinal("ano")) ? 0 : reader.GetInt32(reader.GetOrdinal("ano"));
                diario.mes = reader.IsDBNull(reader.GetOrdinal("mes")) ? 0 : reader.GetInt32(reader.GetOrdinal("mes"));
                diario.dia = reader.IsDBNull(reader.GetOrdinal("dia")) ? 0 : reader.GetInt32(reader.GetOrdinal("dia"));
            }
            connection.Close();

            return diario;
        }

        // deprecated
        public List<DiarioModel> SearchDiarios(string query, string cidade)
        {
            List<DiarioModel> diarios = new List<DiarioModel>();
            string connectionString = _connectionProvider.GetConnectionString(cidade);

            if (!DataBaseExists($"{cidade}.db"))
            {
                throw new DatabaseNotFoundException(connectionString);
            }

            SqliteConnection connection = new SqliteConnection(connectionString);

            connection.Open();
            var command = connection.CreateCommand();
            command.CommandText = query;

            using var reader = command.ExecuteReader();

            while (reader.Read())
            {
                diarios.Add(new DiarioModel {
                    id = reader.IsDBNull(reader.GetOrdinal("id")) ? 0 : reader.GetInt32(reader.GetOrdinal("id")),
                    nmEdicao = reader.IsDBNull(reader.GetOrdinal("nm_edicao")) ? "" : reader.GetString(reader.GetOrdinal("nm_edicao")),
                    caminho = reader.IsDBNull(reader.GetOrdinal("caminho")) ? "" : reader.GetString(reader.GetOrdinal("caminho")),
                    ano = reader.IsDBNull(reader.GetOrdinal("ano")) ? 0 : reader.GetInt32(reader.GetOrdinal("ano")),
                    mes = reader.IsDBNull(reader.GetOrdinal("mes")) ? 0 : reader.GetInt32(reader.GetOrdinal("mes")),
                    dia = reader.IsDBNull(reader.GetOrdinal("dia")) ? 0 : reader.GetInt32(reader.GetOrdinal("dia")),
                });
            }

            return diarios;
        }

        public List<int> SearchForDiariosIds(string query, string cidade)
        {
            string connectionString = GetConnectionStringValidada(cidade);
            List<int> diariosIds = new List<int>();

            SqliteConnection connection = new SqliteConnection(connectionString);
            connection.Open();
            var command = connection.CreateCommand();
            command.CommandText = query;

            using var reader = command.ExecuteReader();

            while(reader.Read())
            {
                diariosIds.Add(reader.GetInt32(reader.GetOrdinal("doc_id")));
            }

            return diariosIds;
        }

        public List<SearchDiariosResultModel> SearchDiariosByIdList(string query, string cidade)
        {
            string connectionString = GetConnectionStringValidada(cidade);

            SqliteConnection connection = new SqliteConnection(connectionString);
            connection.Open();
            var command = connection.CreateCommand();
            command.CommandText = query;

            using var reader = command.ExecuteReader();

            var diariosResult = new Dictionary<int, SearchDiariosResultModel>();
            int paginasObtidasNoIdAtual = 0;
            int idAtual = 0;
            while (reader.Read())
            {
                int id = reader.GetInt32(reader.GetOrdinal("id"));

                if(idAtual != id)
                {
                    idAtual = id;
                    paginasObtidasNoIdAtual = 0;
                }

                if (!diariosResult.ContainsKey(id))
                {
                    diariosResult[id] = MapSearchResult(reader);
                }

                // Se tiver mais de 3 paginas com conteudo
                // omitir o conteudo para salvar espaço na mensagem
                // informar apenas o numero da pagina
                // EX: resultados encontrados
                // Pg1, Pg2, Pg3 e mais [x] resultados...
                // pode ser mudado para quantidade X vindo da requisicao
                if(paginasObtidasNoIdAtual < 3)
                {
                    diariosResult[id].Paginas.Add(MapPagina(reader));
                }
                else
                {
                    diariosResult[id].Paginas.Add(MapPaginaSemConteudo(reader));
                }
                paginasObtidasNoIdAtual++;
            }

            return diariosResult.Values.ToList();
        }

        private static PaginaModel MapPaginaSemConteudo(SqliteDataReader reader) => new()
        {
            Numero = reader.IsDBNull(reader.GetOrdinal("pagina")) ? 0 : reader.GetInt32(reader.GetOrdinal("pagina")),
            Conteudo = String.Empty
        };

        private static PaginaModel MapPagina(SqliteDataReader reader) => new()
        {
            Numero = reader.IsDBNull(reader.GetOrdinal("pagina")) ? 0 : reader.GetInt32(reader.GetOrdinal("pagina")),
            Conteudo = reader.IsDBNull(reader.GetOrdinal("conteudo")) ? String.Empty : reader.GetString(reader.GetOrdinal("conteudo"))
        };

        private static SearchDiariosResultModel MapSearchResult(SqliteDataReader reader) => new()
        {
            Id = reader.IsDBNull(reader.GetOrdinal("id")) ? 0 : reader.GetInt32(reader.GetOrdinal("id")),
            NmEdicao = reader.IsDBNull(reader.GetOrdinal("nm_edicao")) ? "" : reader.GetString(reader.GetOrdinal("nm_edicao")),
            Caminho = reader.IsDBNull(reader.GetOrdinal("caminho")) ? "" : reader.GetString(reader.GetOrdinal("caminho")),
            Ano = reader.IsDBNull(reader.GetOrdinal("ano")) ? 0 : reader.GetInt32(reader.GetOrdinal("ano")),
            Mes = reader.IsDBNull(reader.GetOrdinal("mes")) ? 0 : reader.GetInt32(reader.GetOrdinal("mes")),
            Dia = reader.IsDBNull(reader.GetOrdinal("dia")) ? 0 : reader.GetInt32(reader.GetOrdinal("dia")),
            Paginas = new List<PaginaModel>()
        };

        private string GetConnectionStringValidada(string cidade)
        {
            string connectionString = _connectionProvider.GetConnectionString(cidade);

            if (!File.Exists($"{cidade}.db"))
                throw new DatabaseNotFoundException(connectionString);

            return connectionString;
        }

        private bool DataBaseExists(string connectionString) 
        {
            return File.Exists(connectionString);
        }

    }
}
