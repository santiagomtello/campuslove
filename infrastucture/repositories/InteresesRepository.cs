using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using campuslove.domain.entities;
using campuslove.domain.ports;
using MySql.Data.MySqlClient;

namespace CampusLove.Infrastructure.repositories
{
    public class InteresRepository : IGenericRepository<Interes>, IInteresesRepository
    {
        private readonly MySqlConnection _connection;

        public InteresRepository(MySqlConnection connection)
        {
            _connection = connection;
        }

        public async Task<IEnumerable<Interes>> GetAllAsync()
        {
            var interesList = new List<Interes>();
            const string query = "SELECT id, descripcion FROM Intereses";

            using var command = new MySqlCommand(query, _connection);
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                interesList.Add(new Interes
                {
                    Id = Convert.ToInt32(reader["id"]),
                    Descripcion = reader["descripcion"].ToString() ?? string.Empty
                });
            }

            return interesList;
        }

        public async Task<Interes> GetByIdAsync(object id)
        {
            const string query = "SELECT id, descripcion FROM Intereses WHERE id = @Id";

            using var command = new MySqlCommand(query, _connection);
            command.Parameters.AddWithValue("@Id", id);

            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return new Interes
                {
                    Id = Convert.ToInt32(reader["id"]),
                    Descripcion = reader["descripcion"].ToString() ?? string.Empty
                };
            }

            return null;
        }

        public async Task<int> InsertAsync(Interes interes)
        {
            if (interes == null)
                throw new ArgumentNullException(nameof(interes));

            const string query = "INSERT INTO Intereses (descripcion) VALUES (@descripcion)";
            using var transaction = await _connection.BeginTransactionAsync();

            try
            {
                using var command = new MySqlCommand(query, _connection, transaction);
                command.Parameters.AddWithValue("@descripcion", interes.Descripcion);

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

        public async Task<bool> UpdateAsync(Interes interes)
        {
            if (interes == null)
                throw new ArgumentNullException(nameof(interes));

            const string query = "UPDATE Intereses SET descripcion = @descripcion WHERE id = @Id";
            using var transaction = await _connection.BeginTransactionAsync();

            try
            {
                using var command = new MySqlCommand(query, _connection, transaction);
                command.Parameters.AddWithValue("@descripcion", interes.Descripcion);
                command.Parameters.AddWithValue("@Id", interes.Id);

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
            const string query = "DELETE FROM Intereses WHERE id = @Id";
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
