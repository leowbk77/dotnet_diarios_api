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
            string connectionString = _connectionProvider.GetConnectionString(cidade);
            SqliteConnection connection = new SqliteConnection(connectionString);
            DiarioModel diario = new DiarioModel();

            if (!DataBaseExists(connectionString))
            {
                throw new DatabaseNotFoundException(connectionString);
            }

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

            var reader = command.ExecuteReader();

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

        private bool DataBaseExists(string connectionString) 
        {
            return File.Exists(connectionString);
        }

    }
}
