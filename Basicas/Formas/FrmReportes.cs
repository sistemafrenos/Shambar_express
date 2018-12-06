using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;
using Microsoft.Reporting.WinForms;
using HK.Clases;
using System.Linq;

namespace HK.Formas
{
    public partial class FrmReportes : Form
    {
        public FrmReportes()
        {
            InitializeComponent();
            this.FormClosing += new FormClosingEventHandler(FrmReportes_FormClosing);
        }
        void FrmReportes_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.reportViewer1.LocalReport.ReleaseSandboxAppDomain();
        }
        public void ListadoPlatos(List<Plato> lista)
        {            
            reportViewer1.ProcessingMode = ProcessingMode.Local;
            this.reportViewer1.LocalReport.ReportPath = Application.StartupPath + "\\Reportes\\ListadoPlatos.rdlc";
            this.reportViewer1.LocalReport.DataSources.Add(new ReportDataSource("Platos", lista));
            this.reportViewer1.LocalReport.DataSources.Add(new ReportDataSource("Parametros",new  Parametro[] { Basicas.parametros() } ));
            this.reportViewer1.RefreshReport();
            this.ShowDialog();
        }
        public void ListadoPlatosIngredientes()
        {
            using (var db = new FeriaEntities())
            {
                var recetas = from plato in db.Platos
                        join ingredientes in db.PlatosIngredientes on plato.IdPlato equals ingredientes.IdPlato
                        select new Receta
                        {
                            PlatoGrupo = plato.Grupo,
                            PlatoDescripcion= plato.Descripcion,
                            Ingrediente = ingredientes.Ingrediente,
                            Cantidad = ingredientes.Cantidad,
                            CostoRacion = ingredientes.CostoRacion,
                            Total = ingredientes.Total
                        };
            reportViewer1.ProcessingMode = ProcessingMode.Local;
            this.reportViewer1.LocalReport.ReportPath = Application.StartupPath + "\\Reportes\\ListadoPlatosIngredientes.rdlc";
            this.reportViewer1.LocalReport.DataSources.Add(new ReportDataSource("Recetas", recetas.ToList()));
            this.reportViewer1.LocalReport.DataSources.Add(new ReportDataSource("Parametros", new Parametro[] { Basicas.parametros() }));
            this.reportViewer1.RefreshReport();
            this.ShowDialog();
            }
        }
        public void ListadoUsuarios(List<Usuario> lista)
        {
            reportViewer1.ProcessingMode = ProcessingMode.Local;
            this.reportViewer1.LocalReport.ReportPath = Application.StartupPath + "\\Reportes\\ListadoUsuarios.rdlc";
            this.reportViewer1.LocalReport.DataSources.Add(new ReportDataSource("Usuarios", lista));
            this.reportViewer1.LocalReport.DataSources.Add(new ReportDataSource("Parametros", new Parametro[] { Basicas.parametros() }));
            this.reportViewer1.RefreshReport();
            this.ShowDialog();
        }
        public void ListadoMesoneros(List<Usuario> lista)
        {
            reportViewer1.ProcessingMode = ProcessingMode.Local;
            this.reportViewer1.LocalReport.ReportPath = Application.StartupPath + "\\Reportes\\ListadoMesoneros.rdlc";
            this.reportViewer1.LocalReport.DataSources.Add(new ReportDataSource("Usuarios", lista));
            this.reportViewer1.LocalReport.DataSources.Add(new ReportDataSource("Parametros", new Parametro[] { Basicas.parametros() }));
            this.reportViewer1.RefreshReport();
            this.ShowDialog();
        }
        public void ListadoCajeros(List<Usuario> lista)
        {
            reportViewer1.ProcessingMode = ProcessingMode.Local;
            this.reportViewer1.LocalReport.ReportPath = Application.StartupPath + "\\Reportes\\ListadoCajeros.rdlc";
            this.reportViewer1.LocalReport.DataSources.Add(new ReportDataSource("Usuarios", lista));
            this.reportViewer1.LocalReport.DataSources.Add(new ReportDataSource("Parametros", new Parametro[] { Basicas.parametros() }));
            this.reportViewer1.RefreshReport();
            this.ShowDialog();
        }
        public void CierreDeCaja()
        {
            FrmFechaCajero f = new FrmFechaCajero();
            f.Text = "Cierre De Caja x Usuario";
            f.ShowDialog();
            if (f.DialogResult != System.Windows.Forms.DialogResult.OK)
                return;
            Usuario cajero = f.cajero;            
            if (cajero == null)
            {
                reportViewer1.ProcessingMode = ProcessingMode.Local;
                this.reportViewer1.LocalReport.ReportPath = Application.StartupPath + "\\Reportes\\CierreDeCaja.rdlc";
                this.reportViewer1.LocalReport.DataSources.Add(new ReportDataSource("VentasxPlato", Basicas.VentasLapsoxPlatos(f.fecha, f.fecha)));
                this.reportViewer1.LocalReport.DataSources.Add(new ReportDataSource("TotalxFormaPago", Basicas.VentasxLapso(f.fecha, f.fecha)));
                this.reportViewer1.LocalReport.DataSources.Add(new ReportDataSource("VentasxDia", Basicas.VentasDiariasxLapso(f.fecha, f.fecha)));
                this.reportViewer1.LocalReport.DataSources.Add(new ReportDataSource("Parametros", new Parametro[] { Basicas.parametros() }));
                ReportParameter pUsuario = new
                        ReportParameter("cajero", string.Format("TODOS LOS CAJEROS"));
                        this.reportViewer1.LocalReport.SetParameters(pUsuario);
            }
            else
            {
                reportViewer1.ProcessingMode = ProcessingMode.Local;
                this.reportViewer1.LocalReport.ReportPath = Application.StartupPath + "\\Reportes\\CierreDeCaja.rdlc";
                this.reportViewer1.LocalReport.DataSources.Add(new ReportDataSource("VentasxPlato", Basicas.VentasLapsoxPlatos(f.fecha, f.fecha,cajero)));
                this.reportViewer1.LocalReport.DataSources.Add(new ReportDataSource("TotalxFormaPago", Basicas.VentasxLapso(f.fecha, f.fecha,cajero)));
                this.reportViewer1.LocalReport.DataSources.Add(new ReportDataSource("VentasxDia", Basicas.VentasDiariasxLapso(f.fecha, f.fecha,cajero)));
                this.reportViewer1.LocalReport.DataSources.Add(new ReportDataSource("Parametros", new Parametro[] { Basicas.parametros() }));
                ReportParameter pUsuario = new
                        ReportParameter("cajero", string.Format("CAJERO:{0}",cajero.Nombre));
                        this.reportViewer1.LocalReport.SetParameters(pUsuario);
            }
            this.reportViewer1.RefreshReport();
            this.ShowDialog();
        }
        public void CierreDeCajaHoras()
        {
            FrmLapsoHoras f = new FrmLapsoHoras();
            f.Text = "Corte De Caja x Horas";
            f.ShowDialog();
            if (f.DialogResult != System.Windows.Forms.DialogResult.OK)
                return;
            reportViewer1.ProcessingMode = ProcessingMode.Local;
            this.reportViewer1.LocalReport.ReportPath = Application.StartupPath + "\\Reportes\\CierreDeCajaHoras.rdlc";
            this.reportViewer1.LocalReport.DataSources.Add(new ReportDataSource("VentasxPlato", Basicas.VentasHorasxPlatos(f.desde, f.hasta)));
            this.reportViewer1.LocalReport.DataSources.Add(new ReportDataSource("TotalxFormaPago", Basicas.VentasxHoras(f.desde, f.hasta)));
            this.reportViewer1.LocalReport.DataSources.Add(new ReportDataSource("VentasxDia", Basicas.VentasDiariasxLapso(f.desde, f.hasta)));
            this.reportViewer1.LocalReport.DataSources.Add(new ReportDataSource("Parametros", new Parametro[] { Basicas.parametros() }));
            ReportParameter desde = new
                    ReportParameter("desde", f.desde.ToString("t"));
            ReportParameter hasta = new
                    ReportParameter("hasta", f.hasta.ToString("t"));
            ReportParameter fecha = new
                    ReportParameter("fecha", DateTime.Today.ToString("d"));
            this.reportViewer1.LocalReport.SetParameters(new ReportParameter[] { desde, hasta, fecha });
            this.reportViewer1.RefreshReport();
            this.ShowDialog();
        }
        public void VentasxLapso()
        {
            FrmLapso f = new FrmLapso();
            f.Text = "Ventas x Lapso";
            f.ShowDialog();
            if (f.DialogResult != System.Windows.Forms.DialogResult.OK)
                return;
            reportViewer1.ProcessingMode = ProcessingMode.Local;
            this.reportViewer1.LocalReport.ReportPath = Application.StartupPath + "\\Reportes\\VentasxLapso.rdlc";
            this.reportViewer1.LocalReport.DataSources.Add(new ReportDataSource("VentasxDia", Basicas.VentasDiariasxLapso(f.desde, f.hasta)));
            this.reportViewer1.LocalReport.DataSources.Add(new ReportDataSource("Parametros", new Parametro[] { Basicas.parametros() }));
            this.reportViewer1.RefreshReport();
            this.ShowDialog();
        }
        public void ConsumoxLapso()
        {
            FrmLapso f = new FrmLapso();
            f.Text = "Ventas x Lapso";
            f.ShowDialog();
            if (f.DialogResult != System.Windows.Forms.DialogResult.OK)
                return;
            List<IngredientesConsumo> lista = Basicas.ConsumoPlatosxLapso(f.desde, f.hasta);
            reportViewer1.ProcessingMode = ProcessingMode.Local;
            this.reportViewer1.LocalReport.ReportPath = Application.StartupPath + "\\Reportes\\ConsumoxLapso.rdlc";
            this.reportViewer1.LocalReport.DataSources.Add(new ReportDataSource("Consumo", Basicas.ConsumoPlatosxLapso(f.desde, f.hasta)));
            this.reportViewer1.LocalReport.DataSources.Add(new ReportDataSource("Parametros", new Parametro[] { Basicas.parametros() }));
            this.reportViewer1.LocalReport.SetParameters(new ReportParameter("Lapso", string.Format("Desde:{0} Hasta:{1}", f.desde.ToShortDateString(), f.hasta.ToShortDateString())));
            this.reportViewer1.RefreshReport();
            this.ShowDialog();
        }

