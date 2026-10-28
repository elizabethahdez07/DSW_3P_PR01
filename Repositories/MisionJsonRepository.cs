using System.Text.Json;
using DSW_3P_PR01.Models;

namespace DSW_3P_PR01.Repositories;

public class MisionJsonRepository : IMisionRepository
{
    private readonly string _ruta;
    private readonly object _candado = new();
    private readonly JsonSerializerOptions _opciones = new()
    {
        WriteIndented = true,
        PropertyNameCaseInsensitive = true
    };

    public MisionJsonRepository(IWebHostEnvironment environment)
    {
        _ruta = Path.Combine(environment.ContentRootPath, "Data", "misiones.json");
        Directory.CreateDirectory(Path.GetDirectoryName(_ruta)!);
        if (!File.Exists(_ruta)) File.WriteAllText(_ruta, "[]");
    }

    private List<Mision> Leer() =>
        JsonSerializer.Deserialize<List<Mision>>(File.ReadAllText(_ruta), _opciones)
        ?? throw new JsonException("misiones.json debe contener un arreglo JSON.");

    private void Guardar(List<Mision> datos)
    {
        // Reemplazar el archivo solo después de escribir el contenido completo.
        var temporal = _ruta + ".tmp";
        File.WriteAllText(temporal, JsonSerializer.Serialize(datos, _opciones));
        File.Move(temporal, _ruta, overwrite: true);
    }

    public List<Mision> ObtenerTodas()
    {
        lock (_candado) return Leer();
    }

    public Mision? ObtenerPorId(int id)
    {
        lock (_candado) return Leer().FirstOrDefault(m => m.Id == id);
    }

    public Mision Agregar(Mision nueva)
    {
        lock (_candado)
        {
            var datos = Leer();
            nueva.Id = datos.Count == 0 ? 1 : checked(datos.Max(m => m.Id) + 1);
            datos.Add(nueva);
            Guardar(datos);
            return nueva;
        }
    }

    public bool Actualizar(int id, Mision actualizada)
    {
        lock (_candado)
        {
            var datos = Leer();
            var indice = datos.FindIndex(m => m.Id == id);
            if (indice < 0) return false;
            actualizada.Id = id;
            datos[indice] = actualizada;
            Guardar(datos);
            return true;
        }
    }

    public bool Eliminar(int id)
    {
        lock (_candado)
        {
            var datos = Leer();
            if (datos.RemoveAll(m => m.Id == id) == 0) return false;
            Guardar(datos);
            return true;
        }
    }
}
