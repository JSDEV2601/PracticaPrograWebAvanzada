using Microsoft.AspNetCore.Mvc;

[Route("cedulas")]
public class CedulasController : Controller
{
    private readonly CedulasApiService _svc;
    public CedulasController(CedulasApiService svc) => _svc = svc;

   
    [HttpGet("lookup")]
    public async Task<IActionResult> Lookup([FromQuery] string q, CancellationToken ct)
    {
        var result = await _svc.LookupAsync(q, ct);
        if (!result.Ok) return BadRequest(new { ok = false, error = result.Error });

        return Ok(new
        {
            ok = true,
            cedula = result.Cedula,
            nombre = result.Nombre,
            raw = result.RawJson
        });
    }
}
