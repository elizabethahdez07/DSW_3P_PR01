using DSW_3P_PR01.Data;
using DSW_3P_PR01.Models;
using Microsoft.Identity.Client.Extensibility;
namespace DSW_3P_PR01.Repositories;

public class MisionSQLRepository : IMisionRepository
{
    
    private readonly AppDbContext _context;

    public MisionSQLRepository(AppDbContext context)
    {
        _context = context;
    }

    public List<Mision> ObtenerTodas()
    {
        return _context.Misiones.ToList();
    }

    public Mision? ObtenerPorId(int id)
    {
        return _context.Misiones.Find(id);
    }

    public Mision Agregar(Mision mision)
    {
        mision.Id = 0;

        _context.Misiones.Add(mision);
        _context.SaveChanges();

        return mision;
    }

    public bool Actualizar(int id, Mision mision)
    {
        var actual = _context.Misiones.Find(id);

        if(actual == null)
            return false;

        actual.Nombre = mision.Nombre;
        actual.Dificultad = mision.Dificultad;
        actual.RecompensaXP = mision.RecompensaXP;
        actual.Completada = mision.Completada;

        _context.SaveChanges();

        return true;
    }

    public bool Eliminar(int id)
    {
        var mision = _context.Misiones.Find(id);

        if(mision == null)
            return false;
        
        _context.Misiones.Remove(mision);
        _context.SaveChanges();

        return true;
    }
}