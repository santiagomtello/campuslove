using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using campuslove.domain.entities;
using campuslove.domain.ports;
using MySql.Data.MySqlClient;

namespace CampusLove.Infrastructure.repositories
{
    public class InteraccionRepository : IGenericRepository<Interaccion>
    {
        private readonly MySqlConnection _connection;

        public InteraccionRepository(MySqlConnection connection)
        {
            _connection = connection;
        }

        public async Task<IEnumerable<Interaccion>> GetAllAsync()
        {
            var list = new List<Interaccion>();
            const string query = "SELECT id, usuarioEnviaId, usuarioRecibeId, tipoInteraccionId, fecha FROM interaccion";

            using var command = new MySqlCommand(query, _connection);
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                list.Add(new Interaccion
                {
                    Id = Convert.ToInt32(reader["id"]),
                    UsuarioEnviaId = Convert.ToInt32(reader["usuarioEnviaId"]),
                    UsuarioRecibeId = Convert.ToInt32(reader["usuarioRecibeId"]),
                    TipoInteraccionId = Convert.ToInt32(reader["tipoInteraccionId"]),
                    Fecha = Convert.ToDateTime(reader["fecha"])
                });
            }

            return list;
        }

        public async Task<Interaccion> GetByIdAsync(object id)
        {
            const string query = "SELECT id, usuarioEnviaId, usuarioRecibeId, tipoInteraccionId, fecha FROM interaccion WHERE id = @Id";

            using var command = new MySqlCommand(query, _connection);
            command.Parameters.AddWithValue("@Id", id);

            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return new Interaccion
                {
                    Id = Convert.ToInt32(reader["id"]),
                    UsuarioEnviaId = Convert.ToInt32(reader["usuarioEnviaId"]),
                    UsuarioRecibeId = Convert.ToInt32(reader["usuarioRecibeId"]),
                    TipoInteraccionId = Convert.ToInt32(reader["tipoInteraccionId"]),
                    Fecha = Convert.ToDateTime(reader["fecha"])
                };
            }

            return null;
        }

        public async Task<int> InsertAsync(Interaccion interaccion)
        {
            if (interaccion == null)
                throw new ArgumentNullException(nameof(interaccion));

            const string query = "INSERT INTO interaccion (usuarioEnviaId, usuarioRecibeId, tipoInteraccionId, fecha) VALUES (@UsuarioEnviaId, @UsuarioRecibeId, @TipoIntId, @Fecha)";
            using var transaction = await _connection.BeginTransactionAsync();

            try
            {
                using var command = new MySqlCommand(query, _connection, transaction);
                command.Parameters.AddWithValue("@UsuarioEnviaId", interaccion.UsuarioEnviaId);
                command.Parameters.AddWithValue("@UsuarioRecibeId", interaccion.UsuarioRecibeId);
                command.Parameters.AddWithValue("@TipoIntId", interaccion.TipoInteraccionId);
                command.Parameters.AddWithValue("@Fecha", interaccion.Fecha);

                await command.ExecuteNonQueryAsync();
                int insertedId = (int)command.LastInsertedId;
                await transaction.CommitAsync();
                return insertedId;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<bool> UpdateAsync(Interaccion interaccion)
        {
            if (interaccion == null)
                throw new ArgumentNullException(nameof(interaccion));

            const string query = "UPDATE interaccion SET usuarioEnviaId = @UsuarioEnviaId, usuarioRecibeId = @UsuarioRecibeId, tipoInteraccionId = @TipoIntId, fecha = @Fecha WHERE id = @Id";
            using var transaction = await _connection.BeginTransactionAsync();

            try
            {
                using var command = new MySqlCommand(query, _connection, transaction);
                command.Parameters.AddWithValue("@UsuarioEnviaId", interaccion.UsuarioEnviaId);
                command.Parameters.AddWithValue("@UsuarioRecibeId", interaccion.UsuarioRecibeId);
                command.Parameters.AddWithValue("@TipoIntId", interaccion.TipoInteraccionId);
                command.Parameters.AddWithValue("@Fecha", interaccion.Fecha);
                command.Parameters.AddWithValue("@Id", interaccion.Id);

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
            const string query = "DELETE FROM interaccion WHERE id = @Id";
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

        public async Task<bool> VerificarLike(int usuarioEnviaId, int usuarioRecibeId)
        {
            const string query = "SELECT COUNT(*) FROM interaccion WHERE usuarioEnviaId = @UsuarioEnviaId AND usuarioRecibeId = @UsuarioRecibeId AND tipoInteraccionId = 1";
            
            using var command = new MySqlCommand(query, _connection);
            command.Parameters.AddWithValue("@UsuarioEnviaId", usuarioEnviaId);
            command.Parameters.AddWithValue("@UsuarioRecibeId", usuarioRecibeId);

            var result = await command.ExecuteScalarAsync();
            return Convert.ToInt32(result) > 0;
        }

        public async Task<int> ObtenerConteoLikes(int usuarioId)
        {
            const string query = @"
                SELECT COUNT(*) 
                FROM interaccion 
                WHERE usuarioEnviaId = @UsuarioId 
                AND tipoInteraccionId = 1 
                AND DATE(fecha) = CURDATE()";

            using var command = new MySqlCommand(query, _connection);
            command.Parameters.AddWithValue("@UsuarioId", usuarioId);

            var result = await command.ExecuteScalarAsync();
            return Convert.ToInt32(result);
        }

        public async Task<bool> VerificarDislike(int usuarioEnviaId, int usuarioRecibeId)
        {
            const string query = "SELECT COUNT(*) FROM interaccion WHERE usuarioEnviaId = @UsuarioEnviaId AND usuarioRecibeId = @UsuarioRecibeId AND tipoInteraccionId = 2";
            
            using var command = new MySqlCommand(query, _connection);
            command.Parameters.AddWithValue("@UsuarioEnviaId", usuarioEnviaId);
            command.Parameters.AddWithValue("@UsuarioRecibeId", usuarioRecibeId);

            var result = await command.ExecuteScalarAsync();
            return Convert.ToInt32(result) > 0;
        }
    }
} 