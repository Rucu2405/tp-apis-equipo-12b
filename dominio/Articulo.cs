﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace dominio
{
    public class Articulo
    {
        public int Id { get; set; }
        
        public string Codigo { get; set; }

        public string Nombre { get; set; }
     
        public string Descripcion { get; set; }

        public Marca Marca { get; set; }
    
        public Categoria Categoria { get; set; }

        public List<Imagen> Imagenes { get; set; } = new List<Imagen>();

        public float Precio { get; set; }
    }
}
