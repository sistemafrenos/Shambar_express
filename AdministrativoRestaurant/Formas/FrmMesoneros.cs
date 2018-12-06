using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using HK.Clases;

namespace HK.Formas
{
    public partial class FrmMesoneros : Form
    {
        public FrmMesoneros()
        {
            InitializeComponent();
            this.Load += new EventHandler(FrmMesoneros_Load);
            this.FormClosed += new FormClosedEventHandler(FrmMesoneros_FormClosed);
        }
        void FrmMesoneros_FormClosed(object sender, FormClosedEventArgs e)
        {
            Pantallas.MesonerosLista = null;
        }
        void FrmMesoneros_Load(object sender, EventArgs e)
        {
            this.gridControl1.KeyDown += new KeyEventHandler(gridControl1_KeyDown);
            this.gridControl1.DoubleClick += new EventHandler(gridControl1_DoubleClick);
            this.btnNuevo.Click += new EventHandler(btnNuevo_Click);
            this.btnEditar.Click += new EventHandler(btnEditar_Click);
            this.btnEliminar.Click += new EventHandler(btnEliminar_Click);
            this.btnBuscar.Click += new EventHandler(btnBuscar_Click);
            this.txtBuscar.Validating += new CancelEventHandler(txtBuscar_Validating);
            this.btnImprimir.Click += new EventHandler(btnImprimir_Click);
            Busqueda();
        }
        FeriaEntities db = new FeriaEntities();
        List<Usuario> Lista = new List<Usuario>();
        #region toobar
        void btnImprimir_Click(object sender, EventArgs e)
        {
            FrmReportes f = new FrmReportes();
            f.ListadoMesoneros(Lista);
            f = null;
        }
        void txtBuscar_Validating(object sender, CancelEventArgs e)
        {
            Busqueda();
        }
        void btnBuscar_Click(object sender, EventArgs e)
        {
            Busqueda();
        }
        void btnEditar_Click(object sender, EventArgs e)
        {
            EditarRegistro();
        }
        void btnNuevo_Click(object sender, EventArgs e)
        {
            AgregarRegistro();
        }
        void btnEliminar_Click(object sender, EventArgs e)
        {
            EliminarRegistro();
        }
        void Buscar_Click(object sender, EventArgs e)
        {
            Busqueda();
        }
        private void gridControl1_KeyDown(object sender, KeyEventArgs e)
        {
            if (gridView1.ActiveEditor == null)
            {
                if (e.KeyCode == Keys.Return)
                {
                    EditarRegistro();
                }
                if (e.KeyCode == Keys.Delete || e.KeyCode == Keys.Subtract)
                {
                    EliminarRegistro();
                }
                if (e.KeyCode == Keys.Insert)
                {
                    AgregarRegistro();
                }
            }
        }
        private void gridControl1_DoubleClick(object sender, EventArgs e)
        {
            EditarRegistro();
        }
        #endregion
        private void Busqueda()
        {
            db = new FeriaEntities();
            Lista = FactoryUsuarios.getItems(db, this.txtBuscar.Text,"MESONERO");
            this.bs.DataSource = Lista;
            this.bs.ResetBindings(true);
        }
        private void AgregarRegistro()
        {
            FrmMesonerosItem F = new FrmMesonerosItem();
            F.Incluir();
            if (F.DialogResult == DialogResult.OK)
            {
                try
                {
                    F.registro.Activo = true;
                    F.registro.IdUsuario = FactoryContadores.GetMax("IdUsuario");
                    db.Usuarios.AddObject(F.registro);
                    db.SaveChanges();
                }
                catch (System.Data.OptimisticConcurrencyException x)
                {
                    MessageBox.Show("Error al guardar los datos:\n" + x.InnerException.Message, "Atencion", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                Busqueda();
            }
        }
        private void EditarRegistro()
        {
            FrmMesonerosItem F = new FrmMesonerosItem();
            Usuario registro = (Usuario)this.bs.Current;
            if (registro == null)
                return;
            F.registro = registro;
            F.Modificar();
            if (F.DialogResult == DialogResult.OK)
            {
                try
                {
                    db.SaveChanges();
                }
                catch (System.Data.OptimisticConcurrencyException x)
                {
                    MessageBox.Show("Error al guardar los datos:\n" + x.InnerException.Message, "Atencion", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                db.Refresh(System.Data.Objects.RefreshMode.StoreWins, registro);
            }
        }
        private void EliminarRegistro()
        {
            if (this.gridView1.IsFocusedView)
            {
                Usuario Registro = (Usuario)this.bs.Current;
                if (Registro == null)
                    return;
                if (MessageBox.Show("Esta seguro de eliminar este registro", "Atencion", MessageBoxButtons.YesNo) != DialogResult.Yes)
                {
                    return;
                }
                try
                {
                    Registro.Activo = false;
                    db.SaveChanges();
                    Busqueda();
                }
                catch (Exception x)
                {
                    MessageBox.Show(x.Message);
                }
            }
        }
    }
}
