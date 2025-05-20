using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using campuslove.domain.entities;
using campuslove.domain.ports;
using MySql.Data.MySqlClient;

namespace CampusLove.Infrastructure.repositories
{
    public class PerfilRepository : IGenericRepository<Perfil>, IPerfilRepository
    {
        private readonly MySqlConnection _connection;

        public PerfilRepository(MySqlConnection connection)
        {
            _connection = connection;
        }

        public async Task<IEnumerable<Perfil>> GetAllAsync()
        {
            const string query = @"
                SELECT 
                    p.id, 
                    p.nombre, 
                    p.edad, 
                    p.generoId, 
                    p.orientacionId, 
                    p.descripcion,
                    g.descripcion as generoDescripcion,
                    o.descripcion as orientacionDescripcion
                FROM perfil p
                LEFT JOIN genero g ON p.generoId = g.id
                LEFT JOIN orientacion o ON p.orientacionId = o.id";

            var perfiles = new List<Perfil>();

            using var command = new MySqlCommand(query, _connection);
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                perfiles.Add(new Perfil
                {
                    Id = Convert.ToInt32(reader["id"]),
                    Nombre = reader["nombre"].ToString() ?? string.Empty,
                    Edad = Convert.ToInt32(reader["edad"]),
                    GeneroId = Convert.ToInt32(reader["generoId"]),
                    OrientacionId = Convert.ToInt32(reader["orientacionId"]),
                    Descripcion = reader["descripcion"].ToString() ?? string.Empty,
                    Genero = new Genero { Descripcion = reader["generoDescripcion"].ToString() ?? string.Empty },
                    Orientacion = new Orientacion { Descripcion = reader["orientacionDescripcion"].ToString() ?? string.Empty }
                });
            }

            return perfiles;
        }

        public async Task<Perfil> GetByIdAsync(object id)
        {
            const string query = @"
                SELECT 
                    p.id, 
                    p.nombre, 
                    p.edad, 
                    p.generoId,
                    p.orientacionId,
                    p.descripcion,
                    g.descripcion as generoDescripcion,
                    o.descripcion as orientacionDescripcion
                FROM perfil p
                LEFT JOIN genero g ON p.generoId = g.id
                LEFT JOIN orientacion o ON p.orientacionId = o.id
                WHERE p.id = @Id";

            using var command = new MySqlCommand(query, _connection);
            command.Parameters.AddWithValue("@Id", id);

            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return new Perfil
                {
                    Id = Convert.ToInt32(reader["id"]),
                    Nombre = reader["nombre"].ToString() ?? string.Empty,
                    Edad = Convert.ToInt32(reader["edad"]),
                    GeneroId = Convert.ToInt32(reader["generoId"]),
                    OrientacionId = Convert.ToInt32(reader["orientacionId"]),
                    Descripcion = reader["descripcion"].ToString() ?? string.Empty,
                    Genero = new Genero { Descripcion = reader["generoDescripcion"].ToString() ?? string.Empty },
                    Orientacion = new Orientacion { Descripcion = reader["orientacionDescripcion"].ToString() ?? string.Empty }
                };
            }

            return null;
        }

        public async Task<int> InsertAsync(Perfil perfil)
        {
            if (perfil == null)
                throw new ArgumentNullException(nameof(perfil));

            const string query = @"
                INSERT INTO perfil (nombre, edad, generoId, orientacionId, descripcion)
                VALUES (@nombre, @edad, @generoId, @orientacionId, @descripcion);
                SELECT LAST_INSERT_ID();";

            using var transaction = await _connection.BeginTransactionAsync();

            try
            {
                using var command = new MySqlCommand(query, _connection, (MySqlTransaction)transaction);
                command.Parameters.AddWithValue("@nombre", perfil.Nombre);
                command.Parameters.AddWithValue("@edad", perfil.Edad);
                command.Parameters.AddWithValue("@generoId", perfil.GeneroId);
                command.Parameters.AddWithValue("@orientacionId", perfil.OrientacionId);
                command.Parameters.AddWithValue("@descripcion", perfil.Descripcion);

                var result = await command.ExecuteScalarAsync();
                await transaction.CommitAsync();

                return Convert.ToInt32(result); // Retorna el ID insertado
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<bool> UpdateAsync(Perfil perfil)
        {
            if (perfil == null)
                throw new ArgumentNullException(nameof(perfil));

            const string query = "UPDATE perfil SET nombre = @nombre, edad = @edad, generoId = @generoId, orientacionId = @orientacionId WHERE id = @id";
            using var transaction = await _connection.BeginTransactionAsync();

            try
            {
                using var command = new MySqlCommand(query, _connection, transaction);
                command.Parameters.AddWithValue("@nombre", perfil.Nombre);
                command.Parameters.AddWithValue("@edad", perfil.Edad);
                command.Parameters.AddWithValue("@generoId", perfil.GeneroId);
                command.Parameters.AddWithValue("@orientacionId", perfil.OrientacionId);
                command.Parameters.AddWithValue("@id", perfil.Id);

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
            const string query = "DELETE FROM perfil WHERE id = @Id";
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

        public async Task<IEnumerable<Perfil>> GetRandomProfilesAsync(int cantidad = 10)
        {
            const string query = @"
                SELECT 
                    p.id, 
                    p.nombre, 
                    p.edad, 
                    p.generoId, 
                    p.orientacionId, 
                    p.descripcion,
                    g.descripcion as generoDescripcion,
                    o.descripcion as orientacionDescripcion
                FROM perfil p
                LEFT JOIN genero g ON p.generoId = g.id
                LEFT JOIN orientacion o ON p.orientacionId = o.id
                ORDER BY RAND()
                LIMIT @Cantidad";

            var perfiles = new List<Perfil>();

            using var command = new MySqlCommand(query, _connection);
            command.Parameters.AddWithValue("@Cantidad", cantidad);

            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                perfiles.Add(new Perfil
                {
                    Id = Convert.ToInt32(reader["id"]),
                    Nombre = reader["nombre"].ToString() ?? string.Empty,
                    Edad = Convert.ToInt32(reader["edad"]),
                    GeneroId = Convert.ToInt32(reader["generoId"]),
                    OrientacionId = Convert.ToInt32(reader["orientacionId"]),
                    Descripcion = reader["descripcion"].ToString() ?? string.Empty,
                    Genero = new Genero { Descripcion = reader["generoDescripcion"].ToString() ?? string.Empty },
                    Orientacion = new Orientacion { Descripcion = reader["orientacionDescripcion"].ToString() ?? string.Empty }
                });
            }

            return perfiles;
        }
    }
}
