using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace campuslove.domain.entities
{
public class PerfilIntereses
{
    public int Id { get; set; }

    public int InteresesId { get; set; }
    public int PerfilId { get; set; }

    public Interes Interes { get; set; }
    public Perfil Perfil { get; set; }
}
}