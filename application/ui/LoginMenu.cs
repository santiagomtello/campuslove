using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using campuslove.domain.entities;
using campuslove.domain.ports;
using CampusLove.Infrastructure.repositories;
using MySql.Data.MySqlClient;
using Org.BouncyCastle.Bcpg;

namespace campuslove.application.ui
{
    public class LoginMenu
    {
        private readonly UsuarioRepository _usuarioRepository;
        private readonly PerfilRepository _perfilRepository;
        private readonly InteractuarPerfilesMenu _interactuarPerfilesMenu;
        private readonly VerMatchesMenu _verMatchesMenu;

        public LoginMenu(MySqlConnection connection)
        {
            _usuarioRepository = new UsuarioRepository(connection);
            _perfilRepository = new PerfilRepository(connection);
            _interactuarPerfilesMenu = new InteractuarPerfilesMenu(connection);
            _verMatchesMenu = new VerMatchesMenu(connection);
        }

        public async Task validarLogin()
        {
            bool loginSuccessful = false;
            while (!loginSuccessful)
            {
                Console.Clear();
                Console.WriteLine("Bienvenido a CampusLove");
                Console.WriteLine("Por favor, ingresa tu nombre de usuario y contraseña para iniciar sesión.");
                Console.Write("Usuario: ");
                string username = Console.ReadLine();
                Console.Write("Contraseña: ");
                string password = Console.ReadLine();

                if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
                {
                    Console.WriteLine("Por favor, completa todos los campos.");
                    Console.ReadKey();
                    continue;
                }

                var user = await _usuarioRepository.ObtenerPorusername(username);
                if (user == null)
                {
                    Console.WriteLine("Usuario no encontrado.");
                    Console.ReadKey();
                    continue;
                }

                if (user.Password != password)
                {
                    Console.WriteLine("Contraseña incorrecta.");
                    Console.ReadKey();
                    continue;
                }

                Console.WriteLine($"¡Bienvenido {user.Username}!");
                loginSuccessful = true;
                await mostrarusermenu(user);
            }
        }

        public async Task mostrarusermenu(Usuario usuarioactual)
        {
            bool returnToMain = false;

            while (!returnToMain)
            {
                Console.Clear();
                Console.WriteLine("Bienvenido al menú de usuario");
                Console.WriteLine("1. Ver perfil");
                Console.WriteLine("2. interactuar con otros perfiles");
                Console.WriteLine("3. ver matches");
                Console.WriteLine("0. Volver al menú principal");

                string opcion = Console.ReadLine();

                switch (opcion)
                {
                    case "1":
                        await MostrarPerfil(usuarioactual);
                        break;
                    case "2":
                        await _interactuarPerfilesMenu.MostrarMenu(usuarioactual);
                        break;
                    case "3":
                        await _verMatchesMenu.MostrarMenu(usuarioactual);
                        break;
                    case "0":
                        returnToMain = true;
                        break;
                    default:
                        Console.WriteLine("Opción no válida, por favor intente nuevamente.");
                        Console.ReadKey();
                        break;
                }
            }
        }

        private async Task MostrarPerfil(Usuario usuario)
        {
            try
            {
                var perfil = await _perfilRepository.GetByIdAsync(usuario.PerfilId);
                if (perfil == null)
                {
                    Console.WriteLine("No se encontró el perfil asociado a este usuario.");
                    return;
                }

                Console.Clear();
                Console.WriteLine("=== MI PERFIL ===");
                Console.WriteLine($"Nombre: {perfil.Nombre}");
                Console.WriteLine($"Edad: {perfil.Edad}");
                Console.WriteLine($"Descripción: {perfil.Descripcion}");
                Console.WriteLine($"Género: {perfil.Genero.Descripcion}");
                Console.WriteLine($"Orientación: {perfil.Orientacion.Descripcion}");
                Console.WriteLine("\nPresione cualquier tecla para volver al menú...");
                Console.ReadKey();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al mostrar el perfil: {ex.Message}");
                Console.WriteLine("Presione cualquier tecla para volver al menú...");
                Console.ReadKey();
            }
        }
    }
}