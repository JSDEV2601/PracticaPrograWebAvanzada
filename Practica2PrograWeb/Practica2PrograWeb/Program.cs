using System.Data;
using Microsoft.Data.SqlClient;
using System.Net.Http.Headers;
using Dapper;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();


builder.Services.AddScoped<IDbConnection>(sp =>
{
    var cs = builder.Configuration.GetConnectionString("DefaultConnection")!;
    return new SqlConnection(cs);
});


builder.Services.AddScoped<VendedoresRepository>();
builder.Services.AddScoped<VehiculosRepository>();


builder.Services.AddHttpClient<CedulasApiService>((sp, http) =>
{
    var cfg = builder.Configuration.GetSection("CedulasApi");
    var baseUrl = cfg.GetValue<string>("BaseUrl") ?? "https://apis.gometa.org/cedulas/";
    http.BaseAddress = new Uri(baseUrl);
    http.Timeout = TimeSpan.FromSeconds(12);

    var apiKey = cfg.GetValue<string>("ApiKey");
    if (!string.IsNullOrWhiteSpace(apiKey))
    {
        
        http.DefaultRequestHeaders.Add("X-API-Key", apiKey);
        
    }
});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();



public class VendedoresRepository
{
    private readonly IDbConnection _db;
    public VendedoresRepository(IDbConnection db) => _db = db;

    public async Task CrearAsync(Vendedor vendedor)
    {
        var p = new { vendedor.Cedula, vendedor.Nombre, vendedor.Correo };
        try
        {
            await _db.ExecuteAsync("dbo.sp_Vendedores_Insert", p, commandType: System.Data.CommandType.StoredProcedure);
        }
        catch (SqlException ex) when (ex.Class >= 11)
        {
            throw new InvalidOperationException(ex.Message);
        }
    }

    public async Task<IEnumerable<VendedorOptionDto>> ListarAsync()
    {
        return await _db.QueryAsync<VendedorOptionDto>(
            "dbo.sp_Vendedores_Listar",
            commandType: System.Data.CommandType.StoredProcedure);
    }
}

public class VehiculosRepository
{
    private readonly IDbConnection _db;
    public VehiculosRepository(IDbConnection db) => _db = db;

    public async Task CrearAsync(Vehiculo vehiculo)
    {
        var p = new
        {
            vehiculo.Marca,
            vehiculo.Modelo,
            vehiculo.Color,
            vehiculo.Precio,
            VendedorCedula = vehiculo.Vendedor
        };

        try
        {
            await _db.ExecuteAsync("dbo.sp_Vehiculos_Insert", p, commandType: System.Data.CommandType.StoredProcedure);
        }
        catch (SqlException ex) when (ex.Class >= 11)
        {
            throw new InvalidOperationException(ex.Message);
        }
    }

    public async Task<IEnumerable<VehiculoListadoDto>> ListarAsync()
    {
        return await _db.QueryAsync<VehiculoListadoDto>(
            "dbo.sp_Vehiculos_Listar",
            commandType: System.Data.CommandType.StoredProcedure);
    }
}
