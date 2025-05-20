using System;
using System.Threading.Tasks;
using campuslove.domain.entities;
using campuslove.domain.ports;
using MySql.Data.MySqlClient;

namespace CampusLove.Infrastructure.repositories
{
    public class EstadisticasRepository : IEstadisticasRepository
    {
        private readonly MySqlConnection _connection;

        public EstadisticasRepository(MySqlConnection connection)
        {
            _connection = connection;
        }

        public async Task<(string Nombre, int TotalLikes)?> ObtenerPerfilConMasLikesAsync()
        {
            const string query = @"
                SELECT p.nombre, COUNT(i.id) AS totalLikes
                FROM perfil p
                JOIN usuario u ON p.id = u.perfilId
                JOIN interaccion i ON u.id = i.usuarioRecibeId AND i.tipoInteraccionId = 1
                GROUP BY p.id, p.nombre
                ORDER BY totalLikes DESC
                LIMIT 1;";

            using var command = new MySqlCommand(query, _connection);
            using var reader = await command.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                return (
                    reader["nombre"].ToString() ?? "",
                    Convert.ToInt32(reader["totalLikes"])
                );
            }

            return null;
        }

        public async Task<(string Nombre, int TotalLikes)?> ObtenerPerfilConMenosLikesAsync()
        {
            const string query = @"
                SELECT p.nombre, COUNT(i.id) AS totalLikes
                FROM perfil p
                JOIN usuario u ON p.id = u.perfilId
                JOIN interaccion i ON u.id = i.usuarioRecibeId AND i.tipoInteraccionId = 1
                GROUP BY p.id, p.nombre
                ORDER BY totalLikes ASC
                LIMIT 1;";

            using var command = new MySqlCommand(query, _connection);
            using var reader = await command.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                return (
                    reader["nombre"].ToString() ?? "",
                    Convert.ToInt32(reader["totalLikes"])
                );
            }

            return null;
        }

        public async Task<(string Nombre, int TotalMatches)?> ObtenerPerfilConMasMatchesAsync()
        {
            const string query = @"
                SELECT p.nombre, COUNT(m.id) AS totalMatches
                FROM perfil p
                JOIN usuario u ON p.id = u.perfilId
                JOIN match m ON u.id = m.usuario1Id OR u.id = m.usuario2Id
                GROUP BY p.id, p.nombre
                ORDER BY totalMatches DESC
                LIMIT 1;";

            using var command = new MySqlCommand(query, _connection);
            using var reader = await command.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                return (
                    reader["nombre"].ToString() ?? "",
                    Convert.ToInt32(reader["totalMatches"])
                );
            }

            return null;
        }

        public async Task<(string Nombre, int TotalMatches)?> ObtenerPerfilConMenosMatchesAsync()
        {
            const string query = @"
                SELECT p.nombre, COUNT(m.id) AS totalMatches
                FROM perfil p
                JOIN usuario u ON p.id = u.perfilId
                JOIN match m ON u.id = m.usuario1Id OR u.id = m.usuario2Id
                GROUP BY p.id, p.nombre
                ORDER BY totalMatches ASC
                LIMIT 1;";

            using var command = new MySqlCommand(query, _connection);
            using var reader = await command.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                return (
                    reader["nombre"].ToString() ?? "",
                    Convert.ToInt32(reader["totalMatches"])
                );
            }

            return null;
        }

        public async Task<(string Username, int TotalInteracciones)?> ObtenerUsuarioConMasInteraccionesAsync()
        {
            const string query = @"
                SELECT u.username, COUNT(i.id) AS totalInteracciones
                FROM usuario u
                JOIN interaccion i ON i.usuarioEnviaId = u.id
                GROUP BY u.id, u.username
                ORDER BY totalInteracciones DESC
                LIMIT 1;";

            using var command = new MySqlCommand(query, _connection);
            using var reader = await command.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                return (
                    reader["username"].ToString() ?? "",
                    Convert.ToInt32(reader["totalInteracciones"])
                );
            }

            return null;
        }


        public async Task<int> ObtenerTotalUsuariosAsync()
        {
            const string query = @"SELECT COUNT(*) AS total FROM usuario";

            using var command = new MySqlCommand(query, _connection);
            object result = await command.ExecuteScalarAsync();

            return result != null ? Convert.ToInt32(result) : 0;
        }

        public async Task<int> ObtenerTotalMatchesAsync()
        {
            const string query = @"SELECT COUNT(*) AS total FROM match";

            using var command = new MySqlCommand(query, _connection);
            object result = await command.ExecuteScalarAsync();

            return result != null ? Convert.ToInt32(result) : 0;
        }

        public async Task<int> ObtenerTotalLikesAsync()
        {
            const string query = @"
                SELECT COUNT(*) AS total 
                FROM interaccion 
                WHERE tipoInteraccionId = 1";

            using var command = new MySqlCommand(query, _connection);
            object result = await command.ExecuteScalarAsync();

            return result != null ? Convert.ToInt32(result) : 0;
        }

        public async Task<int> ObtenerTotalDislikesAsync()
        {
            const string query = @"
                SELECT COUNT(*) AS total 
                FROM interaccion 
                WHERE tipoInteraccionId = 2";

            using var command = new MySqlCommand(query, _connection);
            object result = await command.ExecuteScalarAsync();

            return result != null ? Convert.ToInt32(result) : 0;
        }
    }
} 