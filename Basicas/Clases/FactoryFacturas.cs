using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Objects.DataClasses;

namespace HK
{
    public partial class Factura : EntityObject
    {
        public void calcularSaldo()
        {
            this.Saldo = this.MontoTotal.GetValueOrDefault(0) - (
                this.Efectivo.GetValueOrDefault(0) +
                this.Tarjeta.GetValueOrDefault(0) + this.Cheque.GetValueOrDefault(0) +
                this.Cambio.GetValueOrDefault(0) + this.ConsumoInterno.GetValueOrDefault(0) 
            );
            if (this.Saldo < 0)
            {
                this.Cambio = (double)decimal.Round((decimal)Saldo * -1, 2);
                this.Saldo = null;
            }
            else
            {
                this.Cambio = 0;
            }
        }
        private double Round(double? valor)
        {
            try
            {
                decimal myValor = (decimal)valor;
                myValor = decimal.Round(myValor, 2);
                return (double)myValor;
            }
            catch
            {
                return 0;
            }
        }
        public void Totalizar(double? tasaIva = 12)
        {
            foreach (FacturasPlato item in this.FacturasPlatos.Where(x => x.TasaIva > 0))
            {
                item.TasaIva = tasaIva;
                item.PrecioConIva = this.Round(item.Precio + (item.Precio * tasaIva / 100));
                item.Total = item.PrecioConIva * item.Cantidad;
            }
            this.MontoExento = this.FacturasPlatos.Where(x => x.TasaIva == 0).Sum(x => x.Cantidad * x.Precio);
            this.MontoGravable = this.FacturasPlatos.Where(x => x.TasaIva > 0).Sum(x => x.Cantidad * x.Precio);
            this.MontoIva = this.FacturasPlatos.Where(x => x.TasaIva > 0).Sum(x => x.Cantidad * x.Precio * tasaIva / 100);
            this.MontoTotal = this.MontoGravable.GetValueOrDefault(0) + this.MontoExento.GetValueOrDefault(0) + this.MontoIva.GetValueOrDefault(0);
            this.MontoTotal = this.MontoTotal;
            this.calcularSaldo();
        }
    }
    public partial class MesasAbierta : EntityObject
    {
        public void Totalizar()
        {
            this.MontoExento = (double)decimal.Round((decimal)this.MesasAbiertasPlatos.Where(x => x.TasaIva.GetValueOrDefault(0) == 0).Sum(x => x.Cantidad * x.PrecioConIva));
            this.MontoGravable = (double)decimal.Round((decimal)this.MesasAbiertasPlatos.Where(x => x.TasaIva.GetValueOrDefault(0) > 0).Sum(x => x.Cantidad * x.Precio));
            this.MontoIva = (double)decimal.Round((decimal)this.MontoTotal.GetValueOrDefault(0) - (decimal)this.MontoGravable.GetValueOrDefault(0) - (decimal)this.MontoExento.GetValueOrDefault(0), 2);
            this.MontoTotal = (double)decimal.Round((decimal)this.MesasAbiertasPlatos.Sum(x => x.Cantidad * x.PrecioConIva));
        }
    }
}
namespace HK.Clases
{

    public class FactoryFacturas
    {
        //public static List<Plato> getItems(string texto)
        //{
        //    using (FeriaEntities db= new FeriaEntities())
        //    {
        //        var mplatos = (from x in db.Platos
        //                       orderby x.Codigo
        //                       where (x.Codigo.Contains(texto) || x.Descripcion.Contains(texto) || x.Grupo.Contains(texto) || texto.Length == 0)
        //                       select x).ToList();
        //        return mplatos;
        //    }
        //}
        //public static List<Plato> getItems(FeriaEntities db, string texto)
        //{
        //    var mplatos = (from x in db.Platos
        //                   orderby x.Codigo
        //                   where (x.Codigo.Contains(texto) || x.Descripcion.Contains(texto) || x.Grupo.Contains(texto) || texto.Length == 0)
        //                   select x).ToList();
        //    return mplatos;
        //}

