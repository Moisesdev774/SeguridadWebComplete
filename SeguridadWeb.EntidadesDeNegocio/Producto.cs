using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.IO;
using System.Threading.Tasks;

namespace SeguridadWeb.EntidadesDeNegocio
{
    public class Producto
    {
        [Key]
        public int Id { get; set; }
        public int IdCategoria { get; set; }

        [Display(Name = "Nombre")]
        [Required(ErrorMessage = "El Nombre es requerido")]
        [MaxLength(255, ErrorMessage = "El máximo de caracteres es 255")]
        public string Nombre { get; set; }

        [Display(Name = "Precio")]
        [Required(ErrorMessage = "El precio es requerido")]
        public decimal Precio { get; set; }

        [Display(Name = "Cantidad")]
        [Required(ErrorMessage = "La cantidad es requerida")]
        public int Cantidad { get; set; }

        public string Descripcion { get; set; }
        public byte[] FotoProducto { get; set; }
        public byte Estatus { get; set; }
        public int Top_Aux { get; set; } // Usado para paginación u otros propósitos auxiliares.

        [NotMapped]
        public IFormFile FotoProductoArchivo { get; set; } // Campo auxiliar para manejar archivos en la vista.

        public Categoria Categoria { get; set; } // Relación con la tabla Categoria.
    }

    public interface IFormFile
    {
        int Length { get; }

        Task CopyToAsync(MemoryStream ms);
    }
}
