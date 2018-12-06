using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Objects.DataClasses;
using HK;

namespace HK
{
    public partial class Compra : EntityObject
    {
        public void Totalizar()
        {
            this.MontoTotal = (double)decimal.Round((decimal)this.ComprasIngredientes.Sum(x => x.Cantidad * x.CostoIva));
            this.MontoExento = (double)decimal.Round((decimal)this.ComprasIngredientes.Where(x => x.TasaIva.GetValueOrDefault(0) == 0).Sum(x => x.Cantidad * x.CostoIva));
            this.MontoGravable = (double)decimal.Round((decimal)this.ComprasIngredientes.Where(x => x.TasaIva.GetValueOrDefault(0) > 0).Sum(x => x.Cantidad * x.Costo));
            this.MontoIva = (double)decimal.Round((decimal)this.MontoTotal.GetValueOrDefault(0) - (decimal)this.MontoGravable.GetValueOrDefault(0) - (decimal)this.MontoExento.GetValueOrDefault(0), 2);
        }
    }
}
namespace HK.Clases
{
    public class FactoryCompras
    {
        public static Compra Item(string id)
        {
            using (FeriaEntities db= new FeriaEntities())
            {
                var item = (from x in db.Compras
                            where (x.IdCompra == id)
                            select x).FirstOrDefault();
                return item;
            }
        }
        public static Compra Item(FeriaEntities db, string id)
        {
            var item = (from x in db.Compras
                        where (x.IdCompra == id)
                        select x).FirstOrDefault();
            return item;
        }
    
        public static void Validar(Compra registro)
        {
            //registro.Totalizar();
            //if(registro.MontoTotal==0)
            //{
            //    throw new Exception("El Monto no puede ser cero");
            //}
        }

        public static void Inventario(Compra factura)
        {
            using (FeriaEntities db = new FeriaEntities())
            {
                foreach (ComprasIngrediente item in factura.ComprasIngredientes)
                    {
                    IngredientesInventario q = (from p in db.IngredientesInventarios
                            where p.Fecha == factura.FechaInventario && p.IdIngrediente == item.IdIngrediente
                            select p).FirstOrDefault();
                            if(q==null)
                            {
                                IngredientesInventario ant = (from p in db.IngredientesInventarios
                                                              where p.Fecha < factura.FechaInventario && p.IdIngrediente == item.IdIngrediente
                                                            select p).FirstOrDefault();

                                q = new IngredientesInventario();
                                q.IdIngrediente = item.IdIngrediente;
                                q.Fecha = factura.FechaInventario;
                                q.Inicio = ant==null?0: ant.InventarioFisico;
                                q.Entradas = 0;
                                q.Salidas = 0;
                                q.Final = 0;
                                q.InventarioFisico = 0;
                                q.Ajuste = 0;
                            }
                            q.Entradas+= item.Cantidad;
                            q.Final =  q.Entradas+q.Inicio-q.Salidas;
                            q.InventarioFisico = q.Final;
                            q.Ajuste = 0;
                            if (q.IdIngredienteInventario == null)
                            {
                                q.IdIngredienteInventario = FactoryContadores.GetMax("IdIngredienteInventario");
                                db.IngredientesInventarios.AddObject(q);
                            }

                            var ingr = (from prod in db.Ingredientes
                                        where prod.IdIngrediente == item.IdIngrediente
                                        select prod).FirstOrDefault();
                            if (ingr != null)
                            {
                                ingr.Existencia = q.Final;
                                ingr.Costo = item.CostoNeto;
                            }
                            db.SaveChanges();
                    }
                factura.ActualizadoInventario = true;
                db.SaveChanges();
                }
            }
        public static void InventarioDevolver(Compra factura)
        {
            using (FeriaEntities db = new FeriaEntities())
            {
                foreach (ComprasIngrediente item in factura.ComprasIngredientes)
                {
                    IngredientesInventario q = (from p in db.IngredientesInventarios
                                                where p.Fecha == factura.Fecha && p.IdIngrediente == item.IdIngrediente
                                                select p).FirstOrDefault();
                    if (q != null)
                    {
                        q.Entradas -= item.Cantidad;
                        q.Final = q.Entradas + q.Inicio - q.Salidas;
                        q.InventarioFisico = q.Final;
                        q.Ajuste = 0;
                    }
                }
                db.SaveChanges();
            }
        }
   }
}

