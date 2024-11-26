using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeguridadWeb.EntidadesDeNegocio
{
    public class Categoria
    {
        [Key]
        public int Id { get; set; }
        [Display(Name = "Nombre")]
        [Required(ErrorMessage = "El Nombre de Categoria es requerido")]
        [MaxLength(255, ErrorMessage = "El maximo de caracteres es 255")]
        public string Nombre {  get; set; }
        [NotMapped]
        public int Top_Aux { get; set; }
        public List<Producto> Producto { get; set; }
    }
}
