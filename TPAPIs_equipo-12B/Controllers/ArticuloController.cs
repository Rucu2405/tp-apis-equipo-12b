﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Results;
using dominio;
using Microsoft.Ajax.Utilities;
using negocio;
using TPAPIs_equipo_12B.Models;

namespace TPAPIs_equipo_12B.Controllers
{
    public class ArticuloController : ApiController
    {

        private ArticuloNegocio negocio = new ArticuloNegocio();
        private CategoriaNegocio categoriaNegocio = new CategoriaNegocio();
        private MarcaNegocio marcaNegocio = new MarcaNegocio();

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
                return Request.CreateResponse(HttpStatusCode.InternalServerError, "Ocurrió un error inesperado: " + ex.Message);
            }

        }

        //Buscar articulos por criterios de busqueda
        // GET: api/Articulo/Buscar?nombre=S23
        [HttpGet]
        [Route("api/Articulo/Buscar")]
        public HttpResponseMessage Buscar([FromUri] string nombre = null, [FromUri] string codigo = null, [FromUri] string descripcion = null, [FromUri] string marca = null, [FromUri] string categoria = null)
        {
            List<Articulo> resultados = new List<Articulo>();
            try
            {
                if (string.IsNullOrWhiteSpace(nombre) && string.IsNullOrWhiteSpace(codigo) && string.IsNullOrWhiteSpace(descripcion) && string.IsNullOrWhiteSpace(marca) && string.IsNullOrWhiteSpace(categoria))
                {
                    resultados = negocio.ListarArticulos();
                }
                else
                {
                    string criterioBusqueda = string.Empty;

                    if (!string.IsNullOrWhiteSpace(nombre))
                    {
                        criterioBusqueda += nombre;
                    }

                    if (!string.IsNullOrWhiteSpace(codigo))
                    {
                        if (!string.IsNullOrEmpty(criterioBusqueda))
                        {
                            criterioBusqueda += " ";
                        }
                        criterioBusqueda += codigo;
                    }

                    if (!string.IsNullOrWhiteSpace(descripcion))
                    {
                        if (!string.IsNullOrEmpty(criterioBusqueda))
                        {
                            criterioBusqueda += " ";
                        }
                        criterioBusqueda += descripcion;
                    }

                    if (!string.IsNullOrWhiteSpace(marca))
                    {
                        if (!string.IsNullOrEmpty(criterioBusqueda))
                        {
                            criterioBusqueda += " ";
                        }
                        criterioBusqueda += marca;
                    }

                    if (!string.IsNullOrWhiteSpace(categoria))
                    {
                        if (!string.IsNullOrEmpty(criterioBusqueda))
                        {
                            criterioBusqueda += " ";
                        }
                        criterioBusqueda += categoria;
                    }

                    // Llama a Filtrar del filtro rapido
                    resultados = negocio.Filtrar(criterioBusqueda);

                }

                if (resultados == null || !resultados.Any())
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound, "No se encontraron artículos que coincidan con los criterios de búsqueda.");
                }


                return Request.CreateResponse(HttpStatusCode.OK, resultados);


            }
            catch (Exception ex)
            {

                return Request.CreateResponse(HttpStatusCode.InternalServerError, "Ocurrió un error inesperado: " + ex.Message);
            }
        }

        //Agregar artículo con mensaje de éxito...
        // POST: api/Articulo
        public HttpResponseMessage Post([FromBody] ArticuloDto dto)
        {
            try
            {
                //Valida que el dto sea correcto
                if (dto == null)
                {

                    return Request.CreateResponse(HttpStatusCode.BadRequest, "Asegúrese de que el formato  sea el correcto.");
                }
                if (!ModelState.IsValid)
                {
                    foreach (var state in ModelState)
                    {
                        foreach (var error in state.Value.Errors)
                        {

                            //Capturamos el nombre del campo que tira error
                            string campoInvalido = state.Key.Contains(".") ? state.Key.Split('.').Last() : state.Key;
                            if (campoInvalido == "dto")
                            {
                                return Request.CreateResponse(HttpStatusCode.BadRequest, "Debe ingresar un formato válido");
                            }

                            return Request.CreateResponse(HttpStatusCode.BadRequest, "Debe ingresar un formato válido en el campo: " + campoInvalido);
                        }

                    }

                }

                //Valida que el ID de la marca y categoria exista
                Marca marcaExistente = marcaNegocio.ListarMarca().Find(x => x.Id == dto.IdMarca);
                Categoria categoriaExistente = categoriaNegocio.ListarCategoria().Find(x => x.Id == dto.IdCategoria);


                if (marcaExistente == null)
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound, "La marca Nro: " + dto.IdMarca + " para agregar, no existe");
                }

                if (categoriaExistente == null)
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound, "La categoría Nro: " + dto.IdCategoria + " para agregar, no existe");
                }

                //Validamos como campos obligatorios Codigo,Nombre y Precio:
                if (string.IsNullOrWhiteSpace(dto.Codigo))
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, "El código del artículo es obligatorio.");
                }
                if (string.IsNullOrWhiteSpace(dto.Nombre))
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, "El nombre del artículo es obligatorio.");
                }
                if (dto.Precio <= 0)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, "El precio del artículo debe ser mayor a 0.");
                }

                Articulo nuevo = new Articulo();
                nuevo.Codigo = dto.Codigo;
                nuevo.Nombre = dto.Nombre;
                nuevo.Descripcion = dto.Descripcion;
                nuevo.Marca = new Marca { Id = dto.IdMarca };
                nuevo.Categoria = new Categoria { Id = dto.IdCategoria };
                nuevo.Precio = dto.Precio;
                negocio.agregar(nuevo);
                return Request.CreateResponse(HttpStatusCode.Created, "¡Artículo agregado correctamente!");

            }
            catch (Exception ex)
            {

                return Request.CreateResponse(HttpStatusCode.InternalServerError, "Ocurrió un error inesperado al agregar el artículo: " + ex.Message);
            }


        }

        /// Para agregar imagenes a los articulos
        public HttpResponseMessage AgregarImagenes(int id, [FromBody] List<ImagenDto> dto)
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



                //Valida que el dto sea correcto
                if (dto == null || !dto.Any())
                {

                    return Request.CreateResponse(HttpStatusCode.BadRequest, "No puede enviar null, se espera una lista de URL de imágenes. Verifique el formato.");
                }


                if (!ModelState.IsValid)
                {
                    foreach (var state in ModelState)
                    {
                        foreach (var error in state.Value.Errors)
                        {
                            //Capturamos el nombre del campo que tira error
                            string campoInvalido = state.Key.Contains(".") ? state.Key.Split('.').Last() : state.Key;


                            return Request.CreateResponse(HttpStatusCode.BadRequest, "Debe ingresar un formato válido en el campo: " + campoInvalido);
                        }

                    }

                }


                List<Imagen> listaImagenes = dto.Select(i => new Imagen
                {
                    ImagenUrl = i.ImagenUrl
                }).ToList();

                negocio.AgregarImagenes(id, listaImagenes);
                return Request.CreateResponse(HttpStatusCode.OK, "¡Imagenes agregadas correctamente al articulo Nro: " + id + "! ");

            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, "Error inesperado: " + ex.Message);
            }

        }



        //Modifica artículo 
        // PUT: api/Articulo/id
        public HttpResponseMessage Put(int id, [FromBody] ArticuloDto dto)
        {
            try
            {
                /* //Valida que el dto sea correcto
                 if (dto == null)
                 {
                     if (!ModelState.IsValid)
                     {
                         foreach (var state in ModelState)
                         {
                             foreach (var error in state.Value.Errors)
                             {
                                 ///return Request.CreateResponse(HttpStatusCode.BadRequest, "aaaaERRORRRR----->." + state.Value.Errors);
                                 ///return Request.CreateResponse(HttpStatusCode.BadRequest, "aaaaERRORRRR22----->." + state.Key);
                                 //Capturamos el nombre del campo que tira error
                                 string campoInvalido = state.Key.Contains(".") ? state.Key.Split('.').Last() : state.Key;
                                 return Request.CreateResponse(HttpStatusCode.BadRequest, "Debe ingresar un formato válido en el campo: " + campoInvalido);
                             }

                         }

                     }

                     return Request.CreateResponse(HttpStatusCode.BadRequest, "Datos de artículo inválidos. Asegúrese de que el formato  sea el correcto.");
                 }*/
                if (dto == null)
                {

                    return Request.CreateResponse(HttpStatusCode.BadRequest, "Asegúrese de que el formato  sea el correcto.");
                }
                if (!ModelState.IsValid)
                {
                    foreach (var state in ModelState)
                    {
                        foreach (var error in state.Value.Errors)
                        {

                            //Capturamos el nombre del campo que tira error
                            string campoInvalido = state.Key.Contains(".") ? state.Key.Split('.').Last() : state.Key;
                            if (campoInvalido == "dto")
                            {
                                return Request.CreateResponse(HttpStatusCode.BadRequest, "Debe ingresar un formato válido");
                            }

                            return Request.CreateResponse(HttpStatusCode.BadRequest, "Debe ingresar un formato válido en el campo: " + campoInvalido);
                        }

                    }

                }



                //Valida que el ID del articulo, marca y categoria exista
                Articulo articuloExistente = negocio.buscarArticulo(id);
                Marca marcaExistente = marcaNegocio.ListarMarca().Find(x => x.Id == dto.IdMarca);
                Categoria categoriaExistente = categoriaNegocio.ListarCategoria().Find(x => x.Id == dto.IdCategoria);

                if (articuloExistente == null)
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound, "El artículo Nro: " + id + " para modificar, no existe");
                }

                if (marcaExistente == null)
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound, "La marca Nro: " + dto.IdMarca + " para modificar, no existe");
                }

                if (categoriaExistente == null)
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound, "La categoría Nro: " + dto.IdCategoria + " para modificar, no existe");
                }

                //Validamos como campos obligatorios Codigo,Nombre y Precio:
                if (string.IsNullOrWhiteSpace(dto.Codigo))
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, "El código del artículo es obligatorio.");
                }
                if (string.IsNullOrWhiteSpace(dto.Nombre))
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, "El nombre del artículo es obligatorio.");
                }
                if (dto.Precio <= 0)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, "El precio del artículo debe ser mayor a 0.");
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
            catch (Exception ex)
            {

                return Request.CreateResponse(HttpStatusCode.InternalServerError, "Ocurrió un error inesperado al modificar el artículo. " + ex.Message);
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

                //Elimina articulo
                negocio.Eliminar(id);

                return Request.CreateResponse(HttpStatusCode.OK, "Artículo eliminado con éxito.");
            }
            catch (Exception ex)
            {

                return Request.CreateResponse(HttpStatusCode.InternalServerError, "Ocurrió un error inesperado al eliminar el artículo. " + ex.Message);
            }


        }
    }
}
