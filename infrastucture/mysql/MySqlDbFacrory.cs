using campuslove.domain.factory;
using campuslove.domain.ports;
using CampusLove.Infrastructure.Mysql;
using CampusLove.Infrastructure.repositories;


namespace campuslove.infrastructure.mysql;

public class MySqlDbFactory : IDbFactory
{
    private readonly string _connectionString;

    public MySqlDbFactory(string connectionString)
    {
        _connectionString = connectionString;
    }
    public IGeneroRepository CreateGeneroRepository()
    {
        var connection = ConexionSingleton.Instancia(_connectionString).ObtenerConexion();
        return new GeneroRepository(connection);
    }

    public IOrientacionRepository CreateOrientacionRepository()
    {
        var connection = ConexionSingleton.Instancia(_connectionString).ObtenerConexion();
        return new OrientacionRepository(connection);
    }

    public IInteresesRepository CreateInteresesRepository()
    {
        var connection = ConexionSingleton.Instancia(_connectionString).ObtenerConexion();
        return new InteresRepository(connection);
    }

    public IPerfilRepository CreatePerfilRepository()
    {
        var connection = ConexionSingleton.Instancia(_connectionString).ObtenerConexion();
        return new PerfilRepository(connection);
    }

    public IPerfilInteresRepository CreatePerfilInteresRepository()
    {
        var connection = ConexionSingleton.Instancia(_connectionString).ObtenerConexion();
        return new PerfilInteresRepository(connection);
    }
    public IUsuarioRepository CreateUsuarioRepository()
    {
        var connection = ConexionSingleton.Instancia(_connectionString).ObtenerConexion();
        return new UsuarioRepository(connection);
    }
    public IEstadisticasRepository CreateEstadisticasRepository()
    {
        var connection = ConexionSingleton.Instancia(_connectionString).ObtenerConexion();
        return new EstadisticasRepository(connection);
    }

    public IMatchRepository CreateMatchRepository()
    {
        var connection = ConexionSingleton.Instancia(_connectionString).ObtenerConexion();
        return new MatchRepository(connection);
    }

}