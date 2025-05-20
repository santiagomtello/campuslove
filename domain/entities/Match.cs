using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace campuslove.domain.entities
{
    public class Match
    {
        public int Id { get; set; }
        public int Usuario1Id { get; set; }
        public int Usuario2Id { get; set; }
        public DateTime Fecha { get; set; }
        public string Usuario1Username { get; set; } = string.Empty;
        public string Usuario2Username { get; set; } = string.Empty;
        public Usuario Usuario1 { get; set; }
        public Usuario Usuario2 { get; set; }
    }
}