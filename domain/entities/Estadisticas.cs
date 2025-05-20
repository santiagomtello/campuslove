using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace campuslove.domain.entities
{
public class Estadistica
{
    public int Id { get; set; }
    public int UsuarioId { get; set; }

    public int LikesDados { get; set; }
    public int LikesRecibidos { get; set; }
    public int DislikesDados { get; set; }
    public int DislikesRecibidos { get; set; }
    public int MatchHechos { get; set; }

    public Usuario Usuario { get; set; }
}

}