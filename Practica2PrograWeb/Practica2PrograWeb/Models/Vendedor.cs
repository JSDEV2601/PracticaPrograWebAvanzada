using System.ComponentModel.DataAnnotations;

public class Vendedor
{
    [Required, StringLength(20)]
    public string Cedula { get; set; } = "";

    [Required, StringLength(100)]
    public string Nombre { get; set; } = "";

    [Required, EmailAddress, StringLength(150)]
    public string Correo { get; set; } = "";
}
