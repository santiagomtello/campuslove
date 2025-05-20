using System;
using System.Threading.Tasks;
using CampusLove.Infrastructure.repositories;
using campuslove.application.ui;
using campusLove.infrastructure.configuration;


namespace CampusLove.Application.ui
{
    public class MainMenu
    {
        private readonly MenuRegistro _menuRegistro;
        private readonly LoginMenu _loginMenu;
        private readonly MenuEstadisticas _menuEstadisticas;
        public MainMenu()
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.Title = "  CampusLove ... Where is Love 💞   ";

            var connection = DatabaseConfig.GetConnection();
            _menuRegistro = new MenuRegistro(connection);
            _loginMenu = new LoginMenu(connection);
            _menuEstadisticas = new MenuEstadisticas(connection);
        }
        public async Task MostrarMenu()
        {
            bool salir = false;

            while (!salir)
            {
                Console.Clear();
                MostrarEncabezado(" CAMPUSLOVE ");

                Console.WriteLine("\n MENÚ PRINCIPAL ");
                Console.ResetColor();

                Console.ForegroundColor = ConsoleColor.Red	;
                Console.WriteLine("1.  Registrarse");
                Console.WriteLine("2.  Iniciar Sesión");
                Console.WriteLine("3.  Mostrar estadísticas del sistema");
                Console.WriteLine("0.  Salir");
                Console.ResetColor();

                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write("\nSeleccione una opción: ");
                Console.ResetColor();
                string opcion = Console.ReadLine() ?? "";

                switch (opcion)
                {
                    case "1":
                        await _menuRegistro.registrarUsuario();
                        break;
                    case "2":
                        await _loginMenu.validarLogin();
                        break;
                    case "3":
                        await _menuEstadisticas.MostrarMenu();
                        break;
                    case "0":
                        salir = true;
                        break;
                    default:
                        MostrarMensaje("⚠️ Opción no válida. Intente de nuevo.", ConsoleColor.DarkMagenta);
                        Console.ReadKey();
                        break;
                }
            }

            MostrarMensaje("\n ¡Gracias por usar Campus Love! ", ConsoleColor.DarkGreen);
        }

        private void MostrarMensaje(string mensaje, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            Console.WriteLine($"\n{mensaje}");
            Console.ResetColor();
        }


        public static void MostrarEncabezado(string titulo)
        {
            Console.ForegroundColor = ConsoleColor.Magenta;

            string borde = new string('═', titulo.Length + 6);
            Console.WriteLine($"╔{borde}╗");
            Console.WriteLine($"║  {titulo}    ║");
            Console.WriteLine($"╚{borde}╝");

            Console.ResetColor();
        }
    }


}