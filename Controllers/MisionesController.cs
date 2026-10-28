using DSW_3P_PR01.Models;
using DSW_3P_PR01.Services;
using Microsoft.AspNetCore.Mvc;

namespace DSW_3P_PR01.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MisionesController(IMisionService service) : ControllerBase
{
    [HttpGet]
    public ActionResult<List<Mision>> ObtenerTodas() => Ok(service.ObtenerTodas());

    [HttpGet("{id:int}")]
    public ActionResult<Mision> ObtenerPorId(int id)
    {
        var mision = service.ObtenerPorId(id);
        return mision is null ? NoEncontrada(id) : Ok(mision);
    }

    [HttpPost]
    public ActionResult<Mision> Agregar(Mision nueva)
    {
        try
        {
            var creada = service.Agregar(nueva);
            return CreatedAtAction(nameof(ObtenerPorId), new { id = creada.Id }, creada);
        }
        catch (ArgumentException ex) { return DatosInvalidos(ex.Message); }
    }

    [HttpPut("{id:int}")]
    public IActionResult Actualizar(int id, Mision actualizada)
    {
        try { return service.Actualizar(id, actualizada) ? NoContent() : NoEncontrada(id); }
        catch (ArgumentException ex) { return DatosInvalidos(ex.Message); }
    }

    [HttpDelete("{id:int}")]
    public IActionResult Eliminar(int id) =>
        service.Eliminar(id) ? NoContent() : NoEncontrada(id);

    private ObjectResult NoEncontrada(int id) =>
        Problem(statusCode: 404, title: "Misión no encontrada", detail: $"No existe la misión con Id {id}.");

    private ObjectResult DatosInvalidos(string mensaje) =>
        Problem(statusCode: 400, title: "Datos inválidos", detail: mensaje);
}