        public void ConsumoxFecha()
        {
            FrmFechaCajero f = new FrmFechaCajero();
            f.Text = "Consumo x Fecha y Cajero";
            f.ShowDialog();
            if (f.DialogResult != System.Windows.Forms.DialogResult.OK)
                return;
            Usuario cajero = f.cajero;
            if (cajero == null)
            {
                List<IngredientesConsumo> lista = Basicas.ConsumoPorFecha(f.fecha);
                reportViewer1.ProcessingMode = ProcessingMode.Local;
                this.reportViewer1.LocalReport.ReportPath = Application.StartupPath + "\\Reportes\\ConsumoxLapso.rdlc";
                this.reportViewer1.LocalReport.DataSources.Add(new ReportDataSource("Consumo", Basicas.ConsumoPorFecha(f.fecha)));
                this.reportViewer1.LocalReport.DataSources.Add(new ReportDataSource("Parametros", new Parametro[] { Basicas.parametros() }));
                this.reportViewer1.LocalReport.SetParameters(new ReportParameter("Lapso", string.Format("Fecha:{0}", f.fecha.ToShortDateString())));
                this.reportViewer1.RefreshReport();
                this.ShowDialog();
            }
            else
            {
                List<IngredientesConsumo> lista = Basicas.ConsumoPorFechaCajero(f.fecha, f.cajero);
                reportViewer1.ProcessingMode = ProcessingMode.Local;
                this.reportViewer1.LocalReport.ReportPath = Application.StartupPath + "\\Reportes\\ConsumoxLapso.rdlc";
                this.reportViewer1.LocalReport.DataSources.Add(new ReportDataSource("Consumo", lista));
                this.reportViewer1.LocalReport.DataSources.Add(new ReportDataSource("Parametros", new Parametro[] { Basicas.parametros() }));
                this.reportViewer1.LocalReport.SetParameters(new ReportParameter("Lapso", string.Format("Cajero:{0} Fecha:{1}", f.cajero.Nombre, f.fecha.ToShortDateString())));
                this.reportViewer1.RefreshReport();
                this.ShowDialog();
            }
        }
        public void VentasxProducto()
        {
            FrmLapso f = new FrmLapso();
            f.Text = "Ventas x Producto";
            f.ShowDialog();
            if (f.DialogResult != System.Windows.Forms.DialogResult.OK)
                return;
            reportViewer1.ProcessingMode = ProcessingMode.Local;
            this.reportViewer1.LocalReport.ReportPath = Application.StartupPath + "\\Reportes\\VentasxPlatos.rdlc";
            this.reportViewer1.LocalReport.DataSources.Add(new ReportDataSource("VentasxPlato", Basicas.VentasLapsoxPlatos(f.desde, f.hasta)));
            this.reportViewer1.LocalReport.DataSources.Add(new ReportDataSource("Parametros", new Parametro[] { Basicas.parametros() }));
            this.reportViewer1.RefreshReport();
            this.ShowDialog();
        }
        public void LibroDeVentas()
        {
            FrmMesyA�o f = new FrmMesyA�o();
            f.Text = "Libro de Ventas";
            f.ShowDialog();
            if (f.DialogResult != System.Windows.Forms.DialogResult.OK)
                return;
            using (var db = new FeriaEntities())
            {
                int Mes = f.Mes;
                int A�o = f.A�o;
                var consulta = from q in db.Facturas
                               where (q.Fecha.Value.Month == Mes && q.Fecha.Value.Year == f.A�o) && q.Tipo == "FACTURA" 
                               orderby q.Numero
                               select q;
                reportViewer1.ProcessingMode = ProcessingMode.Local;
                this.reportViewer1.LocalReport.ReportPath = Application.StartupPath + "\\Reportes\\LibroDeVentas.rdlc";
                this.reportViewer1.LocalReport.DataSources.Add(new ReportDataSource("Facturas", consulta.ToList() ));
                this.reportViewer1.LocalReport.DataSources.Add(new ReportDataSource("Parametros", new Parametro[] { Basicas.parametros() }));
                this.reportViewer1.RefreshReport();
                this.ShowDialog();
            }
        }
        public void LibroDeCompras()
        {
            FrmMesyA�o f = new FrmMesyA�o();
            f.Text = "Libro de Compras";
            f.ShowDialog();
            if (f.DialogResult != System.Windows.Forms.DialogResult.OK)
                return;
            using (var db = new FeriaEntities())
            {
                int Mes = f.Mes;
                int A�o = f.A�o;
                var consulta = from q in db.Compras
                               where (q.Fecha.Value.Month == Mes && q.Fecha.Value.Year == f.A�o) && q.LibroCompras==true
                               orderby q.Fecha
                               select q;
                reportViewer1.ProcessingMode = ProcessingMode.Local;
                this.reportViewer1.LocalReport.ReportPath = Application.StartupPath + "\\Reportes\\LibroDeCompras.rdlc";
                this.reportViewer1.LocalReport.DataSources.Add(new ReportDataSource("Compras", consulta.ToList()));
                this.reportViewer1.LocalReport.DataSources.Add(new ReportDataSource("Parametros", new Parametro[] { Basicas.parametros() }));
                this.reportViewer1.RefreshReport();
                this.ShowDialog();
            }
        }
        public void ListadoIngredientes(List<Ingrediente> lista)
        {
            reportViewer1.ProcessingMode = ProcessingMode.Local;
            this.reportViewer1.LocalReport.ReportPath = Application.StartupPath + "\\Reportes\\ListadoIngredientes.rdlc";
            this.reportViewer1.LocalReport.DataSources.Add(new ReportDataSource("Ingredientes", lista));
            this.reportViewer1.LocalReport.DataSources.Add(new ReportDataSource("Parametros", new Parametro[] { Basicas.parametros() }));
            this.reportViewer1.RefreshReport();
            this.ShowDialog();
        }
        public void IngredienteInventarios(List<IngredientesInventario> lista)
        {
            reportViewer1.ProcessingMode = ProcessingMode.Local;
            this.reportViewer1.LocalReport.ReportPath = Application.StartupPath + "\\Reportes\\IngredientesInventarios.rdlc";
            this.reportViewer1.LocalReport.DataSources.Add(new ReportDataSource("IngredientesInventarios", lista));
            this.reportViewer1.LocalReport.DataSources.Add(new ReportDataSource("Parametros", new Parametro[] { Basicas.parametros() }));
            this.reportViewer1.RefreshReport();
            this.ShowDialog();
        }
        public void ListadoMesas(List<Mesa> Lista)
        {
            reportViewer1.ProcessingMode = ProcessingMode.Local;
            this.reportViewer1.LocalReport.ReportPath = Application.StartupPath + "\\Reportes\\ListadoMesas.rdlc";
            this.reportViewer1.LocalReport.DataSources.Add(new ReportDataSource("Mesas", Lista));
            this.reportViewer1.LocalReport.DataSources.Add(new ReportDataSource("Parametros", new Parametro[] { Basicas.parametros() }));
            this.reportViewer1.RefreshReport();
            this.ShowDialog();
        }
        public void ReporteFacturasNaturalesyJuridicas()
        {
            FrmLapso f = new FrmLapso();
            f.Text = "Listado de Facturas Naturales y Juridicas";
            f.ShowDialog();
            if (f.DialogResult != System.Windows.Forms.DialogResult.OK)
                return;
            List<Factura> Lista = FactoryFacturas.getNaturalesJuridicas(f.desde,f.hasta);
            reportViewer1.ProcessingMode = ProcessingMode.Local;
            this.reportViewer1.LocalReport.ReportPath = Application.StartupPath + "\\Reportes\\ListadoFacturasNaturalesJuridicas.rdlc";
            this.reportViewer1.LocalReport.DataSources.Add(new ReportDataSource("Facturas", Lista));
            this.reportViewer1.LocalReport.DataSources.Add(new ReportDataSource("Parametros", new Parametro[] { Basicas.parametros() }));
            //this.reportViewer1.LocalReport.SetParameters(new ReportParameter("Empresa", Basicas.parametros().Empresa));
            //this.reportViewer1.LocalReport.SetParameters(new ReportParameter("EmpresaDireccion", Basicas.parametros().EmpresaDireccion));
            //this.reportViewer1.LocalReport.SetParameters(new ReportParameter("EmpresaRif", Basicas.parametros().EmpresaRif));
            //this.reportViewer1.LocalReport.SetParameters(new ReportParameter("EmpresaTelefonos", Basicas.parametros().EmpresaTelefonos));
            this.reportViewer1.RefreshReport();
            this.ShowDialog();
        }
        public void LibroDeVentas(List<LibroVenta> Lista, string periodo)
        {
            foreach (var item in Lista)
            {
                if (item.CedulaRif[0] == 'J' || item.CedulaRif[0] == 'G')
                {
                    item.MontoGravableContribuyentes = item.MontoGravable;
                    item.MontoIvaContribuyentes = item.MontoIva;
                    item.TasaIvaContribuyentes = item.TasaIva;
                }
                else
                {
                    item.MontoGravableNoContribuyentes = item.MontoGravable;
                    item.MontoIvaNoContribuyentes = item.MontoIva;
                    item.TasaIvaNoContribuyentes = item.TasaIva;
                }
            }
            reportViewer1.ProcessingMode = ProcessingMode.Local;
            this.reportViewer1.LocalReport.ReportPath = Application.StartupPath + "\\Reportes\\LibroDeVentas.rdlc";
            this.reportViewer1.LocalReport.DataSources.Add(new ReportDataSource("LibroVentas", Lista));
            this.reportViewer1.LocalReport.DataSources.Add(new ReportDataSource("Parametros", new Parametro[] { Basicas.parametros() }));
            this.reportViewer1.LocalReport.SetParameters(new ReportParameter("Periodo", periodo));
            this.reportViewer1.RefreshReport();
            this.ShowDialog();
        }
        public void LibroDeVentasResumido()
        {
            FrmMesyA�o f = new FrmMesyA�o();
            f.Text = "Libro de Ventas Resumido";
            f.ShowDialog();
            if (f.DialogResult != System.Windows.Forms.DialogResult.OK)
                return;
            using (var db = new FeriaEntities())
            {
                int Mes = f.Mes;
                int A�o = f.A�o;
                List<LibroVenta> libro = new List<LibroVenta>();
                var consulta = from q in db.LibroVentas
                               where (q.Mes == Mes && q.A�o == f.A�o)
                               orderby q.Factura
                               select q;
                if (consulta.FirstOrDefault() == null)
                    return;
                LibroVenta itemActual = null;
                LibroVenta itemInicial = null;
                LibroVenta ultimoItem = null;
                string Inicio = "";
                foreach (LibroVenta item in consulta)
                {
                    if (itemActual == null)
                    {
                        itemActual = item;
                        ultimoItem = item;
                        Inicio = item.Factura;
                        itemActual.Inicio = Inicio;
                        itemActual.Final = Inicio;
                    }
                    else
                    {
                        if (item.NumeroZ == itemActual.NumeroZ && (item.CedulaRif.Substring(0, 1) == "V" || item.CedulaRif.Substring(0, 1) == "E"))
                        {
                            itemActual.MontoIva = itemActual.MontoIva + item.MontoIva;
                            itemActual.MontoTotal = itemActual.MontoTotal + item.MontoTotal;
                            itemActual.MontoExento = itemActual.MontoExento + item.MontoExento;
                            itemActual.MontoGravable = itemActual.MontoGravable + item.MontoGravable;
                            itemActual.RazonSocial = "CONTADO";
                            itemActual.CedulaRif = "V000000000";
                            itemActual.Factura = Inicio + " " + item.Factura;
                            ultimoItem = item;
                            itemActual.Final = item.Factura;
                        }
                        else
                        {
                            if (item.CedulaRif.Substring(0, 1) == "V" || item.CedulaRif.Substring(0, 1) == "E")
                            {
                                libro.Add(itemActual);
                                itemActual = item;
                                itemInicial = item;
                                ultimoItem = item;
                                Inicio = item.Factura;
                                itemActual.Inicio = Inicio;
                                itemActual.Final = Inicio;
                            }
                            else
                            {
                                if (item.IdLibroVentas != itemActual.IdLibroVentas)
                                {
                                    libro.Add(itemActual);
                                }
                                libro.Add(item);
                                itemActual = null;
                            }
                        }
                    }
                }
                if (itemActual != null)
                {
                    libro.Add(itemActual);
                }
                foreach (var item in libro)
                {
                    if (item.CedulaRif[0] == 'J' || item.CedulaRif[0] == 'G')
                    {
                        item.MontoGravableContribuyentes = item.MontoGravable;
                        item.MontoIvaContribuyentes = item.MontoIva;
                        item.TasaIvaContribuyentes = item.TasaIva;
                        item.Inicio = item.Factura;
                        item.Final = item.Factura;
                    }
                    else
                    {
                        item.MontoGravableNoContribuyentes = item.MontoGravable;
                        item.MontoIvaNoContribuyentes = item.MontoIva;
                        item.TasaIvaNoContribuyentes = item.TasaIva;
                    }
                }
                reportViewer1.ProcessingMode = ProcessingMode.Local;
                this.reportViewer1.LocalReport.ReportPath = Application.StartupPath + "\\Reportes\\LibroDeVentasResumido.rdlc";
                this.reportViewer1.LocalReport.DataSources.Add(new ReportDataSource("LibroVentas", libro));
                this.reportViewer1.LocalReport.DataSources.Add(new ReportDataSource("Parametros", new Parametro[] { Basicas.parametros() }));
                this.reportViewer1.LocalReport.SetParameters(new ReportParameter("Periodo", string.Format("Mes {0} A�o {1}", Mes, A�o)));
                this.reportViewer1.RefreshReport();
                this.ShowDialog();
            }
        }
    }
}
