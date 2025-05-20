using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using campuslove.domain.entities;
using campuslove.domain.ports;
using MySql.Data.MySqlClient;

namespace CampusLove.Infrastructure.repositories
{
    public class MatchRepository : IGenericRepository<Match>, IMatchRepository
    {
        private readonly MySqlConnection _connection;

        public MatchRepository(MySqlConnection connection)
        {
            _connection = connection;
        }

        public async Task<IEnumerable<Match>> GetAllAsync()
        {
            const string query = @"
                SELECT m.id, m.usuario1Id, m.usuario2Id, m.fecha,
                       u1.username as usuario1Username,
                       u2.username as usuario2Username
                FROM `match` m
                JOIN usuario u1 ON m.usuario1Id = u1.id
                JOIN usuario u2 ON m.usuario2Id = u2.id
                ORDER BY m.fecha DESC";

            var matches = new List<Match>();

            using var command = new MySqlCommand(query, _connection);
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                matches.Add(new Match
                {
                    Id = Convert.ToInt32(reader["id"]),
                    Usuario1Id = Convert.ToInt32(reader["usuario1Id"]),
                    Usuario2Id = Convert.ToInt32(reader["usuario2Id"]),
                    Fecha = Convert.ToDateTime(reader["fecha"]),
                    Usuario1Username = reader["usuario1Username"].ToString() ?? string.Empty,
                    Usuario2Username = reader["usuario2Username"].ToString() ?? string.Empty
                });
            }

            return matches;
        }

        public async Task<IEnumerable<Match>> GetMatchesByUsuarioIdAsync(int usuarioId)
        {
            const string query = @"
                SELECT m.id, m.usuario1Id, m.usuario2Id, m.fecha,
                       u1.username as usuario1Username,
                       u2.username as usuario2Username
                FROM `match` m
                JOIN usuario u1 ON m.usuario1Id = u1.id
                JOIN usuario u2 ON m.usuario2Id = u2.id
                WHERE m.usuario1Id = @UsuarioId OR m.usuario2Id = @UsuarioId
                ORDER BY m.fecha DESC";

            var matches = new List<Match>();

            using var command = new MySqlCommand(query, _connection);
            command.Parameters.AddWithValue("@UsuarioId", usuarioId);

            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                matches.Add(new Match
                {
                    Id = Convert.ToInt32(reader["id"]),
                    Usuario1Id = Convert.ToInt32(reader["usuario1Id"]),
                    Usuario2Id = Convert.ToInt32(reader["usuario2Id"]),
                    Fecha = Convert.ToDateTime(reader["fecha"]),
                    Usuario1Username = reader["usuario1Username"].ToString() ?? string.Empty,
                    Usuario2Username = reader["usuario2Username"].ToString() ?? string.Empty
                });
            }

            return matches;
        }

        public async Task<Match> GetByIdAsync(object id)
        {
            const string query = @"
                SELECT m.id, m.usuario1Id, m.usuario2Id, m.fecha,
                       u1.username as usuario1Username,
                       u2.username as usuario2Username
                FROM `match` m
                JOIN usuario u1 ON m.usuario1Id = u1.id
                JOIN usuario u2 ON m.usuario2Id = u2.id
                WHERE m.id = @Id";

            using var command = new MySqlCommand(query, _connection);
            command.Parameters.AddWithValue("@Id", id);

            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return new Match
                {
                    Id = Convert.ToInt32(reader["id"]),
                    Usuario1Id = Convert.ToInt32(reader["usuario1Id"]),
                    Usuario2Id = Convert.ToInt32(reader["usuario2Id"]),
                    Fecha = Convert.ToDateTime(reader["fecha"]),
                    Usuario1Username = reader["usuario1Username"].ToString() ?? string.Empty,
                    Usuario2Username = reader["usuario2Username"].ToString() ?? string.Empty
                };
            }

            return null;
        }

        public async Task<int> InsertAsync(Match match)
        {
            if (match == null)
                throw new ArgumentNullException(nameof(match));

            // Verificar si ya existe un match entre estos usuarios
            const string checkQuery = @"
                SELECT COUNT(*) 
                FROM `match` 
                WHERE (usuario1Id = @Usuario1Id AND usuario2Id = @Usuario2Id)
                   OR (usuario1Id = @Usuario2Id AND usuario2Id = @Usuario1Id)";

            using var checkCommand = new MySqlCommand(checkQuery, _connection);
            checkCommand.Parameters.AddWithValue("@Usuario1Id", match.Usuario1Id);
            checkCommand.Parameters.AddWithValue("@Usuario2Id", match.Usuario2Id);

            var existingMatches = Convert.ToInt32(await checkCommand.ExecuteScalarAsync());
            if (existingMatches > 0)
            {
                return 0; // Ya existe un match
            }

            const string insertQuery = @"
                INSERT INTO `match` (usuario1Id, usuario2Id, fecha)
                VALUES (@Usuario1Id, @Usuario2Id, @Fecha)";

            using var transaction = await _connection.BeginTransactionAsync();

            try
            {
                using var command = new MySqlCommand(insertQuery, _connection, (MySqlTransaction)transaction);
                command.Parameters.AddWithValue("@Usuario1Id", match.Usuario1Id);
                command.Parameters.AddWithValue("@Usuario2Id", match.Usuario2Id);
                command.Parameters.AddWithValue("@Fecha", match.Fecha);

                await command.ExecuteNonQueryAsync();
                var matchId = (int)command.LastInsertedId;
                await transaction.CommitAsync();

                return matchId;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw new Exception($"Error al insertar match: {ex.Message}", ex);
            }
        }

        public async Task<bool> UpdateAsync(Match match)
        {
            if (match == null)
                throw new ArgumentNullException(nameof(match));

            const string query = @"
                UPDATE `match` 
                SET usuario1Id = @Usuario1Id,
                    usuario2Id = @Usuario2Id,
                    fecha = @Fecha
                WHERE id = @Id";

            using var transaction = await _connection.BeginTransactionAsync();

            try
            {
                using var command = new MySqlCommand(query, _connection, transaction);
                command.Parameters.AddWithValue("@Id", match.Id);
                command.Parameters.AddWithValue("@Usuario1Id", match.Usuario1Id);
                command.Parameters.AddWithValue("@Usuario2Id", match.Usuario2Id);
                command.Parameters.AddWithValue("@Fecha", match.Fecha);

                var result = await command.ExecuteNonQueryAsync() > 0;
                await transaction.CommitAsync();
                return result;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<bool> DeleteAsync(object id)
        {
            const string query = "DELETE FROM `match` WHERE id = @Id";
            using var transaction = await _connection.BeginTransactionAsync();

            try
            {
                using var command = new MySqlCommand(query, _connection, transaction);
                command.Parameters.AddWithValue("@Id", id);

                var result = await command.ExecuteNonQueryAsync() > 0;
                await transaction.CommitAsync();
                return result;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<bool> ExisteMatchAsync(int usuario1Id, int usuario2Id)
        {
            const string query = @"
                SELECT COUNT(*) 
                FROM `match` 
                WHERE (usuario1Id = @Usuario1Id AND usuario2Id = @Usuario2Id)
                   OR (usuario1Id = @Usuario2Id AND usuario2Id = @Usuario1Id)";

            using var command = new MySqlCommand(query, _connection);
            command.Parameters.AddWithValue("@Usuario1Id", usuario1Id);
            command.Parameters.AddWithValue("@Usuario2Id", usuario2Id);

            var count = Convert.ToInt32(await command.ExecuteScalarAsync());
            return count > 0;
        }
    }
} 