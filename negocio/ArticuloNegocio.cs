using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;
using dominio;

namespace negocio
{
    public class ArticuloNegocio
    {
        private ListadoArticuloNegocio listadoArticuloNegocio = new ListadoArticuloNegocio();

        public List<Articulo> ListarArticulos()
        {
            return listadoArticuloNegocio.Listar();
        }

        //Agregar articulo
        public void agregar(Articulo nuevo)
        {
            AccesoDatos datos = new AccesoDatos();

            try
            {

                datos.setearConsulta("Insert into ARTICULOS (Codigo, Nombre, Precio, Descripcion, IdMarca, IdCategoria)values(@Codigo, @Nombre, @Precio, @Descripcion, @IdMarca, @IdCategoria)");
                datos.setearParametro("@Codigo", nuevo.Codigo);
                datos.setearParametro("@Nombre", nuevo.Nombre);
                datos.setearParametro("@Precio", nuevo.Precio);
                datos.setearParametro("@Descripcion", nuevo.Descripcion);
                datos.setearParametro("@IdMarca", nuevo.Marca.Id);
                datos.setearParametro("@IdCategoria", nuevo.Categoria.Id);
                datos.ejecutarAccion();
            }
            catch (Exception ex)
            {

                throw ex;
            }
            finally
            {
                datos.cerrarConexion();
            }

        }

        //Obtener el ID del último artículo creado
        public int obtenerUltimoArticuloCreado()
        {
            AccesoDatos datos = new AccesoDatos();
            try
            {
                datos.setearConsulta("select MAX(Id) from ARTICULOS");
                datos.ejecutarLectura();
                if (datos.Lector.Read())
                {
                    return datos.Lector.GetInt32(0);
                }
                return -1;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                datos.cerrarConexion();
            }
        }

        public void crearImagenes(string urlImagen, int idArticulo)
        {
            AccesoDatos datos = new AccesoDatos();
            try
            {
                datos.setearConsulta("Insert into IMAGENES (IdArticulo, ImagenUrl) values (@IdArticulo, @ImagenUrl)");
                datos.setearParametro("@IdArticulo", idArticulo);
                datos.setearParametro("@ImagenUrl", urlImagen);
                datos.ejecutarAccion();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                datos.cerrarConexion();
            }
        }


        //Traer las imagenes del articulo seleccionado
        public List<Imagen> ObtenerImagenesPorIdArticulo(int idArticulo)
        {
            List<Imagen> listaImagenes = new List<Imagen>();
            AccesoDatos datos = new AccesoDatos();
            try
            {
                datos.setearConsulta("select Id, ImagenUrl from IMAGENES where IdArticulo = @IdArticulo");
                datos.setearParametro("@IdArticulo", idArticulo);
                datos.ejecutarLectura();

                while (datos.Lector.Read())
                {
                    Imagen imagen = new Imagen();
                    imagen.Id = (int)datos.Lector["Id"];
                    imagen.ImagenUrl = (string)datos.Lector["ImagenUrl"];
                    listaImagenes.Add(imagen);
                }

                return listaImagenes;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                datos.cerrarConexion();
            }
        }

        //Modificar articulo
        public void Modificar(Articulo mod)
        {
            AccesoDatos datos = new AccesoDatos();

            try
            {
                datos.setearConsulta("Update ARTICULOS set Codigo = '" + mod.Codigo + "' , Nombre = '" + mod.Nombre + "', Precio = '" + mod.Precio + "' , Descripcion = '" + mod.Descripcion + "' , IdMarca = '" + mod.Marca.Id + "', IdCategoria = '" + mod.Categoria.Id + "' Where Id = '" + mod.Id + "'");
                datos.ejecutarAccion();
            }
            catch (Exception ex)
            {

                throw ex;
            }
            finally
            {
                datos.cerrarConexion();
            }

        }



