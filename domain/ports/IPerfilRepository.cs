using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using campuslove.domain.entities;
using campuslove.domain.ports;

namespace campuslove.domain.ports
;

public interface IPerfilRepository : IGenericRepository<Perfil> 
{
    Task<IEnumerable<Perfil>> GetRandomProfilesAsync(int cantidad = 10);
}