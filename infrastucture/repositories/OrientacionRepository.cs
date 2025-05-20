using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using campuslove.domain.entities;
using campuslove.domain.ports;
using MySql.Data.MySqlClient;

namespace CampusLove.Infrastructure.repositories
{
    public class OrientacionRepository : IGenericRepository<Orientacion>, IOrientacionRepository
    {
        private readonly MySqlConnection _connection;

        public OrientacionRepository(MySqlConnection connection)
        {
            _connection = connection;
        }

        public async Task<IEnumerable<Orientacion>> GetAllAsync()
        {
            var orientacionList = new List<Orientacion>();
            const string query = "SELECT id, descripcion FROM Orientacion";

            using var command = new MySqlCommand(query, _connection);
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                orientacionList.Add(new Orientacion
                {
                    Id = Convert.ToInt32(reader["id"]),
                    Descripcion = reader["descripcion"].ToString() ?? string.Empty
                });
            }

            return orientacionList;
        }

        public async Task<Orientacion> GetByIdAsync(object id)
        {
            const string query = "SELECT id, descripcion FROM Orientacion WHERE id = @Id";

            using var command = new MySqlCommand(query, _connection);
            command.Parameters.AddWithValue("@Id", id);

            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return new Orientacion
                {
                    Id = Convert.ToInt32(reader["id"]),
                    Descripcion = reader["descripcion"].ToString() ?? string.Empty
                };
            }

            return null;
        }

        public async Task<int> InsertAsync(Orientacion orientacion)
        {
            if (orientacion == null)
                throw new ArgumentNullException(nameof(orientacion));

            const string query = "INSERT INTO Orientacion (descripcion) VALUES (@descripcion)";
            using var transaction = await _connection.BeginTransactionAsync();

            try
            {
                using var command = new MySqlCommand(query, _connection, transaction);
                command.Parameters.AddWithValue("@descripcion", orientacion.Descripcion);

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

        public async Task<bool> UpdateAsync(Orientacion orientacion)
        {
            if (orientacion == null)
                throw new ArgumentNullException(nameof(orientacion));

            const string query = "UPDATE Orientacion SET descripcion = @descripcion WHERE id = @Id";
            using var transaction = await _connection.BeginTransactionAsync();

            try
            {
                using var command = new MySqlCommand(query, _connection, transaction);
                command.Parameters.AddWithValue("@descripcion", orientacion.Descripcion);
                command.Parameters.AddWithValue("@Id", orientacion.Id);

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
            const string query = "DELETE FROM Orientacion WHERE id = @Id";
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
    }
}
