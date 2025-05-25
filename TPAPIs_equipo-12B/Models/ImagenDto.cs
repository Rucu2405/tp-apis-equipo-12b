using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace TPAPIs_equipo_12B.Models
{
    public class ImagenDto
    {
        [Required(ErrorMessage = "El campo 'ImagenUrl' es obligatorio.")]
        [Url(ErrorMessage = "Ingrese una URL válida.")]
        public string ImagenUrl { get; set; }

        public override string ToString()
        {
            return ImagenUrl;
        }

    }
}