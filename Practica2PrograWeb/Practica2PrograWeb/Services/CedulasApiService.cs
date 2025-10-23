using System.Text.Json;

public sealed class CedulasApiService
{
    private readonly HttpClient _http;
    public CedulasApiService(HttpClient http) => _http = http;

    public async Task<CedulaLookupResult> LookupAsync(string query, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(query))
            return CedulaLookupResult.Fail("La consulta está vacía.");

        var url = Uri.EscapeDataString(query.Trim());
        using var res = await _http.GetAsync(url, ct);
        if (!res.IsSuccessStatusCode)
            return CedulaLookupResult.Fail($"Error del API ({(int)res.StatusCode}).");

        var json = await res.Content.ReadAsStringAsync(ct);
        if (string.IsNullOrWhiteSpace(json))
            return CedulaLookupResult.Fail("El API devolvió vacío.");

        try
        {
            using var doc = JsonDocument.Parse(json);
            var root = doc.RootElement;

            var item = root.ValueKind == JsonValueKind.Array
                ? (root.GetArrayLength() > 0 ? root[0] : default)
                : root;

            string? nombre = TryGetString(item, "nombre") ?? TryGetString(item, "Nombre") ?? TryGetString(item, "name");
            string? cedula = TryGetString(item, "cedula") ?? TryGetString(item, "Cedula") ?? TryGetString(item, "id") ?? query;

            return new CedulaLookupResult { Ok = true, Cedula = cedula, Nombre = nombre, RawJson = json };
        }
        catch
        {
            return new CedulaLookupResult { Ok = true, Cedula = query, RawJson = json };
        }
    }

    private static string? TryGetString(JsonElement el, string name)
    {
        if (el.ValueKind != JsonValueKind.Object) return null;
        if (el.TryGetProperty(name, out var prop) && prop.ValueKind == JsonValueKind.String)
            return prop.GetString();
        return null;
    }
}

public sealed class CedulaLookupResult
{
    public bool Ok { get; set; }
    public string? Cedula { get; set; }
    public string? Nombre { get; set; }
    public string? RawJson { get; set; }
    public string? Error { get; set; }

    public static CedulaLookupResult Fail(string error) => new() { Ok = false, Error = error };
}
