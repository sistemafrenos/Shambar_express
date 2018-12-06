using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using HK;
using HK.Clases;

namespace HK.Formas
{
    public partial class FrmFacturas : Form
    {
        FeriaEntities db = new FeriaEntities();
        List<Factura> Lista = new List<Factura>();
        public FrmFacturas()
        {
            InitializeComponent();
            this.Load += new EventHandler(FrmFacturas_Load);
        }
        void FrmFacturas_Load(object sender, EventArgs e)
        {
            Busqueda();
            Buscar.Click += new EventHandler(Buscar_Click);
            gridControl1.KeyDown += new KeyEventHandler(gridControl1_KeyDown);
            VerFactura.Click += new EventHandler(VerFactura_Click);
            Eliminar.Click += new EventHandler(Eliminar_Click);
            txtBuscar.KeyDown += new KeyEventHandler(txtBuscar_KeyDown);
            Duplicar.Click += new EventHandler(Duplicar_Click);
            this.FormClosed += new FormClosedEventHandler(FrmFacturasLista_FormClosed);
            gridView1.OptionsLayout.Columns.Reset();
        }        
        void VerFactura_Click(object sender, EventArgs e)
        {
            if (this.bs.Current == null)
                return;
            Factura documento = (Factura)this.bs.Current;
            FrmFacturasItem f = new FrmFacturasItem();
            f.factura = documento;
            f.Ver();
        }
        void FrmFacturasLista_FormClosed(object sender, FormClosedEventArgs e)
        {
            Pantallas.Facturaslista = null;
        }        
        private void Busqueda()
        {
            db = new FeriaEntities();
            switch (txtFiltro.Text)
            {
                case "TODAS":
                    Lista = (from p in db.Facturas
                             where p.RazonSocial.Contains(txtBuscar.Text) || txtBuscar.Text.Length == 0
                            orderby p.Fecha
                            select p).ToList();
                    break;
                case "AYER":
                    DateTime ayer = DateTime.Today.AddDays(-1);
                    Lista = (from p in db.Facturas
                             where (p.RazonSocial.Contains(txtBuscar.Text) || txtBuscar.Text.Length == 0)&& p.Fecha.Value == ayer
                             orderby p.Numero
                             select p).ToList();
                    break;
                case "HOY":
                    Lista = (from p in db.Facturas
                             where (p.RazonSocial.Contains(txtBuscar.Text) || txtBuscar.Text.Length == 0) && p.Fecha.Value == DateTime.Today
                             orderby p.Numero
                             select p).ToList();
                    break;
                case "ESTE MES":
                    Lista = (from p in db.Facturas
                             where (p.RazonSocial.Contains(txtBuscar.Text) || txtBuscar.Text.Length == 0)&& p.Fecha.Value.Month == DateTime.Today.Month && p.Fecha.Value.Year == DateTime.Today.Year
                             orderby p.Numero
                             select p).ToList();
                    break;
            }
            this.bs.DataSource = Lista;
            this.bs.ResetBindings(true);
        }
        private void DuplicarRegistro()
        {
            if (this.bs.Current == null)
                return;
            Factura documento = (Factura)this.bs.Current;
            try
            {
                FiscalBixolon f = new FiscalBixolon();
                f.ImprimeFacturaCopia(documento.Numero);
                f = null;
            }
            catch (Exception x)
            {
                MessageBox.Show(x.Message);
            }
        }
        private void EliminarRegistro()
        {
            if (this.bs.Current == null)
                return;
            Factura documento = (Factura)this.bs.Current;
            if (documento.Anulado.GetValueOrDefault(false) == true)
            {
                if (MessageBox.Show("Esta operacion ya fue devuelta,Desea realizar la devolucion de nuevo", "Atencion", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes)
                {
                    try
                    {
                        FiscalBixolon f = new FiscalBixolon();
                        f.ImprimeDevolucion(documento, documento.Numero);
                        f = null;
                    }
                    catch (Exception x)
                    {
                        MessageBox.Show(x.Message);
                    }
                }
            }
            else
            {
                try
                {
                    FiscalBixolon f = new FiscalBixolon();
                    f.ImprimeDevolucion(documento, documento.Numero);
                    documento.Anulado = true;
                    db.SaveChanges();
                    f = null;
                }
                catch (Exception x)
                {
                    MessageBox.Show(x.Message);
                }
            }
        }
        #region Eventos
        private void gridControl1_KeyDown(object sender, KeyEventArgs e)
        {
            if (gridView1.ActiveEditor == null)
            {
                switch (e.KeyCode)
                {
                    case Keys.Return:
                        VerFactura.PerformClick();
                        break;
                    case Keys.Delete:
                        Eliminar.PerformClick();
                        break;
                    case Keys.Subtract:
                        Eliminar.PerformClick();
                        break;
                    case Keys.P:
                        Duplicar.PerformClick();
                        break;
                }
            }
        }
        private void Buscar_Click(object sender, EventArgs e)
        {
            Busqueda();
        }
        private void Eliminar_Click(object sender, EventArgs e)
        {
            EliminarRegistro();
        }
        private void txtBuscar_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Return)
            {
                Busqueda();
            }
        }
        private void Duplicar_Click(object sender, EventArgs e)
        {
            DuplicarRegistro();
        }
        #endregion
    }
}
