using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace campuslove.domain.entities
{
public class Perfil
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public int Edad { get; set; }
    public int GeneroId { get; set; }
    public int OrientacionId { get; set; }
    public string Descripcion { get; set; } = string.Empty;

    public Genero Genero { get; set; }
    public Orientacion Orientacion { get; set; }
    public ICollection<PerfilIntereses> PerfilIntereses { get; set; } = new List<PerfilIntereses>();
    public ICollection<Usuario> Usuarios { get; set; } = new List<Usuario>();
}

}