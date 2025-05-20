using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace campuslove.domain.entities
{
public class Usuario
{
    public int Id { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
    public int PerfilId { get; set; }
    public int Likes { get; set; }

    public Perfil Perfil { get; set; }

    public ICollection<Match> MatchesComoUsuario1 { get; set; }
    public ICollection<Match> MatchesComoUsuario2 { get; set; }

    public ICollection<Interaccion> InteraccionesEnviadas { get; set; }
    public ICollection<Interaccion> InteraccionesRecibidas { get; set; }

    public Estadistica Estadisticas { get; set; }
}

}