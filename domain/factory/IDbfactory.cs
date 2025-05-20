using System;
using campuslove.domain.entities;
using campuslove.domain.ports;

namespace campuslove.domain.factory;

public interface IDbFactory
{
    IGeneroRepository CreateGeneroRepository();
    IOrientacionRepository CreateOrientacionRepository();
    IInteresesRepository CreateInteresesRepository();
    IPerfilRepository CreatePerfilRepository();

    IPerfilInteresRepository CreatePerfilInteresRepository();
    IUsuarioRepository CreateUsuarioRepository();
    IMatchRepository CreateMatchRepository();
    IEstadisticasRepository CreateEstadisticasRepository();}