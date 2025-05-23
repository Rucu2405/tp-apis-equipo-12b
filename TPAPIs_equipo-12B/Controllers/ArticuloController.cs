using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Results;
using dominio;
using negocio;

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
            return listado.Find(x=> x.Id == id);    

        }

        // POST: api/Articulo
        public void Post([FromBody]Articulo art)
        {
        }

        // PUT: api/Articulo/5
        public void Put(int id, [FromBody] Articulo art)
        {
        }

        // DELETE: api/Articulo/5
        public void Delete(int id)
        {
        }
    }
}
