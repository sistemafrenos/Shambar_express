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
    public partial class FrmPlatosItem : Form
    {
        bool esNuevo = false;
        public Plato registro =null;
        private Ingrediente ingrediente=null;
        private PlatosIngrediente platoIngrediente= null;
        FeriaEntities db = new FeriaEntities();
        public FrmPlatosItem()
        {
            InitializeComponent();
            this.Load += new EventHandler(FrmPlatosItem_Load);
        }
        void FrmPlatosItem_Load(object sender, EventArgs e)
        {
            this.KeyPreview = true;
            this.KeyDown += new KeyEventHandler(Frm_KeyDown);
            this.Aceptar.Click += new EventHandler(Aceptar_Click);
            this.Cancelar.Click += new EventHandler(Cancelar_Click);
            this.PrecioCalcEdit.Validating += new CancelEventHandler(PrecioCalcEdit_Validating);
            this.PrecioConIvaCalcEdit.Validating+=new CancelEventHandler(PrecioConIvaCalcEdit_Validating);
            this.GrupoComboBoxEdit.Properties.Items.AddRange(FactoryPlatos.getArrayGrupos());
            this.pictureBox1.Click += new EventHandler(pictureBox1_Click);
            this.btnContornos.Click += new EventHandler(btnContornos_Click);
            this.btnComentariosComanda.Click += new EventHandler(btnComentariosComanda_Click);
            this.EnviarComandaComboBoxEdit.Properties.Items.AddRange(FactoryPlatos.getArrayEnviarComanda());
            this.btnCargarReceta.Click += new EventHandler(btnCargarReceta_Click);
            #region Ingredientes            
            txtIngrediente.Validating += new CancelEventHandler(txtIngrediente_Validating);
            txtIngrediente.ButtonClick+=new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(txtIngrediente_ButtonClick);
            txtCantidad.Validating+=new CancelEventHandler(txtCantidad_Validating);
            txtCostoRacion.Validating+=new CancelEventHandler(txtCostoRacion_Validating);    
            this.gridControl1.KeyDown+=new KeyEventHandler(gridControl1_KeyDown);
            #endregion
        }
        void btnCargarReceta_Click(object sender, EventArgs e)
        {
            Plato p = new Plato();
            FrmBuscarEntidades f = new FrmBuscarEntidades();
            f.BuscarPlatos("");
            if (f.DialogResult != System.Windows.Forms.DialogResult.OK)
                return;
            p = (Plato)f.registro;
            foreach (PlatosIngrediente item in p.PlatosIngredientes)
            {
                PlatosIngrediente nuevo = new PlatosIngrediente();
                nuevo.Cantidad = item.Cantidad;
                nuevo.CostoRacion = item.CostoRacion;
                nuevo.IdIngrediente = item.IdIngrediente;
                nuevo.Ingrediente = item.Ingrediente;
                nuevo.Raciones = item.Raciones;
                nuevo.Total = item.Total;
                nuevo.UnidadMedida = item.UnidadMedida;
                nuevo.IdPlatoIngrediente = FactoryContadores.GetMax("IdPlatoIngrediente");
                registro.PlatosIngredientes.Add(nuevo);
            }
            this.platosIngredienteBindingSource.ResetBindings(true);
        }
        void PrecioCalcEdit_Validating(object sender, CancelEventArgs e)
        {
            platoBindingSource.EndEdit();
            registro.PrecioConIva = registro.Precio.GetValueOrDefault(0) + (registro.Precio.GetValueOrDefault(0) * registro.TasaIva.GetValueOrDefault(0) / 100);
        }
        void btnComentariosComanda_Click(object sender, EventArgs e)
        {
            FrmPlatosItemComentarios f = new FrmPlatosItemComentarios();
            f.plato = registro;
            f.ShowDialog();
        }
        void btnContornos_Click(object sender, EventArgs e)
        {
            FrmPlatosItemContornos f = new FrmPlatosItemContornos();
            f.plato = registro;
            f.ShowDialog();
        }
        void pictureBox1_Click(object sender, EventArgs e)
        {
            FileDialog f = new OpenFileDialog();
            f.AddExtension = true;
            f.DefaultExt = "jpg";
            f.Filter = "jpg files (*.jpg)|*.jpg|todos (*.*)|*.*";
            f.Title = "Seleccione la foto";
            if (f.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                if (f.FileName == string.Empty)
                    return;
                Image imagen = new System.Drawing.Bitmap((Image)Image.FromFile(f.FileName));
                System.IO.File.Delete(Application.StartupPath + "\\Imagenes\\" + registro.Codigo + ".jpg");
                System.IO.File.Copy(f.FileName, Application.StartupPath + "\\Imagenes\\" + registro.Codigo + ".jpg");
                this.pictureBox1.Image= LeerImagen(registro.Codigo);
            }
        }
        void PrecioConIvaCalcEdit_Validating(object sender, CancelEventArgs e)
        {
            platoBindingSource.EndEdit();
            registro.Precio = registro.PrecioConIva.GetValueOrDefault(0) /  (1 + (registro.TasaIva.GetValueOrDefault(0) / 100));
        }
        private void Limpiar()
        {
            registro = new Plato();
            registro.TasaIva = Basicas.parametros().TasaIva;
            registro.Precio = 0;
            registro.PrecioConIva = 0;
        }
        public void Incluir()
        {
            esNuevo = true;
            Limpiar();
            Enlazar();
            this.ShowDialog();
        }
        public void Modificar()
        {
            registro = FactoryPlatos.Item(db, registro.IdPlato);
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
            this.platoBindingSource.DataSource = registro;
            this.platoBindingSource.ResetBindings(true);
            this.platosIngredienteBindingSource.DataSource=registro.PlatosIngredientes;
            this.pictureBox1.Image = LeerImagen(registro.Codigo);
        }
        private void Aceptar_Click(object sender, EventArgs e)
        {
            try
            {
                platoBindingSource.EndEdit();
                registro = (Plato)platoBindingSource.Current;
                if (string.IsNullOrEmpty(registro.Codigo))
                    throw new Exception("Error codigo no puede estar vacio");
                if (string.IsNullOrEmpty(registro.Grupo))
                    throw new Exception("Error el grupo no puede estar vacio");
                if (registro.Codigo.Length>6)
                    throw new Exception("Error codigo no puede tener mas de 6 caracteres");
                if (registro.Grupo.Length>20)
                    throw new Exception("Error grupo no puede tener mas de 20 caracteres");
                this.DialogResult = DialogResult.OK;
                if (esNuevo)
                {
                    if (registro.IdPlato == null)
                    {
                        registro.IdPlato = FactoryContadores.GetMax("IdPlato");
                    }
                }
                foreach (PlatosIngrediente p in registro.PlatosIngredientes)
                {
                    if (p.IdPlatoIngrediente == null)
                    {
                        p.IdPlatoIngrediente = FactoryContadores.GetMax("IdPlatoIngrediente");
                    }
                }
                foreach (PlatosContorno p in registro.PlatosContornos)
                {
                    if (p.IdPlatoContorno == null)
                    {
                        p.IdPlatoContorno = FactoryContadores.GetMax("IdPlatoContorno");
                    }
                }
                foreach (PlatosComentario p in registro.PlatosComentarios)
                {
                    if (p.IdPlatoComentario == null)
                    {
                        p.IdPlatoComentario = FactoryContadores.GetMax("IdPlatoComentario");
                    }
                }
                if (esNuevo)
                {
                    db.Platos.AddObject(registro);
                }
                db.SaveChanges();
                this.Close();
            }
            catch (Exception ex)
            {
                string s = ex.Message;                
                if (ex.InnerException != null)
                {
                    s = ex.InnerException.Message;
                }
                else
                {
                    MessageBox.Show("Error al guardar los datos:" + s, "Atencion", MessageBoxButtons.OK);
                }
            }
        }
        private void Cancelar_Click(object sender, EventArgs e)
        {
            this.platoBindingSource.ResetCurrentItem();
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
        private void Frm_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.F3:
                    this.btnCargarReceta.PerformClick();
                    e.Handled = true;
                    break;
                case Keys.Escape:
                    this.Cancelar.PerformClick();
                    e.Handled = true;
                    break;
                case Keys.F12:
                    this.Aceptar.PerformClick();
                    e.Handled = true;
                    break;
            }
        }
        Image LeerImagen(string codigo)
        {
            try
            {
                string archivo = Application.StartupPath + "\\Imagenes\\" + codigo + ".jpg";
                System.Drawing.Bitmap imagen = new System.Drawing.Bitmap((Image)Image.FromFile(
                                                    Application.StartupPath + "\\Imagenes\\" + codigo + ".jpg"));
                Bitmap b = new Bitmap(imagen);
                Basicas.DisposeImage(imagen);
                return Image.FromHbitmap(b.GetHbitmap()).GetThumbnailImage(80, 80, null, IntPtr.Zero);
            }
            catch
            {
                return null;
            }
        }
        private void EscribirImagen(Image image)
        {
            try
            {
                image.Dispose();
                try
                {
                    System.IO.File.Delete(Application.StartupPath + "\\Imagenes\\" + registro.Codigo + ".jpg");
                }
                catch (Exception x)
                {
                    throw x;
                }
                image.Save(Application.StartupPath + "\\Imagenes\\" + registro.Codigo + ".jpg");
            }
            catch (System.Runtime.InteropServices.ExternalException ix)
            {
                throw ix;
            }
            catch (Exception x2)
            {
                throw x2;
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
                        PlatosIngrediente i = (PlatosIngrediente)this.platosIngredienteBindingSource.Current;
                        if (i == null)
                            return;
                        registro.PlatosIngredientes.Remove(i);
                        this.platosIngredienteBindingSource.DataSource = registro.PlatosIngredientes;
                    }
                    e.Handled = true;
                }
            }
        }
        private void txtIngrediente_Validating(object sender, CancelEventArgs e)
        {
            DevExpress.XtraEditors.TextEdit Editor = (DevExpress.XtraEditors.TextEdit)this.gridControl1.MainView.ActiveEditor;
            if (!Editor.IsModified)
                return;
            string Texto = Editor.Text;
            platoIngrediente = (PlatosIngrediente)this.platosIngredienteBindingSource.Current;
            if (UbicarProducto(Texto))
            {
                if (ingrediente == null)
                {
                    Editor.Undo();
                    e.Cancel = true;
                    return;
                }
                LeerIngrediente(false);
                Editor.Text = ingrediente.Descripcion;
            }
            else
            {
                LeerIngrediente(false);
                Editor.Undo();
            }
        }
        void txtIngrediente_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            platoIngrediente = (PlatosIngrediente)this.platosIngredienteBindingSource.Current;
            UbicarProducto("");
            LeerIngrediente(false);
        }
        private void LeerIngrediente(bool Buscar)
        {
            if (ingrediente == null)
            {
                return;
            }
            platoIngrediente = (PlatosIngrediente)this.platosIngredienteBindingSource.Current;
            platoIngrediente.Cantidad = 1;
            platoIngrediente.CostoRacion = ingrediente.Costo ;
            platoIngrediente.IdIngrediente = ingrediente.IdIngrediente;
            platoIngrediente.Ingrediente = ingrediente.Descripcion;
            platoIngrediente.Raciones = 1;
            platoIngrediente.UnidadMedida = ingrediente.UnidadMedida;
            platoIngrediente.Total = platoIngrediente.Cantidad * platoIngrediente.CostoRacion;
            platoIngrediente.UnidadMedida = ingrediente.UnidadMedida;
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
                    if (ingrediente == null)
                    {
 
                    }
                    break;
            }
            LeerIngrediente(false);
            return true;
        }
        private void txtCostoRacion_Validating(object sender, CancelEventArgs e)
        {
            DevExpress.XtraEditors.CalcEdit Editor = (DevExpress.XtraEditors.CalcEdit)this.gridControl1.MainView.ActiveEditor;
            if (!Editor.IsModified)
                return;
            platoIngrediente.Total = platoIngrediente.Cantidad * (double)Editor.Value;
        }
        private void txtCantidad_Validating(object sender, CancelEventArgs e)
        {
            DevExpress.XtraEditors.CalcEdit Editor = (DevExpress.XtraEditors.CalcEdit)this.gridControl1.MainView.ActiveEditor;
            if (!Editor.IsModified)
                return;
            platoIngrediente = (PlatosIngrediente)this.platosIngredienteBindingSource.Current;
            platoIngrediente.Total = (double)(double)Editor.Value * platoIngrediente.CostoRacion;
        }
        #endregion
    }
}
