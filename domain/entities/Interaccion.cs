using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace campuslove.domain.entities
{
public class Interaccion
{
    public int Id { get; set; }

    public int UsuarioEnviaId { get; set; }
    public int UsuarioRecibeId { get; set; }

    public int TipoInteraccionId { get; set; }

    public DateTime Fecha { get; set; }

    public Usuario UsuarioEnvia { get; set; }
    public Usuario UsuarioRecibe { get; set; }

    public TipoInteraccion TipoInteraccion { get; set; }
}

}