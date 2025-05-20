using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using campuslove.domain.entities;
using campuslove.domain.ports;
using MySql.Data.MySqlClient;

namespace CampusLove.Infrastructure.repositories
{
    public class UsuarioRepository : IGenericRepository<Usuario>, IUsuarioRepository
    {
        private readonly MySqlConnection _connection;

        public UsuarioRepository(MySqlConnection connection)
        {
            _connection = connection;
        }

        public async Task<IEnumerable<Usuario>> GetAllAsync()
        {
            var list = new List<Usuario>();
            const string query = "SELECT id, perfilid, username, password FROM Usuario";

            using var command = new MySqlCommand(query, _connection);
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                list.Add(new Usuario
                {
                    Id = Convert.ToInt32(reader["id"]),
                    PerfilId = Convert.ToInt32(reader["perfilid"]),
                    Username = reader["username"].ToString() ?? string.Empty,
                    Password = reader["password"].ToString() ?? string.Empty
                });
            }

            return list;
        }

        public async Task<Usuario> GetByIdAsync(object id)
        {
            const string query = "SELECT id, perfilid, username, password FROM Usuario WHERE id = @Id";

            using var command = new MySqlCommand(query, _connection);
            command.Parameters.AddWithValue("@Id", id);

            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return new Usuario
                {
                    Id = Convert.ToInt32(reader["id"]),
                    PerfilId = Convert.ToInt32(reader["perfilid"]),
                    Username = reader["username"].ToString() ?? string.Empty,
                    Password = reader["password"].ToString() ?? string.Empty
                };
            }

            return null;
        }

        public async Task<int> InsertAsync(Usuario usuario)
        {
            if (usuario == null)
                throw new ArgumentNullException(nameof(usuario));

            const string query = "INSERT INTO Usuario (perfilid, username, password) VALUES (@PerfilId, @username, @Password)";
            using var transaction = await _connection.BeginTransactionAsync();

            try
            {
                using var command = new MySqlCommand(query, _connection, transaction);
                command.Parameters.AddWithValue("@PerfilId", usuario.PerfilId);
                command.Parameters.AddWithValue("@username", usuario.Username);
                command.Parameters.AddWithValue("@Password", usuario.Password);

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

        public async Task<bool> UpdateAsync(Usuario usuario)
        {
            if (usuario == null)
                throw new ArgumentNullException(nameof(usuario));

            const string query = "UPDATE Usuario SET perfilid = @PerfilId, username = @username, password = @Password WHERE id = @Id";
            using var transaction = await _connection.BeginTransactionAsync();

            try
            {
                using var command = new MySqlCommand(query, _connection, transaction);
                command.Parameters.AddWithValue("@PerfilId", usuario.PerfilId);
                command.Parameters.AddWithValue("@username", usuario.Username);
                command.Parameters.AddWithValue("@Password", usuario.Password);
                command.Parameters.AddWithValue("@Id", usuario.Id);

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
            const string query = "DELETE FROM Usuario WHERE id = @Id";
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

        public async Task<Usuario> ObtenerPorusername(string username)
        {
            Usuario usuario = null;
            const string query = "SELECT id, perfilid, username, password FROM Usuario WHERE username = @username";

            using var command = new MySqlCommand(query, _connection);
            command.Parameters.AddWithValue("@username", username);

            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                usuario = new Usuario
                {
                    Id = Convert.ToInt32(reader["id"]),
                    PerfilId = Convert.ToInt32(reader["perfilid"]),
                    Username = reader["username"].ToString() ?? string.Empty,
                    Password = reader["password"].ToString() ?? string.Empty
                };
            }

            return usuario;
        }
    }
}
