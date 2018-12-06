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
    public partial class FrmComprasItem : Form
    {
        private bool esNuevo = true;
        public FrmComprasItem()
        {
            InitializeComponent();
            this.Load += new EventHandler(FrmComprasItem_Load);
        }
        void FrmComprasItem_Load(object sender, EventArgs e)
        {
            this.KeyPreview = true;
            this.Aceptar.Click += new EventHandler(Aceptar_Click);
            this.Cancelar.Click += new EventHandler(Cancelar_Click);
            this.KeyDown += new KeyEventHandler(FrmComprasItem_KeyDown);
            #region Proveedor
            this.CedulaRifButtonEdit.Validating+=new CancelEventHandler(CedulaRifButtonEdit_Validating);
            this.CedulaRifButtonEdit.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(CedulaRifButtonEdit_ButtonClick);
            #endregion
            #region Ingredientes
            txtIngrediente.Validating += new CancelEventHandler(txtIngrediente_Validating);
            txtIngrediente.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(txtIngrediente_ButtonClick);
            txtCantidad.Validating += new CancelEventHandler(txtCantidad_Validating);
            txtCantidadNeta.Validating += new CancelEventHandler(txtCantidadNeta_Validating);
            txtCosto.Validating += new CancelEventHandler(txtCosto_Validating);
            txtTasaIva.Validating += new CancelEventHandler(txtTasaIva_Validating);
            txtCostoIva.Validating += new CancelEventHandler(txtCostoIva_Validating);
            this.comprasIngredienteBindingSource.ListChanged += new ListChangedEventHandler(comprasIngredienteBindingSource_ListChanged);
            this.gridControl1.KeyDown+=new KeyEventHandler(gridControl1_KeyDown);
            #endregion
        }
        public Compra registro = new Compra();
        private Ingrediente ingrediente = null;
        private ComprasIngrediente registroDetalle = null;
        private Proveedore proveedor = new Proveedore();
        FeriaEntities db = new FeriaEntities();
        #region Proveedor
        void CedulaRifButtonEdit_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            FrmBuscarEntidades F = new FrmBuscarEntidades();
            F.BuscarProveedores("");
            if (F.registro != null)
            {
                proveedor = (Proveedore)F.registro;
                proveedor = FactoryProveedores.Item(proveedor.CedulaRif);
                LeerProveedor();
            }
            else
            {
                proveedor = new Proveedore();
            }
        }
        void CedulaRifButtonEdit_Validating(object sender, CancelEventArgs e)
        {
            DevExpress.XtraEditors.TextEdit Editor = (DevExpress.XtraEditors.TextEdit)sender;
            if (!Editor.IsModified)
                return;
            string Texto = Editor.Text;
            this.compraBindingSource.EndEdit();
            List<Proveedore> T = FactoryProveedores.getItems(Texto);
            switch (T.Count)
            {
                case 0:
                    proveedor = new Proveedore();
                    proveedor.CedulaRif = Basicas.CedulaRif(Texto);
                    break;
                case 1:
                    proveedor = T[0];
                    break;
                default:
                    FrmBuscarEntidades F = new FrmBuscarEntidades();
                    F.BuscarProveedores(Texto);
                    if (F.registro != null)
                    {
                        proveedor = (Proveedore)F.registro;
                        proveedor = FactoryProveedores.Item(proveedor.CedulaRif);
                    }
                    else
                    {
                        proveedor = null;
                    }
                    break;
            }
            LeerProveedor();
        }
        private void LeerProveedor()
        {
            registro.CedulaRif = proveedor.CedulaRif;
            registro.RazonSocial = proveedor.RazonSocial;
            registro.Direccion = proveedor.Direccion;
            this.compraBindingSource.ResetCurrentItem();
        }
        #endregion
        private void Limpiar()
        {
            registro = new Compra();
            registro.Fecha = DateTime.Today;
            registro.Año = registro.Fecha.Value.Year;
            registro.Mes = registro.Fecha.Value.Month;
            registro.TasaIva = Basicas.parametros().TasaIva;
            registro.IdUsuario = FactoryUsuarios.UsuarioActivo.IdUsuario;
            proveedor = new Proveedore();
            registro.LibroCompras = true;
        }
        public void Incluir()
        {
            Limpiar();
            Enlazar();
            this.ShowDialog();
        }
        public void Ver()
        {
            Enlazar();
            this.Aceptar.Enabled = false;
            this.ShowDialog();
        }
        public void Modificar()
        {
            Enlazar();
            esNuevo = false;
            this.ShowDialog();
        }
        private void Enlazar()
        {
            if (registro == null)
            {
                Limpiar();
            }
            this.compraBindingSource.DataSource = registro;
            this.compraBindingSource.ResetBindings(true);
            this.comprasIngredienteBindingSource.DataSource = registro.ComprasIngredientes;
            this.comprasIngredienteBindingSource.ResetBindings(true);
        }
        private void Aceptar_Click(object sender, EventArgs e)
        {
            try
            {
                compraBindingSource.EndEdit();
                comprasIngredienteBindingSource.EndEdit();
                registro = (Compra)compraBindingSource.Current;                
                FactoryCompras.Validar(registro);
                this.Guadar();
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al validar los datos \n" + ex.Message, "Atencion", MessageBoxButtons.OK);
            }
        }
        private void Guadar()
        {
            this.compraBindingSource.EndEdit();
            registro.IdUsuario = FactoryUsuarios.UsuarioActivo.IdUsuario;
            if (esNuevo)
            {
                registro.IdCompra = FactoryContadores.GetMax("IdCompra");
            }
            foreach (HK.ComprasIngrediente p in registro.ComprasIngredientes)
            {
                if (p.IdCompraIngrediente == null)
                {
                    p.IdCompraIngrediente = FactoryContadores.GetMax("IdCompraIngrediente");
                }
            }
            proveedor = FactoryProveedores.Item(db, registro.CedulaRif);
            if (proveedor == null)
            {
                proveedor = new Proveedore();
                proveedor.IdProveedor = FactoryContadores.GetMax("IdProveedor");
                proveedor.CedulaRif = registro.CedulaRif;
                proveedor.RazonSocial = registro.RazonSocial;
                proveedor.Direccion = registro.Direccion;
                db.Proveedores.AddObject(proveedor);
            }
            else
            {
                proveedor.CedulaRif = registro.CedulaRif;
                proveedor.RazonSocial = registro.RazonSocial;
                proveedor.Direccion = registro.Direccion;
            }
            if (esNuevo)
            {
                db.Compras.AddObject(registro);
            }
            try
            {
                db.SaveChanges();
            }
            catch (Exception x)
            {
                MessageBox.Show(x.InnerException.Message);
            }
        }
        private void Cancelar_Click(object sender, EventArgs e)
        {
            this.compraBindingSource.ResetCurrentItem();
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
        private void FrmComprasItem_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Escape:
                    
                    //this.Cancelar.PerformClick();
                    e.Handled = true;
                    break;
                case Keys.F12:
                    this.Aceptar.PerformClick();
                    e.Handled = true;
                    break;
            }
        }
        #region ManejoDocumentoProductos
        private void gridControl1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Return)
            {
                gridView1.MoveBy(0);
            }

            if (gridView1.ActiveEditor == null)
            {
                if (e.KeyCode == Keys.Delete || e.KeyCode == Keys.Subtract)
                {
                    if (this.gridView1.IsFocusedView)
                    {
                        ComprasIngrediente i = (ComprasIngrediente)this.comprasIngredienteBindingSource.Current;
                        registro.ComprasIngredientes.Remove(i);
                    }
                    e.Handled = true;
                }
            }
        }
        void comprasIngredienteBindingSource_ListChanged(object sender, ListChangedEventArgs e)
        {
            registro.Totalizar();
            this.compraBindingSource.ResetCurrentItem();
        }
        private void txtIngrediente_Validating(object sender, CancelEventArgs e)
        {
            DevExpress.XtraEditors.TextEdit Editor = (DevExpress.XtraEditors.TextEdit)this.gridControl1.MainView.ActiveEditor;
            if (!Editor.IsModified)
                return;
            string Texto = Editor.Text;
            registroDetalle = (ComprasIngrediente)this.comprasIngredienteBindingSource.Current;
            if (UbicarProducto(Texto))
            {
                if (ingrediente == null)
                    return;
                LeerIngrediente(false);
                Editor.Text = ingrediente.Descripcion;
                if (string.IsNullOrEmpty(ingrediente.Descripcion))
                {
                    Editor.Undo();
                    e.Cancel = true;
                }
            }
            else
            {
                LeerIngrediente(false);
                Editor.Undo();
            }
        }
        void txtIngrediente_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            registroDetalle = (ComprasIngrediente)this.comprasIngredienteBindingSource.Current;
            UbicarProducto("");
            LeerIngrediente(false);
        }
        private void LeerIngrediente(bool Buscar)
        {
            if (ingrediente == null)
            {
                return;
            }
            registroDetalle = (ComprasIngrediente)this.comprasIngredienteBindingSource.Current;
            //      this.gridControl1.MainView.ActiveEditor.Text = producto.Codigo;
            registroDetalle.Cantidad = 1;
            registroDetalle.CantidadNeta = 1;
            registroDetalle.Costo = ingrediente.Costo;
            registroDetalle.ExistenciaAnterior = ingrediente.Existencia;
            registroDetalle.IdIngrediente = ingrediente.IdIngrediente;
            registroDetalle.Ingrediente = ingrediente.Descripcion;
            registroDetalle.TasaIva = ingrediente.TasaIva;
            registroDetalle.UnidadMedida = ingrediente.UnidadMedida;
            CalcularMontoItem(registroDetalle);
        }
        private bool UbicarProducto(string Texto)
        {
            List<Ingrediente> T = new List<Ingrediente>();
            T = FactoryIngredientes.getItems(Texto);
            switch (T.Count)
            {
                case 0:
                    if (MessageBox.Show("Suministro no Encontrado, Desea crear uno nuevo", "Atencion", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                    {
                        ingrediente = new Ingrediente();
                        return false;
                    }
                    FrmIngredientesItem nuevo = new FrmIngredientesItem();
                    nuevo.descripcion = Texto;
                    nuevo.Incluir();
                    if (nuevo.DialogResult == DialogResult.OK)
                    {
                        using (var db = new FeriaEntities())
                        {
                            nuevo.registro.IdIngrediente = FactoryContadores.GetMax("IdIngrediente");
                            nuevo.registro.Activo = true;
                            db.Ingredientes.AddObject(nuevo.registro);
                            db.SaveChanges();
                        }
                    }
                    if (nuevo.DialogResult == DialogResult.OK)
                    {
                        ingrediente = nuevo.registro;
                    }
                    else
                    {
                        ingrediente = new Ingrediente();
                        return false;
                    }
                    break;
                case 1:
                    ingrediente = T[0];
                    break;
                default:
                    FrmBuscarEntidades F = new FrmBuscarEntidades();
                    F.BuscarIngredientes(Texto);
                    ingrediente = (Ingrediente)F.registro;
                    break;
            }
            LeerIngrediente(false);
            return true;
        }
        private void txtCosto_Validating(object sender, CancelEventArgs e)
        {
            DevExpress.XtraEditors.CalcEdit Editor = (DevExpress.XtraEditors.CalcEdit)this.gridControl1.MainView.ActiveEditor;
            if (!Editor.IsModified)
                return;
            if (this.comprasIngredienteBindingSource.Current == null)
                return;
            registroDetalle = (ComprasIngrediente)this.comprasIngredienteBindingSource.Current;
            registroDetalle.Costo = (double)Editor.Value;
            CalcularMontoItem(registroDetalle);
        }
        private void txtCantidad_Validating(object sender, CancelEventArgs e)
        {
            DevExpress.XtraEditors.CalcEdit Editor = (DevExpress.XtraEditors.CalcEdit)this.gridControl1.MainView.ActiveEditor;
            if (!Editor.IsModified)
                return;
            if ((double)Editor.Value <= 0)
            {
                Editor.Value = 1;
            }
            if (this.comprasIngredienteBindingSource.Current == null)
                return;
            registroDetalle = (ComprasIngrediente)this.comprasIngredienteBindingSource.Current;
            registroDetalle.Cantidad = (double)Editor.Value;
            registroDetalle.CantidadNeta = registroDetalle.Cantidad;
            CalcularMontoItem(registroDetalle);
        }
        void txtCostoIva_Validating(object sender, CancelEventArgs e)
        {
            DevExpress.XtraEditors.CalcEdit Editor = (DevExpress.XtraEditors.CalcEdit)this.gridControl1.MainView.ActiveEditor;
            if (!Editor.IsModified)
                return;
            if (this.comprasIngredienteBindingSource.Current == null)
                return;
            registroDetalle = (ComprasIngrediente)this.comprasIngredienteBindingSource.Current;
            registroDetalle.Costo = (double)Editor.Value / (1 + (registroDetalle.TasaIva / 100));
        }
        void txtTasaIva_Validating(object sender, CancelEventArgs e)
        {
            DevExpress.XtraEditors.CalcEdit Editor = (DevExpress.XtraEditors.CalcEdit)this.gridControl1.MainView.ActiveEditor;
            if (!Editor.IsModified)
                return;
            if (this.comprasIngredienteBindingSource.Current == null)
                return;
            registroDetalle = (ComprasIngrediente)this.comprasIngredienteBindingSource.Current;
            registroDetalle.TasaIva = (double)Editor.Value;
            CalcularMontoItem(registroDetalle);
        }
        void txtCantidadNeta_Validating(object sender, CancelEventArgs e)
        {
            DevExpress.XtraEditors.CalcEdit Editor = (DevExpress.XtraEditors.CalcEdit)this.gridControl1.MainView.ActiveEditor;
            if (!Editor.IsModified)
                return;
            if (this.comprasIngredienteBindingSource.Current == null)
                return;
            registroDetalle = (ComprasIngrediente)this.comprasIngredienteBindingSource.Current;
            if ((double)Editor.Value == 0)
            {
                Editor.Value = (decimal)registroDetalle.Cantidad;
            }
            registroDetalle.CantidadNeta = (double)Editor.Value;
            CalcularMontoItem(registroDetalle);
        }
        private void CalcularMontoItem(ComprasIngrediente item)
        {
            if (item != null)
            {
                this.comprasIngredienteBindingSource.EndEdit();
                item.Iva = registroDetalle.Costo * registroDetalle.TasaIva / 100;
                item.CostoIva = item.Costo + item.Iva;
                item.Total = registroDetalle.Cantidad * registroDetalle.CostoIva;
                item.CostoNeto = registroDetalle.Total / registroDetalle.CantidadNeta;
                registro.Totalizar();
                this.comprasIngredienteBindingSource.ResetCurrentItem();
            }
            registro.Totalizar();
        }
        #endregion
    }
}
