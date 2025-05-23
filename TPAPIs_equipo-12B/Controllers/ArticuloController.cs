using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Results;
using dominio;
using negocio;
using TPAPIs_equipo_12B.Models;

namespace TPAPIs_equipo_12B.Controllers
{
    public class ArticuloController : ApiController
    {

        private ArticuloNegocio negocio = new ArticuloNegocio();

        //Obtener listado de artículos...
        // GET: api/Articulo
        public IEnumerable<Articulo> Get()
        {
            return negocio.ListarArticulos();
        }

        //Buscar artículos por ID...
        // GET: api/Articulo/5
        public Articulo Get(int id)
        {
            List<Articulo> listado = negocio.ListarArticulos();
            return listado.Find(x => x.Id == id);
        }

        //Agregar artículo con mensaje de éxito...
        // POST: api/Articulo
        public HttpResponseMessage Post([FromBody] ArticuloDto dto)
        {
            Articulo nuevo = new Articulo();
            nuevo.Codigo = dto.Codigo;
            nuevo.Nombre = dto.Nombre;
            nuevo.Descripcion = dto.Descripcion;
            nuevo.Marca = new Marca { Id = dto.IdMarca };
            nuevo.Categoria = new Categoria { Id = dto.IdCategoria };
            nuevo.Precio = dto.Precio;
            negocio.agregar(nuevo);
            return Request.CreateResponse(HttpStatusCode.OK, "¡Artículo agregado correctamente!");

            //acá iría el agregar imágenes
        }

        // PUT: api/Articulo/5
        public void Put(int id, [FromBody] Articulo art)
        {
        }

        //Eliminación física 
        // DELETE: api/Articulo/5
        public HttpResponseMessage Delete(int id)
        {
            negocio.Eliminar(id);
            return Request.CreateResponse(HttpStatusCode.OK, "Artículo eliminado con éxito.");

            //acá iría el eliminar imágenes
        }
    }
}
