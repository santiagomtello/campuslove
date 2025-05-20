using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using campuslove.domain.entities;
using campuslove.domain.ports;
using MySql.Data.MySqlClient;


namespace CampusLove.Infrastructure.repositories
{
    public class GeneroRepository : IGenericRepository<Genero>, IGeneroRepository
    {
        private readonly MySqlConnection _connection;

        public GeneroRepository(MySqlConnection connection)
        {
            _connection = connection;
        }

        public async Task<IEnumerable<Genero>> GetAllAsync()
        {
            var GeneroList = new List<Genero>();
            const string query = "SELECT id, descripcion FROM Genero";

            using var command = new MySqlCommand(query, _connection);
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                GeneroList.Add(new Genero
                {
                    Id = Convert.ToInt32(reader["id"]),
                    Descripcion = reader["descripcion"].ToString() ?? string.Empty
                });
            }

            return GeneroList;
        }

        public async Task<Genero> GetByIdAsync(object id)
        {
            const string query = "SELECT id, descripcion FROM Genero WHERE id = @Id";

            using var command = new MySqlCommand(query, _connection);
            command.Parameters.AddWithValue("@Id", id);

            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return new Genero
                {
                    Id = Convert.ToInt32(reader["id"]),
                    Descripcion = reader["descripcion"].ToString() ?? string.Empty
                };
            }

            return null;
        }

        public async Task<int> InsertAsync(Genero Genero)
        {
            if (Genero == null)
                throw new ArgumentNullException(nameof(Genero));

            const string query = "INSERT INTO Genero (descripcion) VALUES (@descripcion)";
            using var transaction = await _connection.BeginTransactionAsync();

            try
            {
                using var command = new MySqlCommand(query, _connection, transaction);
                command.Parameters.AddWithValue("@descripcion", Genero.Descripcion);

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

        public async Task<bool> UpdateAsync(Genero Genero)
        {
            if (Genero == null)
                throw new ArgumentNullException(nameof(Genero));

            const string query = "UPDATE Genero SET descripcion = @descripcion WHERE id = @Id";
            using var transaction = await _connection.BeginTransactionAsync();

            try
            {
                using var command = new MySqlCommand(query, _connection, transaction);
                command.Parameters.AddWithValue("@descripcion", Genero.Descripcion);
                command.Parameters.AddWithValue("@Id", Genero.Id);

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
            const string query = "DELETE FROM Genero WHERE id = @Id";
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