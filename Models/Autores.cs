using System.ComponentModel.DataAnnotations;

namespace Franshelina_AP1_P1_2.Models;

public class Autores
{
    [Key]
    public int IdAutor { get; set; }

    [Required(ErrorMessage = "Este campo es obligatorio")]
    public string Nombres { get; set; }

    [Required(ErrorMessage = "Este campo es obligatorio")]
    public string Nacionalidad { get; set; }

    [Required(ErrorMessage = "Este campo es obligatorio")]
    public DateOnly FechaNacimiento { get; set; }

    [Required(ErrorMessage = "Este campo es obligatorio")]
    public double Sueldo { get; set; }
}
