using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CampusLove.Infrastructure.repositories;
using MySql.Data.MySqlClient;

namespace campuslove.application.ui
{
    public class MenuEstadisticas
    {
        private readonly EstadisticasRepository _estadisticasRepository;

        public MenuEstadisticas(MySqlConnection connection)
        {
            _estadisticasRepository = new EstadisticasRepository(connection);
        }

        public async Task<bool> MostrarMenu()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("📊 Bienvenido al menú de estadísticas");
                Console.WriteLine("1. Perfil con más likes recibidos");
                Console.WriteLine("2. Perfil con menos likes recibidos");
                Console.WriteLine("3. Usuario con más interacciones realizadas");
                Console.WriteLine("0. Volver al menú principal");
                
                Console.Write("\nSeleccione una opción: ");
                string opcion = Console.ReadLine();

                try
                {
                    switch (opcion)
                    {
                        case "1":
                            await MostrarPerfilConMasLikesRecibidos();
                            break;
                        case "2":
                            await MostrarPerfilConMenosLikesRecibidos();
                            break;
                        case "3":
                            await MostrarPerfilConMasInteraccionesHechas();
                            break;
                        case "0":
                            return true;
                        default:
                            Console.WriteLine("\n❌ Opción no válida. Por favor, intente nuevamente.");
                            Console.WriteLine("\nPresione una tecla para continuar...");
                            Console.ReadKey();
                            break;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"\n❌ Error: {ex.Message}");
                    Console.WriteLine("\nPresione una tecla para continuar...");
                    Console.ReadKey();
                }
            }
        }

        private async Task MostrarPerfilConMasLikesRecibidos()
        {
            var resultado = await _estadisticasRepository.ObtenerPerfilConMasLikesAsync();
            Console.Clear();
            Console.WriteLine(" Perfil con más likes recibidos");

            if (resultado != null)
            {
                Console.WriteLine($"Perfil: {resultado.Value.Nombre}");
                Console.WriteLine($"Likes recibidos: {resultado.Value.TotalLikes}");
            }
            else
            {
                Console.WriteLine("No hay datos disponibles.");
            }

            Console.WriteLine("\nPresione una tecla para volver...");
            Console.ReadKey();
        }

        private async Task MostrarPerfilConMenosLikesRecibidos()
        {
            var resultado = await _estadisticasRepository.ObtenerPerfilConMenosLikesAsync();
            Console.Clear();
            Console.WriteLine(" Perfil con menos likes recibidos");

            if (resultado != null)
            {
                Console.WriteLine($"Perfil: {resultado.Value.Nombre}");
                Console.WriteLine($"Likes recibidos: {resultado.Value.TotalLikes}");
            }
            else
            {
                Console.WriteLine("No hay datos disponibles.");
            }

            Console.WriteLine("\nPresione una tecla para volver...");
            Console.ReadKey();
        }

        private async Task MostrarPerfilConMasMatches()
        {
            var resultado = await _estadisticasRepository.ObtenerPerfilConMasMatchesAsync();
            Console.Clear();
            Console.WriteLine(" Perfil con más matches realizados");

            if (resultado != null)
            {
                Console.WriteLine($"Perfil: {resultado.Value.Nombre}");
                Console.WriteLine($"Matches realizados: {resultado.Value.TotalMatches}");
            }
            else
            {
                Console.WriteLine("No hay datos disponibles.");
            }

            Console.WriteLine("\nPresione una tecla para volver...");
            Console.ReadKey();
        }

        private async Task MostrarPerfilConMenosMatches()
        {
            var resultado = await _estadisticasRepository.ObtenerPerfilConMenosMatchesAsync();
            Console.Clear();
            Console.WriteLine("💘 Perfil con menos matches realizados");

            if (resultado != null)
            {
                Console.WriteLine($"Perfil: {resultado.Value.Nombre}");
                Console.WriteLine($"Matches realizados: {resultado.Value.TotalMatches}");
            }
            else
            {
                Console.WriteLine("No hay datos disponibles.");
            }

            Console.WriteLine("\nPresione una tecla para volver...");
            Console.ReadKey();
        }

        private async Task MostrarPerfilConMasInteraccionesHechas()
        {
            var resultado = await _estadisticasRepository.ObtenerUsuarioConMasInteraccionesAsync();
            Console.Clear();
            Console.WriteLine("🎯 Usuario con más interacciones realizadas");

            if (resultado != null)
            {
                Console.WriteLine($"Usuario: {resultado.Value.Username}");
                Console.WriteLine($"Interacciones realizadas: {resultado.Value.TotalInteracciones}");
            }
            else
            {
                Console.WriteLine("No hay datos disponibles.");
            }

            Console.WriteLine("\nPresione una tecla para volver...");
            Console.ReadKey();
        }
    }
}