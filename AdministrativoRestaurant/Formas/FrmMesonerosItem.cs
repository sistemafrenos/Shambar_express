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
    public partial class FrmMesonerosItem : Form
    {
        public FrmMesonerosItem()
        {
            InitializeComponent();
            this.Load += new EventHandler(FrmUsuariosItem_Load);
        }

        void FrmUsuariosItem_Load(object sender, EventArgs e)
        {
            this.KeyPreview = true;
            this.Aceptar.Click += new EventHandler(Aceptar_Click);
            this.Cancelar.Click += new EventHandler(Cancelar_Click);
            this.KeyDown += new KeyEventHandler(FrmMesasItem_KeyDown);
            this.CedulaTextEdit.Validating += new CancelEventHandler(CedulaTextEdit_Validating);
        }

        void CedulaTextEdit_Validating(object sender, CancelEventArgs e)
        {
            this.CedulaTextEdit.Text = Basicas.CedulaRif(this.CedulaTextEdit.Text);
        }
        public Usuario registro = new Usuario();
        private void Limpiar()
        {
            registro = new Usuario();
            registro.TipoUsuario = "MESONERO";
            registro.PuedeDarConsumoInterno = false;
            registro.PuedePedirCorteDeCuenta = false;
            registro.PuedeSepararCuentas = false;
            registro.Puntos = 1;
        }
        public void Incluir()
        {
            Limpiar();
            Enlazar();
            this.ShowDialog();
        }
        public void Modificar()
        {
            Enlazar();
            this.ShowDialog();
        }
        private void Enlazar()
        {
            if (registro == null)
            {
                Limpiar();
            }
            this.mesoneroBindingSource.DataSource = registro;
            this.mesoneroBindingSource.ResetBindings(true);
        }
        private void Aceptar_Click(object sender, EventArgs e)
        {
            try
            {
                mesoneroBindingSource.EndEdit();
                registro = (Usuario)mesoneroBindingSource.Current;
                FactoryUsuarios.Validar(registro);
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al validar los datos \n" + ex.Message, "Atencion", MessageBoxButtons.OK);
            }
        }
        private void Cancelar_Click(object sender, EventArgs e)
        {
            this.mesoneroBindingSource.ResetCurrentItem();
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
        private void FrmMesasItem_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
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
    }
}
