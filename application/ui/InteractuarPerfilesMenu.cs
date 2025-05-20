using System;
using System.Threading.Tasks;
using System.Linq;
using campuslove.domain.entities;
using CampusLove.Infrastructure.repositories;
using MySql.Data.MySqlClient;

namespace campuslove.application.ui
{
    public class InteractuarPerfilesMenu
    {
        private readonly PerfilRepository _perfilRepository;
        private readonly UsuarioRepository _usuarioRepository;
        private readonly InteraccionRepository _interaccionRepository;
        private readonly MatchRepository _matchRepository;
        private const int MAX_LIKES = 10;

        public InteractuarPerfilesMenu(MySqlConnection connection)
        {
            _perfilRepository = new PerfilRepository(connection);
            _usuarioRepository = new UsuarioRepository(connection);
            _interaccionRepository = new InteraccionRepository(connection);
            _matchRepository = new MatchRepository(connection);
        }

        public async Task MostrarMenu(Usuario usuarioActual)
        {
            bool volverAlMenu = false;
            while (!volverAlMenu)
            {
                Console.Clear();
                Console.WriteLine("===  INTERACTUAR CON PERFILES  ===");
                Console.WriteLine("1.  Ver Perfiles");
                Console.WriteLine("2. ↩ Volver al Menú Principal");

                string opcion = Console.ReadLine();

                try
                {
                    switch (opcion)
                    {
                        case "1":
                            await VerPerfiles(usuarioActual);
                            break;
                        case "2":
                            volverAlMenu = true;
                            break;
                        default:
                            Console.WriteLine(" Opción no válida. Por favor, intente nuevamente.");
                            Console.ReadKey();
                            break;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"\n Error: {ex.Message}");
                    Console.ReadKey();
                }
            }
        }

        private async Task VerPerfiles(Usuario usuarioActual)
        {
            try
            {
                Console.Clear();
                Console.WriteLine(" Buscando Perfiles...");
                
                var perfiles = await _perfilRepository.GetRandomProfilesAsync(5);
                if (perfiles == null || !perfiles.Any())
                {
                    Console.WriteLine(" No hay perfiles disponibles en el sistema.");
                    Console.ReadKey();
                    return;
                }

                var perfilesFiltrados = perfiles
                    .Where(p => p != null && p.Id != usuarioActual.PerfilId)
                    .ToList();

                if (!perfilesFiltrados.Any())
                {
                    Console.WriteLine(" No hay perfiles disponibles para mostrar.");
                    Console.ReadKey();
                    return;
                }

                await MostrarPerfiles(perfilesFiltrados, usuarioActual);
            }
            catch (Exception ex)
            {
                Console.WriteLine($" Error al cargar perfiles: {ex.Message}");
                Console.ReadKey();
            }
        }

        private async Task MostrarPerfiles(IEnumerable<Perfil> perfiles, Usuario usuarioActual)
        {
            foreach (var perfil in perfiles)
            {
                Console.Clear();
                Console.WriteLine($" Perfil de {perfil.Nombre}");
                Console.WriteLine($" Edad: {perfil.Edad}");
                Console.WriteLine($" Género: {perfil.Genero.Descripcion}");
                Console.WriteLine($" Orientación: {perfil.Orientacion.Descripcion}");
                Console.WriteLine($" Descripción: {perfil.Descripcion}");

                // Verificar si ya interactuó con este perfil
                bool yaDioLike = await _interaccionRepository.VerificarLike(usuarioActual.Id, perfil.Id);
                bool yaDioDislike = await _interaccionRepository.VerificarDislike(usuarioActual.Id, perfil.Id);

                if (yaDioLike)
                {
                    Console.WriteLine("\n⚠️ Ya le has dado like a este perfil");
                }
                else if (yaDioDislike)
                {
                    Console.WriteLine("\n⚠️ Ya le has dado dislike a este perfil");
                }
                else
                {
                    // Verificar límite de likes solo si no ha dado like ni dislike
                    int conteoLikes = await _interaccionRepository.ObtenerConteoLikes(usuarioActual.Id);
                    Console.WriteLine($"\n👍 Likes restantes hoy: {MAX_LIKES - conteoLikes}");
                }

                Console.WriteLine("\n¿Qué acción desea realizar?");
                Console.WriteLine("1. Dar Like 👍");
                Console.WriteLine("2. Dar Dislike 👎");
                Console.WriteLine("3. Ver siguiente perfil ➡️");
                Console.WriteLine("0. Volver al menú principal ↩️");

                Console.Write("\nSeleccione una opción: ");
                string opcion = Console.ReadLine();

                switch (opcion)
                {
                    case "1":
                        await ManejarLike(usuarioActual, perfil);
                        break;
                    case "2":
                        await ManejarDislike(usuarioActual, perfil);
                        break;
                    case "3":
                        continue;
                    case "0":
                        return;
                    default:
                        Console.WriteLine("\n❌ Opción no válida. Por favor, intente nuevamente.");
                        Console.WriteLine("\nPresione una tecla para continuar...");
                        Console.ReadKey();
                        break;
                }
            }

            Console.WriteLine("\nNo hay más perfiles disponibles por el momento.");
            Console.WriteLine("Presione una tecla para volver al menú principal...");
            Console.ReadKey();
        }

