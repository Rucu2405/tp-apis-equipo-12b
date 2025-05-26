using dominio;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace TPAPIs_equipo_12B.Models
{
    public class ArticuloDto
    {

        [Required(ErrorMessage = "El campo Codigo es obligatorio")]
        public string Codigo { get; set; }

        [Required(ErrorMessage = "El campo Nombre es obligatorio")]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "El campo Descripcion es obligatorio")]
        public string Descripcion { get; set; }

        [Required(ErrorMessage = "El campo Marca es obligatorio")]
        public int IdMarca { get; set; }

        [Required(ErrorMessage = "El campo Categoria es obligatorio")]
        public int IdCategoria { get; set; }

        [Required(ErrorMessage = "El campo Precio es obligatorio")]
        public float Precio { get; set; }
    }
}