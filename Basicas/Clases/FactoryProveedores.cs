﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HK.Clases
{
    public class FactoryProveedores
    {
        public static List<string> getProveedores()
        {
            using (var db = new FeriaEntities())
            {
                var q = from p in db.Proveedores
                        orderby p.RazonSocial
                        select p.RazonSocial;
                return q.ToList();
            }
        }
        public static List<Proveedore> getItems(string texto)
        {
            using (var db = new FeriaEntities())
            {
                var q = from p in db.Proveedores
                        orderby p.RazonSocial
                        where (p.CedulaRif.Contains(texto) || p.RazonSocial.Contains(texto) || texto.Length == 0)
                        select p;
                return q.ToList();
            }
        }
        public static List<Proveedore> getItems(FeriaEntities db, string texto)
        {
            var q = from p in db.Proveedores
                    orderby p.RazonSocial
                    where (p.CedulaRif.Contains(texto) || p.RazonSocial.Contains(texto) || texto.Length == 0)
                    select p;
            return q.ToList();
        }
        public static Proveedore Item(string cedularif)
        {
            using (var db = new FeriaEntities())
            {
                var q = from p in db.Proveedores
                        orderby p.RazonSocial
                        where p.CedulaRif == cedularif
                        select p;
                return q.FirstOrDefault();
            }
        }
        public static Proveedore Item(FeriaEntities db, string cedularif)
        {
            var q = from p in db.Proveedores
                    orderby p.RazonSocial
                    where p.CedulaRif == cedularif
                    select p;
            return q.FirstOrDefault();
        }
    }
}