        private async Task ManejarLike(Usuario usuarioActual, Perfil perfilDestino)
        {
            try
            {
                // Verificar si ya dio like o dislike
                bool yaDioLike = await _interaccionRepository.VerificarLike(usuarioActual.Id, perfilDestino.Id);
                bool yaDioDislike = await _interaccionRepository.VerificarDislike(usuarioActual.Id, perfilDestino.Id);

                if (yaDioLike)
                {
                    Console.WriteLine("\n⚠️ Ya le has dado like a este perfil");
                    Console.ReadKey();
                    return;
                }

                if (yaDioDislike)
                {
                    Console.WriteLine("\n⚠️ Ya le has dado dislike a este perfil");
                    Console.ReadKey();
                    return;
                }

                // Verificar límite de likes
                int conteoLikes = await _interaccionRepository.ObtenerConteoLikes(usuarioActual.Id);
                if (conteoLikes >= MAX_LIKES)
                {
                    Console.WriteLine("\n⚠️ Has alcanzado tu límite diario de likes");
                    Console.ReadKey();
                    return;
                }

                // Crear el like
                var interaccion = new Interaccion
                {
                    UsuarioEnviaId = usuarioActual.Id,
                    UsuarioRecibeId = perfilDestino.Id,
                    TipoInteraccionId = 1, // Asumiendo que 1 es el ID para likes
                    Fecha = DateTime.Now
                };

                await _interaccionRepository.InsertAsync(interaccion);

                // Verificar si hay match
                bool hayMatch = await VerificarMatch(usuarioActual.Id, perfilDestino.Id);
                if (hayMatch)
                {
                    // Crear el match
                    var match = new Match
                    {
                        Usuario1Id = usuarioActual.Id,
                        Usuario2Id = perfilDestino.Id,
                        Fecha = DateTime.Now,
                    };

                    await _matchRepository.InsertAsync(match);
                    Console.WriteLine("\n🎉 ¡MATCH! ¡Se gustaron mutuamente!");
                }
                else
                {
                    Console.WriteLine("\a 👍n ¡Like enviado!");
                }
                Console.ReadKey();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\n❌ Error al enviar like: {ex.Message}");
                Console.ReadKey();
            }
        }

        private async Task ManejarDislike(Usuario usuarioActual, Perfil perfilDestino)
        {
            try
            {
                // Verificar si ya dio like o dislike
                bool yaDioLike = await _interaccionRepository.VerificarLike(usuarioActual.Id, perfilDestino.Id);
                bool yaDioDislike = await _interaccionRepository.VerificarDislike(usuarioActual.Id, perfilDestino.Id);

                if (yaDioLike)
                {
                    Console.WriteLine("\n⚠️ Ya le has dado like a este perfil");
                    Console.ReadKey();
                    return;
                }

                if (yaDioDislike)
                {
                    Console.WriteLine("\n⚠️ Ya le has dado dislike a este perfil");
                    Console.ReadKey();
                    return;
                }

                // Crear el dislike
                var interaccion = new Interaccion
                {
                    UsuarioEnviaId = usuarioActual.Id,
                    UsuarioRecibeId = perfilDestino.Id,
                    TipoInteraccionId = 2, // Asumiendo que 2 es el ID para dislikes
                    Fecha = DateTime.Now
                };

                await _interaccionRepository.InsertAsync(interaccion);
                Console.WriteLine("\n👎 ¡Dislike enviado!");
                Console.ReadKey();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\n❌ Error al enviar dislike: {ex.Message}");
                Console.ReadKey();
            }
        }

        private async Task<bool> VerificarMatch(int usuarioId, int perfilDestinoId)
        {
            // Verificar si el usuario que recibió el like también le dio like al usuario actual
            return await _interaccionRepository.VerificarLike(perfilDestinoId, usuarioId);
        }
    }
} 