        public void EliminarImagenesArticulo(int id)
        {
            AccesoDatos datos = new AccesoDatos();
            try
            {
                //Elimino primero la imagen asociada a ese regstro de articulo
                datos.setearConsulta("delete from IMAGENES where IdArticulo = '" + id + "'");
                datos.ejecutarAccion();

            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                datos.cerrarConexion();
            }
        }

        //Eliminar artículo (eliminación física)

        public void Eliminar(int id)
        {
            AccesoDatos datos = new AccesoDatos();
            try
            {
                //Elimino el articulo con el id asociado
                datos.setearConsulta("Delete From ARTICULOS Where Id = '" + id + "'");
                datos.ejecutarAccion();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                datos.cerrarConexion();
            }
        }

        //Filtro rapido del buscador
        public List<Articulo> Filtrar(string buscar)
        {
            List<Articulo> lista = new List<Articulo>();
            AccesoDatos datos = new AccesoDatos();
            try
            {
                string consulta = "select A.Id,A.Codigo,A.Nombre,A.Descripcion,M.Descripcion as 'Marca',C.Descripcion as 'Categoria',A.Precio, A.IdMarca, A.IdCategoria  from ARTICULOS as A ,MARCAS as M ,CATEGORIAS as C where A.IdMarca=M.Id and A.IdCategoria=C.Id and ( A.Codigo like '%" + buscar + "%' or A.Nombre like '%" + buscar + "%' or  A.Descripcion like '%" + buscar + "%' or M.Descripcion like '%" + buscar + "%' or C.Descripcion like '%" + buscar + "%' )";

                datos.setearConsulta(consulta);

                datos.ejecutarLectura();

                while (datos.Lector.Read())
                {

                    lista.Add(ArticuloFiltrado(datos));
                }

                return lista;
            }
            catch (Exception ex)
            {

                throw ex;
            }
            finally
            {
                datos.cerrarConexion();
            }
        }

        public List<Articulo> Filtrar(string tabla, string criterio)
        {
            List<Articulo> lista = new List<Articulo>();
            AccesoDatos datos = new AccesoDatos();
            try
            {
                string consulta = "select A.Id,A.Codigo,A.Nombre,A.Descripcion,M.Descripcion as 'Marca',C.Descripcion as 'Categoria',A.Precio, A.IdMarca, A.IdCategoria  from ARTICULOS as A ,MARCAS as M ,CATEGORIAS as C where A.IdMarca=M.Id and A.IdCategoria=C.Id and ";

                switch (tabla)
                {
                    case "Marca":
                        consulta += "M.Descripcion ='" + criterio + "'";
                        break;
                    case "Categoria":
                        consulta += "C.Descripcion ='" + criterio + "'";
                        break;

                }


                datos.setearConsulta(consulta);

                datos.ejecutarLectura();

                while (datos.Lector.Read())
                {
                    lista.Add(ArticuloFiltrado(datos));
                }
                return lista;
            }
            catch (Exception ex)
            {

                throw ex;
            }
            finally
            {
                datos.cerrarConexion();
            }

        }



        private Articulo ArticuloFiltrado(AccesoDatos datos)
        {
            Articulo aux = new Articulo();

            aux.Id = (int)datos.Lector["Id"];
            aux.Codigo = (string)datos.Lector["Codigo"];
            aux.Nombre = (string)datos.Lector["Nombre"];
            aux.Descripcion = (string)datos.Lector["Descripcion"];

            aux.Marca = new Marca();
            aux.Marca.Id = (int)datos.Lector["IdMarca"];
            aux.Marca.Descripcion = (string)datos.Lector["Marca"];

            aux.Categoria = new Categoria();
            aux.Categoria.Id = (int)datos.Lector["IdCategoria"];
            aux.Categoria.Descripcion = (string)datos.Lector["Categoria"];

            aux.Precio = (float)(decimal)datos.Lector["Precio"];

            return aux;
        }

    }
}