        //public static List<Plato> getItems(string grupo, string texto)
        //{
        //    using (FeriaEntities db= new FeriaEntities())
        //    {
        //        var mplatos = (from x in db.Platos
        //                       orderby x.Codigo
        //                       where (x.Codigo.Contains(texto) || x.Descripcion.Contains(texto) || texto.Length == 0) && x.Grupo == grupo
        //                       select x).ToList();
        //        return mplatos;
        //    }
        //}
        public static Factura Item(string id)
        {
            using (FeriaEntities db= new FeriaEntities())
            {
                var item = (from x in db.Facturas
                            where (x.IdFactura == id)
                            select x).FirstOrDefault();
                return item;
            }
        }
        public static Factura Item(FeriaEntities db, string id)
        {
            var item = (from x in db.Facturas
                        where (x.IdFactura == id)
                        select x).FirstOrDefault();
            return item;
        }
        public static List<Factura> getFacturasPendientes(FeriaEntities db, string texto)
        {
            var mFacturas = (from x in db.Facturas
                           orderby x.IdFactura
                           where (x.Tipo=="PENDIENTE")
                           select x).ToList();
            return mFacturas;
        }
        public static List<Factura> getNaturalesJuridicas(DateTime inicio,DateTime final)
        {
            using (FeriaEntities db = new FeriaEntities())
            {
            var mFacturas = (from x in db.Facturas
                             orderby  x.Numero 
                             where (x.Fecha>=inicio && x.Fecha<=final )
                             select x).ToList();
            return mFacturas;
           }
        }
        public static void DescontarInventario(Factura factura)
        {
            foreach (FacturasPlato plato in factura.FacturasPlatos)
            {
                List<PlatosIngrediente> ingredientes = FactoryPlatos.getIngredientes(plato.Idplato);
                foreach (PlatosIngrediente ingrediente in ingredientes)
                {
                    using (FeriaEntities db= new FeriaEntities())
                    {
                        IngredientesInventario InventarioDia = (from item in db.IngredientesInventarios
                                                                where item.Fecha == factura.Fecha && item.IdIngrediente == ingrediente.IdIngrediente                                            
                                                                select item).FirstOrDefault();
                        if (InventarioDia == null)
                        {

                            IngredientesInventario InventarioAnterior = (from item in db.IngredientesInventarios
                                                                    where item.Fecha < factura.Fecha && item.IdIngrediente == ingrediente.IdIngrediente
                                                                    select item).FirstOrDefault();
                            InventarioDia = new IngredientesInventario();
                            InventarioDia.IdIngrediente = ingrediente.IdIngrediente;
                            InventarioDia.Ingrediente = ingrediente.Ingrediente;
                            InventarioDia.Fecha = factura.Fecha;
                            InventarioDia.Ajuste = 0;
                            InventarioDia.Entradas = 0;
                            if (InventarioAnterior == null)
                            {
                                InventarioDia.Inicio = 0;
                            }
                            else
                            {
                                InventarioDia.Inicio = InventarioDia.InventarioFisico;
                            }
                        }
                        InventarioDia.Salidas = ingrediente.Cantidad * plato.Cantidad;
                        InventarioDia.Final = InventarioDia.Inicio + InventarioDia.Entradas + InventarioDia.Salidas;
                        InventarioDia.InventarioFisico = InventarioDia.Final;
                        InventarioDia.Ajuste = 0;
                        if (InventarioDia.IdIngredienteInventario==null)
                        {
                           InventarioDia.IdIngredienteInventario = FactoryContadores.GetMax("IdIngredienteInventario");
                           db.IngredientesInventarios.AddObject(InventarioDia);
                        }
                        db.SaveChanges();
                    }
                }
            }
        }

        public static void PasarFacturasLibro()
        {
            using (FeriaEntities db = new FeriaEntities())
            {
                var x = from p in db.Facturas
                        where p.Tipo == "FACTURA"
                        select p;
                foreach (var item in x)
                {
                    if (item.LibroVentas==null)
                    {
                        FactoryLibroVentas.EscribirItemFactura(item);
                        item.LibroVentas = true;
                    }
                }
                db.SaveChanges();
            }
        }
    }
}
