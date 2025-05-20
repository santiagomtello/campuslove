using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace campuslove.domain.entities
{
public class Interes
{
    public int Id { get; set; }
    public string Descripcion { get; set; }

    public ICollection<PerfilIntereses> PerfilIntereses { get; set; }
}

}