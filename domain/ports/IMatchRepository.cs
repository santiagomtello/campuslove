using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using campuslove.domain.entities;
using campuslove.domain.ports;

namespace campuslove.domain.ports
;

public interface IMatchRepository : IGenericRepository<Match> 
{
    Task<IEnumerable<Match>> GetMatchesByUsuarioIdAsync(int usuarioId);
    Task<bool> ExisteMatchAsync(int usuario1Id, int usuario2Id);
}