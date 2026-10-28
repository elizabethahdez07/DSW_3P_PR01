using DSW_3P_PR01.Models;

namespace DSW_3P_PR01.Repositories;

public interface IMisionRepository
{
    List<Mision> ObtenerTodas();
    Mision? ObtenerPorId(int id);
    Mision Agregar(Mision nueva);
    bool Actualizar(int id, Mision actualizada);
    bool Eliminar(int id);
}
