using MySql.Data.MySqlClient;

namespace campusLove.infrastructure.configuration
{
    public static class DatabaseConfig
    {
        private static string _connectionString = "Server=localhost;Database=campusLove;User=root;Password=2512;";
        private static MySqlConnection _connection;

        public static MySqlConnection GetConnection()
        {
            if (_connection == null)
            {
                _connection = new MySqlConnection(_connectionString);
            }

            if (_connection.State != System.Data.ConnectionState.Open)
            {
                _connection.Open();
            }

            return _connection;
        }

        public static void CloseConnection()
        {
            if (_connection != null && _connection.State == System.Data.ConnectionState.Open)
            {
                _connection.Close();
                _connection.Dispose();
                _connection = null;
            }
        }
    }
} 