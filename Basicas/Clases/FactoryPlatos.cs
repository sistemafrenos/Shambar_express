﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HK
{
    public class Receta : PlatosIngrediente
    {
        string platoDescripcion;

        public string PlatoDescripcion
        {
            get { return platoDescripcion; }
            set { platoDescripcion = value; }
        }
        string platoGrupo;

        public string PlatoGrupo
        {
            get { return platoGrupo; }
            set { platoGrupo = value; }
        }
    }
}
namespace HK.Clases
{
    public class FactoryPlatos
    {
        public static List<Plato> getItems(string texto)
        {
            using (FeriaEntities db= new FeriaEntities())
            {
                var  mplatos = (from x in db.Platos
                                      orderby x.Codigo
                                where (x.Codigo.Contains(texto) || x.Descripcion.Contains(texto) || x.Grupo.Contains(texto) || texto.Length == 0)  
                                select x).ToList();
                return mplatos;
            }
        }
        public static List<Plato> getItems(FeriaEntities db,string texto)
        {
                var mplatos = (from x in db.Platos
                               orderby x.Codigo
                               where (x.Codigo.Contains(texto) || x.Descripcion.Contains(texto) || x.Grupo.Contains(texto) || texto.Length == 0)  
                               select x).ToList();
                return mplatos;
        }

        public static List<Plato> getItems(string grupo,string texto)
        {
            using (FeriaEntities db= new FeriaEntities())
            {
                var mplatos = (from x in db.Platos
                               orderby x.Codigo
                               where (x.Codigo.Contains(texto) || x.Descripcion.Contains(texto) || texto.Length == 0) && x.Grupo == grupo
                               select x).ToList();
                return mplatos;
            }
        }
        public static Plato Item(string id)
        {
            using (FeriaEntities db= new FeriaEntities())
            {
                var item = (from x in db.Platos
                            where (x.IdPlato == id)
                            select x).FirstOrDefault();
                return item;
            }
        }
        public static Plato Item(FeriaEntities db,string id)
        {
                var item = (from x in db.Platos
                            where (x.IdPlato == id)
                            select x).FirstOrDefault();
                return item;
        }

        public static Plato ItemxCodgo(string codigo)
        {
            using (FeriaEntities db= new FeriaEntities())
            {
                var item = (from x in db.Platos
                            where x.Codigo == codigo
                            select x).FirstOrDefault();
                return item;
            }
        }
        public static Plato ItemxCodgo(FeriaEntities db,string codigo)
        {
                var item = (from x in db.Platos
                            where x.Codigo == codigo
                            select x).FirstOrDefault();
                return item;
        }


        public static object[] getArrayGrupos()
        {
            using (FeriaEntities db= new FeriaEntities())
            {
                var mgrupos = (from x in db.Platos
                                        orderby x.Grupo
                                        select x.Grupo).Distinct();
                return mgrupos.ToArray();
            }
        }
        public static List<string> getListGrupos()
        {
            using (FeriaEntities db= new FeriaEntities())
            {
                var mgrupos = (from x in db.Platos
                               orderby x.Grupo
                               select x.Grupo).Distinct().Take(10);
                return mgrupos.ToList();
            }
        }
        public static List<string> getListContornos()
        {
            using (FeriaEntities db= new FeriaEntities())
            {
                var mgrupos = (from x in db.PlatosContornos
                               orderby x.Contorno
                               select x.Contorno).Distinct();
                return mgrupos.ToList();
            }
        }
        public static object[] getArrayEnviarComanda()
        {
            using (FeriaEntities db= new FeriaEntities())
            {
                var mgrupos = (from x in db.Platos
                               orderby x.EnviarComanda
                               where x.EnviarComanda!=null
                               select x.EnviarComanda).Distinct();
                return mgrupos.ToArray();
            }
        }
        public static object[] getArrayComentarios()
        {
            using (FeriaEntities db= new FeriaEntities())
            {
                var mgrupos = (from x in db.PlatosComentarios
                               orderby x.Comentario
                               where x.Comentario != null
                               select x.Comentario).Distinct();
                return mgrupos.ToArray();
            }
        }
        public static object[] getArrayComentarios(Plato plato)
        {
            using (FeriaEntities db= new FeriaEntities())
            {
                var mgrupos = (from x in db.PlatosComentarios
                               orderby x.Comentario
                               where x.Comentario != null && plato.IdPlato == x.IdPlato
                               select x.Comentario).Distinct();
                return mgrupos.ToArray();
            }
        }
        public static object[] getArrayContornos(Plato plato)
        {
            using (FeriaEntities db= new FeriaEntities())
            {
                var mgrupos = (from x in db.PlatosContornos
                               orderby x.Contorno
                               where x.Contorno != null && plato.IdPlato == x.IdPlato
                               select x.Contorno).Distinct();
                return mgrupos.ToArray();
            }
        }
        public static List<PlatosIngrediente> getIngredientes(string IdPlato)
        {
            using (FeriaEntities db= new FeriaEntities())
            {
                var mgrupos = from x in db.PlatosIngredientes
                              orderby x.Ingrediente
                              where x.Ingrediente != null && x.IdPlato == IdPlato
                              select x;
                return mgrupos.ToList();
            }
        }

        public static bool VerificarExistencia(Plato plato)
        {
            using (FeriaEntities db= new FeriaEntities())
            {
               var q =  (from x in db.Platos
                 orderby x.Codigo
                 where (x.Codigo== plato.Codigo || x.Descripcion== plato.Descripcion)
                 select x).FirstOrDefault();
               return q != null;
            }
        }
    }
}
