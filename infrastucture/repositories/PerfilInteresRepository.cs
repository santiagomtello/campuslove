using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using campuslove.domain.entities;
using campuslove.domain.ports;
using MySql.Data.MySqlClient;

namespace CampusLove.Infrastructure.repositories
{
    public class PerfilInteresRepository : IGenericRepository<PerfilIntereses>, IPerfilInteresRepository
    {
        private readonly MySqlConnection _connection;

        public PerfilInteresRepository(MySqlConnection connection)
        {
            _connection = connection;
        }

        public async Task<IEnumerable<PerfilIntereses>> GetAllAsync()
        {
            var list = new List<PerfilIntereses>();
            const string query = "SELECT id, perfilid, interesesid FROM perfilInterese";

            using var command = new MySqlCommand(query, _connection);
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                list.Add(new PerfilIntereses
                {
                    Id = Convert.ToInt32(reader["id"]),
                    PerfilId = Convert.ToInt32(reader["perfilid"]),
                    InteresesId = Convert.ToInt32(reader["interesid"])
                });
            }

            return list;
        }

        public async Task<PerfilIntereses> GetByIdAsync(object id)
        {
            const string query = "SELECT id, perfilid, interesesid FROM perfilInterese WHERE id = @Id";

            using var command = new MySqlCommand(query, _connection);
            command.Parameters.AddWithValue("@Id", id);

            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return new PerfilIntereses
                {
                    Id = Convert.ToInt32(reader["id"]),
                    PerfilId = Convert.ToInt32(reader["perfilid"]),
                    InteresesId = Convert.ToInt32(reader["interesid"])
                };
            }

            return null;
        }

        public async Task<int> InsertAsync(PerfilIntereses PerfilIntereses)
        {
            if (PerfilIntereses == null)
                throw new ArgumentNullException(nameof(PerfilIntereses));

            const string query = "INSERT INTO perfilInterese (perfilid, interesesid) VALUES (@PerfilId, @InteresesId)";
            using var transaction = await _connection.BeginTransactionAsync();

            try
            {
                using var command = new MySqlCommand(query, _connection, transaction);
                command.Parameters.AddWithValue("@PerfilId", PerfilIntereses.PerfilId);
                command.Parameters.AddWithValue("@InteresesId", PerfilIntereses.InteresesId);

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

        public async Task<bool> UpdateAsync(PerfilIntereses PerfilIntereses)
        {
            if (PerfilIntereses == null)
                throw new ArgumentNullException(nameof(PerfilIntereses));

            const string query = "UPDATE perfilInterese SET perfilid = @PerfilId, interesesid = @InteresesId WHERE id = @Id";
            using var transaction = await _connection.BeginTransactionAsync();

            try
            {
                using var command = new MySqlCommand(query, _connection, transaction);
                command.Parameters.AddWithValue("@PerfilId", PerfilIntereses.PerfilId);
                command.Parameters.AddWithValue("@InteresesId", PerfilIntereses.InteresesId);
                command.Parameters.AddWithValue("@Id", PerfilIntereses.Id);

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
            const string query = "DELETE FROM perfilInterese WHERE id = @Id";
            using var transaction = await _connection.BeginTransactionAsync();

            try
            {
                using var command = new MySqlCommand(query, _connection, transaction);
                command.Parameters.AddWithValue("@Id", id);

                var result = await command.ExecuteNonQueryAsync()>0;
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