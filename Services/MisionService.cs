using DSW_3P_PR01.Models;
using DSW_3P_PR01.Repositories;

namespace DSW_3P_PR01.Services;

public class MisionService(IMisionRepository repository) : IMisionService
{
    public List<Mision> ObtenerTodas() => repository.ObtenerTodas();
    public Mision? ObtenerPorId(int id) => repository.ObtenerPorId(id);

    private static void Validar(Mision mision)
    {
        if (string.IsNullOrWhiteSpace(mision.Nombre))
            throw new ArgumentException("El nombre de la misión es obligatorio.");
        if (mision.Dificultad is not ("Fácil" or "Media" or "Difícil"))
            throw new ArgumentException("La dificultad debe ser Fácil, Media o Difícil.");
        if (mision.RecompensaXP < 0)
            throw new ArgumentException("La recompensa XP no puede ser negativa.");
        mision.Nombre = mision.Nombre.Trim();
    }

    public Mision Agregar(Mision nueva)
    {
        Validar(nueva);
        nueva.Completada = false;
        return repository.Agregar(nueva);
    }

    public bool Actualizar(int id, Mision actualizada)
    {
        if (repository.ObtenerPorId(id) is null) return false;
        Validar(actualizada);
        return repository.Actualizar(id, actualizada);
    }

    public bool Eliminar(int id) => repository.Eliminar(id);
}
