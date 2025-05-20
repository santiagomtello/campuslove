using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using campuslove.domain.entities;
using CampusLove.Infrastructure.repositories;
using MySql.Data.MySqlClient;

namespace campuslove.application.ui
{
    public class MenuRegistro
    {
        private readonly GeneroRepository _generoRepository;
        private readonly OrientacionRepository _orientacionRepository;
        private readonly InteresRepository _interesRepository;
        private readonly PerfilRepository _perfilRepository;
        private readonly PerfilInteresRepository _perfilInteresRepository;
        private readonly UsuarioRepository _usuarioRepository;

        public MenuRegistro(MySqlConnection connection)
        {
            _generoRepository = new GeneroRepository(connection);
            _orientacionRepository = new OrientacionRepository(connection);
            _interesRepository = new InteresRepository(connection);
            _perfilRepository = new PerfilRepository(connection);
            _perfilInteresRepository = new PerfilInteresRepository(connection);
            _usuarioRepository = new UsuarioRepository(connection);
        }


        public async Task registrarUsuario()
        {
            // Aquí puedes implementar la lógica para registrar un nuevo usuario
            Console.WriteLine("Registro de usuario");
            Console.Write("Ingrese su nombre: ");
            string nombre = Console.ReadLine() ?? "";
            Console.Write("Ingrese su edad: ");
            string edadInput = Console.ReadLine() ?? "";
            int edad;
            while (!int.TryParse(edadInput, out edad))
            {
                Console.Write("Entrada no válida. Ingrese su edad: ");
                edadInput = Console.ReadLine() ?? "";
            }

            Console.Write("Ingrese su género:\n ");
            var generos = await _generoRepository.GetAllAsync();
                    foreach (var genero in generos)
                    {
                        Console.WriteLine($"ID: {genero.Id} - {genero.Descripcion}");
                    }
            string generoInput = Console.ReadLine() ?? "";
            int generoId;
            while (!int.TryParse(generoInput, out generoId) || !generos.Any(g => g.Id == generoId))
            {
                Console.Write("Entrada no válida. Ingrese el ID de su género: ");
                generoInput = Console.ReadLine() ?? "";
            }

            Console.Write("Ingrese su orientación:\n ");
            var orientaciones = await _orientacionRepository.GetAllAsync();
                    foreach (var orientacion in orientaciones)
                    {
                        Console.WriteLine($"ID: {orientacion.Id} - {orientacion.Descripcion}");
                    }
            string orientacionInput = Console.ReadLine() ?? "";
            int orientacionId;
            while (!int.TryParse(orientacionInput, out orientacionId) || !orientaciones.Any(o => o.Id == orientacionId))
            {
                Console.Write("Entrada no válida. Ingrese el ID de su orientación: ");
                orientacionInput = Console.ReadLine() ?? "";
            }
            Console.WriteLine("ingrese su descripcion");
            string descripcion = Console.ReadLine() ?? "";


            Console.WriteLine("seleccione intereses");
            var intereses = await _interesRepository.GetAllAsync();
            foreach (var interes in intereses)
            {
                Console.WriteLine($"ID: {interes.Id} - {interes.Descripcion}");
                }
                Console.WriteLine("0. Terminar selección");

            List<int> interesIds = new List<int>();
            string interesInput;
            do
            {
                Console.Write("Ingrese el ID de su interés (o 0 para terminar): ");
                interesInput = Console.ReadLine() ?? "";
                if (interesInput.ToLower() != "0" && int.TryParse(interesInput, out int interesId) && intereses.Any(i => i.Id == interesId))
                {
                    interesIds.Add(interesId);
                }
            } while (interesInput.ToLower() != "0"); 


            Console.WriteLine("ingrese su usuario");
            string usuario = Console.ReadLine() ?? "";
            Console.WriteLine("ingrese su contraseña");
            string contrasena = Console.ReadLine() ?? "";

            Console.WriteLine("desea registrar este perfil? (si/no)");
            string ingresoperfil = Console.ReadLine() ?? "";
            if (ingresoperfil.ToLower() == "si")
            {
                int nuevoPerfilId = await _perfilRepository.InsertAsync(new Perfil
                {
                    Nombre = nombre,
                    Edad = edad,
                    GeneroId = generoId,
                    OrientacionId = orientacionId,
                    Descripcion = descripcion
                });

                Console.WriteLine($"Perfil registrado con ID: {nuevoPerfilId}");

                foreach (var interesId in interesIds)
                {
                    await _perfilInteresRepository.InsertAsync(new PerfilIntereses
                    {
                        PerfilId = nuevoPerfilId,
                        InteresesId = interesId
                    });
                }

                var nuevoUsuario = new Usuario
                {
                    Username = usuario,
                    Password = contrasena,
                    PerfilId = nuevoPerfilId
                };
                int nuevoUsuarioId = await _usuarioRepository.InsertAsync(nuevoUsuario);
                Console.WriteLine($"Usuario registrado con ID: {nuevoUsuarioId}");
            }
            else
            {
                Console.WriteLine("Registro cancelado.");
            }




            await Task.Delay(1000);
            Console.WriteLine("Usuario registrado con éxito.");
        }
    }
}