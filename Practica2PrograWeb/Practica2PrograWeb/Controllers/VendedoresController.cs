using Microsoft.AspNetCore.Mvc;

public class VendedoresController : Controller
{
    private readonly VendedoresRepository _repo;
    public VendedoresController(VendedoresRepository repo) => _repo = repo;

    [HttpGet]
    public IActionResult Create() => View(new Vendedor());

    [HttpPost]
    public async Task<IActionResult> Create(Vendedor model)
    {
        if (!ModelState.IsValid) return View(model);

        try
        {
            await _repo.CrearAsync(model);
            TempData["Ok"] = "Vendedor registrado correctamente.";
            return RedirectToAction(nameof(Create));
        }
        catch (InvalidOperationException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            return View(model);
        }
    }
}