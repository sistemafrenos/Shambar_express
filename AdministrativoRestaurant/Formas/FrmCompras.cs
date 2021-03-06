﻿using System;
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
    public partial class FrmCompras : Form
    {
        FeriaEntities db = new FeriaEntities();
        List<Compra> Lista = new List<Compra>();
        public FrmCompras()
        {
            InitializeComponent();
            this.Load += new EventHandler(FrmCompras_Load);
            this.FormClosed += new FormClosedEventHandler(FrmCompras_FormClosed);
        }
        void FrmCompras_FormClosed(object sender, FormClosedEventArgs e)
        {
            Pantallas.ComprasLista = null;
        }
        void FrmCompras_Load(object sender, EventArgs e)
        {
            Busqueda();
            Buscar.Click += new EventHandler(Buscar_Click);
            gridControl1.KeyDown += new KeyEventHandler(gridControl1_KeyDown);
            btnNuevo.Click += new EventHandler(btnNuevo_Click);
            btnVer.Click += new EventHandler(btnVer_Click);
            btnEliminar.Click += new EventHandler(btnEliminar_Click);
            btnEditar.Click += new EventHandler(btnEditar_Click);
            txtBuscar.KeyDown += new KeyEventHandler(txtBuscar_KeyDown);
            btnActualizarInventario.Click += new EventHandler(btnActualizarInventario_Click);
            gridView1.OptionsLayout.Columns.Reset();
        }
        void btnEditar_Click(object sender, EventArgs e)
        {
            if (this.bs.Current == null)
                return;
            Compra c = (Compra)this.bs.Current;
            if (c.ActualizadoInventario.GetValueOrDefault(false)==true)
            {
                MessageBox.Show("Esta compra ya fue pasada a inventario y no se puede modificar");
                return;
            }
            FrmComprasItem f = new FrmComprasItem();
            f.registro = (Compra)this.bs.Current;
            f.Modificar();
        }
        void btnActualizarInventario_Click(object sender, EventArgs e)
        {
            if (this.bs.Current == null)
                return;
           Compra  registro = (Compra)this.bs.Current;
           try
           {
               if (registro.ActualizadoInventario.GetValueOrDefault(false) == true)
               {
                   throw new Exception("Esta compra se tiene el inventario actualizado");
               }
               registro.FechaInventario = DateTime.Today;
               FactoryCompras.Inventario(registro);

               this.bs.ResetCurrentItem();
           }
           catch (Exception x)
           {
               MessageBox.Show(x.Message);
           }
        }
        void btnEliminar_Click(object sender, EventArgs e)
        {
            EliminarRegistro();
        }
        void btnVer_Click(object sender, EventArgs e)
        {
            if (this.bs.Current == null)
                return;
            FrmComprasItem f = new FrmComprasItem();
            f.registro = (Compra)this.bs.Current;
            f.Ver();
        }
        void btnNuevo_Click(object sender, EventArgs e)
        {
            FrmComprasItem f = new FrmComprasItem();
            f.Incluir();
            if (f.DialogResult == System.Windows.Forms.DialogResult.OK)
            {
                Busqueda();
            }
        }              
        private void Busqueda()
        {
            db = new FeriaEntities();
            switch (txtFiltro.Text)
            {
                case "TODAS":
                    Lista = (from p in db.Compras
                             where p.RazonSocial.Contains(txtBuscar.Text) || txtBuscar.Text.Length == 0
                            orderby p.Fecha
                            select p).ToList();
                    break;
                case "AYER":
                    DateTime ayer = DateTime.Today.AddDays(-1);
                    Lista = (from p in db.Compras
                             where (p.RazonSocial.Contains(txtBuscar.Text) || txtBuscar.Text.Length == 0)&& p.Fecha.Value == ayer
                             orderby p.Numero
                             select p).ToList();
                    break;
                case "HOY":
                    Lista = (from p in db.Compras
                             where (p.RazonSocial.Contains(txtBuscar.Text) || txtBuscar.Text.Length == 0) && p.Fecha.Value == DateTime.Today
                             orderby p.Numero
                             select p).ToList();
                    break;
                case "ESTE MES":
                    Lista = (from p in db.Compras
                             where (p.RazonSocial.Contains(txtBuscar.Text) || txtBuscar.Text.Length == 0)&& p.Fecha.Value.Month == DateTime.Today.Month && p.Fecha.Value.Year == DateTime.Today.Year
                             orderby p.Numero
                             select p).ToList();
                    break;
            }
            this.bs.DataSource = Lista;
            this.bs.ResetBindings(true);
        }
        private void EliminarRegistro()
        {
            if (this.bs.Current == null)
                return;
            Compra documento = (Compra)this.bs.Current;
            if (MessageBox.Show("Esta seguro de eliminar esta compra", "Atencion", MessageBoxButtons.YesNo) != System.Windows.Forms.DialogResult.Yes)
                return;
            try
            {
                if (documento.ActualizadoInventario.GetValueOrDefault(false) == true)
                {
                    FactoryCompras.InventarioDevolver(documento);
                }
                db.Compras.DeleteObject(documento);
                db.SaveChanges();
                Busqueda();
                }
                catch (Exception x)
                {
                    MessageBox.Show(x.Message);
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
                        btnVer.PerformClick();
                        break;
                    case Keys.Delete:
                        btnEliminar.PerformClick();
                        break;
                    case Keys.Subtract:
                        btnEliminar.PerformClick();
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
        #endregion
    }
}
