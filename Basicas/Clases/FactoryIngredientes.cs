using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HK
{

}
namespace HK.Clases
{

    public class FactoryIngredientes
    {
        public static List<Ingrediente> getItems(string texto)
        {
            using (FeriaEntities db= new FeriaEntities())
            {
                var mplatos = (from x in db.Ingredientes
                               orderby x.Descripcion
                               where (x.Descripcion.Contains(texto) || texto.Length == 0) && x.Activo==true
                               select x).ToList();
                return mplatos;
            }
        }
        public static List<Ingrediente> getItems(FeriaEntities db, string texto)
        {
            var mplatos = (from x in db.Ingredientes
                           orderby x.Descripcion
                           where (x.Descripcion.Contains(texto) || texto.Length == 0) && x.Activo == true
                           select x).ToList();
            return mplatos;
        }

        public static Ingrediente Item(string id)
        {
            using (FeriaEntities db= new FeriaEntities())
            {
                var item = (from x in db.Ingredientes
                            where (x.IdIngrediente == id)
                            select x).FirstOrDefault();
                return item;
            }
        }
        public static Ingrediente Item(FeriaEntities db, string id)
        {
            var item = (from x in db.Ingredientes
                        where (x.IdIngrediente == id)
                        select x).FirstOrDefault();
            return item;
        }

        public static object[] getArrayUnidadesMedida()
        {
            using (FeriaEntities db= new FeriaEntities())
            {
                var mgrupos = (from x in db.Ingredientes
                               orderby x.UnidadMedida
                               where x.UnidadMedida!=null
                               select x.UnidadMedida).Distinct();
                return mgrupos.ToArray();
            }
        }
        public static List<string> getIngredientes()
        {
            using (FeriaEntities db= new FeriaEntities())
            {
                var mgrupos = (from x in db.Ingredientes
                               orderby x.Descripcion
                               where x.Activo == true
                               select x.Descripcion).Distinct();
                return mgrupos.ToList();
            }
        }

        public static Ingrediente ItemxDescripcion(string texto)
        {
            using (FeriaEntities db= new FeriaEntities())
            {
                var item = (from x in db.Ingredientes
                            where (x.Descripcion == texto) && x.Activo == true
                            select x).FirstOrDefault();
                return item;
            }
        }

        public static object[] getArrayGrupos()
        {
            using (FeriaEntities db= new FeriaEntities())
            {
                var mgrupos = (from x in db.Ingredientes
                               orderby x.Grupo
                               select x.Grupo).Distinct();
                return mgrupos.ToArray();
            }
        }
    }
}
