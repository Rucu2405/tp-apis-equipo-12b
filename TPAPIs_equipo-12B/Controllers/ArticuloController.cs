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


        /*  public  Articulo Get(int id)
          {
              Articulo articulo = new Articulo();
              articulo = negocio.buscarArticulo(id);
              articulo.Imagenes = negocio.ListarImagenes(id);

              return articulo;
          }
        */

        //Buscar artículos por ID...
        // GET: api/Articulo/5
        public HttpResponseMessage Get(int id)
        {
            try
            {
                Articulo articulo = new Articulo();
                articulo = negocio.buscarArticulo(id);
                if (articulo == null)
                {
                    //Si es null, se informa que no existe.
                    return Request.CreateResponse(HttpStatusCode.NotFound, "El articulo Nro: " + id + ", no existe");
                }
                //Si existe se retorna el articulo con sus imagenes.
                articulo.Imagenes = negocio.ListarImagenes(id);
                return Request.CreateResponse(HttpStatusCode.OK, articulo);

            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound, "El articulo Nro: " + id + ", no existe");

            }

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

        /// Para agregar imagenes a los articulos
        public HttpResponseMessage AgregarImagenes(int id, [FromBody] List<Imagen> dto)
        {

            try
            {
                Articulo articulo = new Articulo();
                articulo = negocio.buscarArticulo(id);
                if (articulo == null)
                {
                    //Si es null, se informa que no existe.
                    return Request.CreateResponse(HttpStatusCode.NotFound, "El articulo Nro: " + id + ", no existe");
                }
              
                negocio.AgregarImagenes(id, dto);
                return Request.CreateResponse(HttpStatusCode.OK, "¡Imagenes agregadas correctamente al Articulo Nro: " + id + "! ");
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, "Error inesperado");
            }

        }



        //Modifica artículo 
        // PUT: api/Articulo/id
        public HttpResponseMessage Put(int id, [FromBody] ArticuloDto dto)
        {
            try
            {
                //Valida que el ID del articulo exista
                Articulo articuloExistente = negocio.buscarArticulo(id);

                if (articuloExistente == null)
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound, "El articulo Nro: " + id + " para modificar, no existe");
                }

                Articulo nuevo = new Articulo();
                nuevo.Codigo = dto.Codigo;
                nuevo.Nombre = dto.Nombre;
                nuevo.Descripcion = dto.Descripcion;
                nuevo.Marca = new Marca { Id = dto.IdMarca };
                nuevo.Categoria = new Categoria { Id = dto.IdCategoria };
                nuevo.Precio = dto.Precio;
                nuevo.Id = id;
                negocio.Modificar(nuevo);
                return Request.CreateResponse(HttpStatusCode.OK, "¡Artículo modificado correctamente!");

            }
            catch (Exception)
            {

                return Request.CreateResponse(HttpStatusCode.InternalServerError, "Ocurrió un error inesperado al modificar el artículo.");
            }

        }

        //Eliminación física 
        // DELETE: api/Articulo/5
        public HttpResponseMessage Delete(int id)
        {
            try
            {
                //Valida que el ID del articulo exista
                Articulo articuloExistente = negocio.buscarArticulo(id);

                if (articuloExistente == null)
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound, "El articulo Nro: " + id + ", no existe");
                }

                negocio.Eliminar(id);
                return Request.CreateResponse(HttpStatusCode.OK, "Artículo eliminado con éxito.");
            }
            catch (Exception)
            {

                return Request.CreateResponse(HttpStatusCode.InternalServerError, "Ocurrió un error inesperado al eliminar el artículo.");
            }


        }
    }
}
