using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

public class VehiculosController : Controller
{
    private readonly VehiculosRepository _vehiculos;
    private readonly VendedoresRepository _vendedores;

    public VehiculosController(VehiculosRepository vehiculos, VendedoresRepository vendedores)
    {
        _vehiculos = vehiculos;
        _vendedores = vendedores;
    }

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        await CargarVendedoresAsync();
        return View(new Vehiculo());
    }

    [HttpPost]
    public async Task<IActionResult> Create(Vehiculo model)
    {
        if (!ModelState.IsValid)
        {
            await CargarVendedoresAsync();
            return View(model);
        }

        try
        {
            await _vehiculos.CrearAsync(model);
            TempData["Ok"] = "Vehículo registrado correctamente.";
            return RedirectToAction(nameof(Create));
        }
        catch (InvalidOperationException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            await CargarVendedoresAsync();
            return View(model);
        }
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var lista = await _vehiculos.ListarAsync();
        return View(lista);
    }

    private async Task CargarVendedoresAsync()
    {
        var items = await _vendedores.ListarAsync();
        ViewBag.Vendedores = new SelectList(items, "Cedula", "Nombre");
    }
}