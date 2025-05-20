using System.Threading.Tasks;

namespace campuslove.domain.ports
{
    public interface IEstadisticasRepository
    {
        Task<(string Nombre, int TotalLikes)?> ObtenerPerfilConMasLikesAsync();
        Task<(string Nombre, int TotalLikes)?> ObtenerPerfilConMenosLikesAsync();
        Task<(string Nombre, int TotalMatches)?> ObtenerPerfilConMasMatchesAsync();
        Task<(string Nombre, int TotalMatches)?> ObtenerPerfilConMenosMatchesAsync();
        Task<(string Username, int TotalInteracciones)?> ObtenerUsuarioConMasInteraccionesAsync();
        Task<int> ObtenerTotalUsuariosAsync();
        Task<int> ObtenerTotalMatchesAsync();
        Task<int> ObtenerTotalLikesAsync();
        Task<int> ObtenerTotalDislikesAsync();
    }
} 