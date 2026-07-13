using Diarios.Api.Models;
using Diarios.Api.Repository.Interface;
using Diarios.Api.Service;
using Diarios.Api.Util.CustomException;
using Diarios.Api.Util.Provider.Interface;
using Microsoft.Data.Sqlite;
using Serilog;

namespace Diarios.Api.Repository
{
    public class DiarioRepository : IDiarioRepository
    {
        private readonly ICidadeProvider _connectionProvider;
        private readonly IConfiguration _configuration;

        public DiarioRepository(ICidadeProvider connectionProvider, IConfiguration configuration)
        {
            _connectionProvider = connectionProvider;
            _configuration = configuration;
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
                diario.Id = reader.IsDBNull(reader.GetOrdinal("id")) ? 0 : reader.GetInt32(reader.GetOrdinal("id"));
                diario.NmEdicao = reader.IsDBNull(reader.GetOrdinal("nm_edicao")) ? "" : reader.GetString(reader.GetOrdinal("nm_edicao"));
                diario.Caminho = reader.IsDBNull(reader.GetOrdinal("caminho")) ? "" : reader.GetString(reader.GetOrdinal("caminho"));
                diario.Ano = reader.IsDBNull(reader.GetOrdinal("ano")) ? 0 : reader.GetInt32(reader.GetOrdinal("ano"));
                diario.Mes = reader.IsDBNull(reader.GetOrdinal("mes")) ? 0 : reader.GetInt32(reader.GetOrdinal("mes"));
                diario.Dia = reader.IsDBNull(reader.GetOrdinal("dia")) ? 0 : reader.GetInt32(reader.GetOrdinal("dia"));
            }
            connection.Close();

            return diario;
        }

        public async Task<DiarioModel?> SearchForLatestAsync(string cidade)
        {
            string connectionString = GetConnectionStringValidada(cidade);

            using var connection = new SqliteConnection(connectionString);
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText = """
                          SELECT *
                          FROM docs
                          ORDER BY ano DESC, mes DESC, dia DESC
                          LIMIT 1
                          """;

            using var reader = await command.ExecuteReaderAsync();
            if (!reader.Read())
                return null;

            return MapDiario(reader);
        }

        public async Task<List<int>> SearchForDiariosIdsAsync(string query, string cidade)
        {
            string connectionString = GetConnectionStringValidada(cidade);
            List<int> diariosIds = new List<int>();

            SqliteConnection connection = new SqliteConnection(connectionString);
            connection.Open();
            var command = connection.CreateCommand();
            command.CommandText = query;

            using var reader = await command.ExecuteReaderAsync();

            while(reader.Read())
            {
                diariosIds.Add(reader.GetInt32(reader.GetOrdinal("doc_id")));
            }

            return diariosIds;
        }

        public async Task<List<SearchDiariosResultModel>> SearchDiariosByIdListAsync(string query, string cidade)
        {
            string connectionString = GetConnectionStringValidada(cidade);

            SqliteConnection connection = new SqliteConnection(connectionString);
            connection.Open();
            var command = connection.CreateCommand();
            command.CommandText = query;

            using var reader = await command.ExecuteReaderAsync();

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

        private static DiarioModel MapDiario(SqliteDataReader reader) => new()
        {
            Id = reader.IsDBNull(reader.GetOrdinal("id")) ? 0 : reader.GetInt32(reader.GetOrdinal("id")),
            NmEdicao = reader.IsDBNull(reader.GetOrdinal("nm_edicao")) ? "" : reader.GetString(reader.GetOrdinal("nm_edicao")),
            Caminho = reader.IsDBNull(reader.GetOrdinal("caminho")) ? "" : reader.GetString(reader.GetOrdinal("caminho")),
            Ano = reader.IsDBNull(reader.GetOrdinal("ano")) ? 0 : reader.GetInt32(reader.GetOrdinal("ano")),
            Mes = reader.IsDBNull(reader.GetOrdinal("mes")) ? 0 : reader.GetInt32(reader.GetOrdinal("mes")),
            Dia = reader.IsDBNull(reader.GetOrdinal("dia")) ? 0 : reader.GetInt32(reader.GetOrdinal("dia")),
        };

        private string GetConnectionStringValidada(string cidade)
        {
            string connectionString = _connectionProvider.GetConnectionString(cidade);

            if (!DataBaseExists(cidade))
            {
                Log.Information($"database: {cidade} nao encontrado.");
                throw new DatabaseNotFoundException(connectionString);
            }

            return connectionString;
        }

        private bool DataBaseExists(string cidade)
        {
            string db = $"{cidade}.db";
            return (File.Exists($"./{db}") || File.Exists($"/data/{db}"));
        }

    }
}
