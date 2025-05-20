using System;
using System.Threading.Tasks;
using System.Linq;
using campuslove.domain.entities;
using CampusLove.Infrastructure.repositories;
using MySql.Data.MySqlClient;

namespace campuslove.application.ui
{
    public class VerMatchesMenu
    {
        private readonly MatchRepository _matchRepository;
        private readonly UsuarioRepository _usuarioRepository;
        private readonly PerfilRepository _perfilRepository;

        public VerMatchesMenu(MySqlConnection connection)
        {
            _matchRepository = new MatchRepository(connection);
            _usuarioRepository = new UsuarioRepository(connection);
            _perfilRepository = new PerfilRepository(connection);
        }

        public async Task MostrarMenu(Usuario usuarioActual)
        {
            bool volverAlMenu = false;
            while (!volverAlMenu)
            {
                Console.Clear();
                Console.WriteLine("=== 💞 VER MATCHES 💞 ===");
                Console.WriteLine("\nSelecciona una opción:");
                Console.WriteLine("1. 👥 Ver Todos los Matches");
                Console.WriteLine("0. ↩️ Volver al Menú");

                string opcion = Console.ReadLine();

                try
                {
                    switch (opcion)
                    {
                        case "1":
                            await VerTodosLosMatches(usuarioActual);
                            break;
                        case "0":
                            volverAlMenu = true;
                            break;
                        default:
                            Console.WriteLine("Opción no válida. Por favor, intente nuevamente.");
                            Console.ReadKey();
                            break;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"\n❌ Error: {ex.Message}");
                    Console.ReadKey();
                }
            }
        }

        private async Task VerTodosLosMatches(Usuario usuarioActual)
        {
            Console.Clear();
            Console.WriteLine("=== 💞 TODOS LOS MATCHES 💞 ===\n");

            try
            {
                var matches = await _matchRepository.GetMatchesByUsuarioIdAsync(usuarioActual.Id);
                var matchesUsuario = matches.Where(m => m.Usuario1Id == usuarioActual.Id || m.Usuario2Id == usuarioActual.Id).ToList();

                if (!matchesUsuario.Any())
                {
                    Console.WriteLine("No se encontraron matches.");
                }
                else
                {
                    Console.WriteLine($"Total de matches: {matchesUsuario.Count}\n");
                    Console.WriteLine("Fecha del Match\t\tMatch con\t\tUsuario\t\tEstado");
                    Console.WriteLine("----------------------------------------------------------------");

                    foreach (var match in matchesUsuario)
                    {
                        int usuarioMatchId = match.Usuario1Id == usuarioActual.Id ? match.Usuario2Id : match.Usuario1Id;
                        var usuarioMatch = await _usuarioRepository.GetByIdAsync(usuarioMatchId);
                        if (usuarioMatch == null) continue;
                        var perfilMatch = await _perfilRepository.GetByIdAsync(usuarioMatch.PerfilId);
                        if (perfilMatch == null) continue;

                        string estado = match.Fecha >= DateTime.Now.AddDays(-1) 
                            ? "¡Nuevo!" 
                            : match.Fecha >= DateTime.Now.AddDays(-7) 
                                ? "Reciente" 
                                : "Antiguo";

                        Console.WriteLine($"{match.Fecha:dd/MM/yyyy HH:mm}\t{perfilMatch.Nombre}\t\t{usuarioMatch.Username}\t\t{estado}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\n❌ Error al ver matches: {ex.Message}");
            }

            Console.WriteLine("\nPresiona cualquier tecla para continuar...");
            Console.ReadKey();
        }

        
    }
} 