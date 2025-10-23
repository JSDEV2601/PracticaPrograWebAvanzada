using System.ComponentModel.DataAnnotations;

public class Vehiculo
{
    [Required, StringLength(50)]
    public string Marca { get; set; } = "";

    [Required, StringLength(50)]
    public string Modelo { get; set; } = "";

    [Required, StringLength(30)]
    public string Color { get; set; } = "";

    [Required, Range(0.01, 999999999)]
    public decimal Precio { get; set; }

    [Required, StringLength(20)]
    public string Vendedor { get; set; } = ""; 
}
