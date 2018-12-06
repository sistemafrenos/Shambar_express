using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using HK.Clases;
using HK.Formas;

namespace HK
{
    public partial class FrmPagar : Form
    {
        public Factura factura = new Factura();
        public Cliente cliente = new Cliente();
        public FrmPagar()
        {
            InitializeComponent();
            this.Load += new EventHandler(FrmPagar_Load);
            this.Aceptar.Click += new EventHandler(Aceptar_Click);
            this.Cancelar.Click += new EventHandler(Cancelar_Click);
            this.KeyDown += new KeyEventHandler(FrmPagar_KeyDown);
            this.EfectivoTextEdit.Enter += new EventHandler(Editor_Enter);
            this.ChequeTextEdit.Enter += new EventHandler(Editor_Enter);
            this.TarjetaTextEdit.Enter += new EventHandler(Editor_Enter);
            this.CestaTicketTextEdit.Enter += new EventHandler(Editor_Enter);
            this.EfectivoTextEdit.Validating += new CancelEventHandler(EfectivoTextEdit_Validating);
            this.ChequeTextEdit.Validating += new CancelEventHandler(ChequeTextEdit_Validating);
            this.CestaTicketTextEdit.Validating += new CancelEventHandler(CestaTicketTextEdit_Validating);
            this.TarjetaTextEdit.Validating += new CancelEventHandler(TarjetaTextEdit_Validating);
            this.Efectivo.Click += new EventHandler(Efectivo_Click);
            this.Cheque.Click += new EventHandler(Cheque_Click);
            this.Tarjeta.Click += new EventHandler(Tarjeta_Click);
            this.CestaTicket.Click += new EventHandler(CestaTicket_Click);
            this.ConsumoInterno.Click += new EventHandler(ConsumoInterno_Click);
            this.txtCedulaRif.Validating += new CancelEventHandler(txtCedulaRif_Validating);
            this.txtCedulaRif.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(txtCedulaRif_ButtonClick);
            this.KeyPreview = true;
        }
        void txtCedulaRif_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            FrmBuscarEntidades F = new FrmBuscarEntidades();
            F.BuscarClientes("");
            if (F.registro != null)
            {
                cliente = (Cliente)F.registro;
                cliente = FactoryClientes.Item(cliente.CedulaRif);
                factura.CedulaRif = cliente.CedulaRif;
                LeerCliente();
            }
            else
            {
                cliente = new Cliente();
                cliente.CedulaRif = "";
                LeerCliente();
            }
        }
        void txtCedulaRif_Validating(object sender, CancelEventArgs e)
        {
            DevExpress.XtraEditors.TextEdit Editor = (DevExpress.XtraEditors.TextEdit)sender;
            if (!Editor.IsModified)
                return;
            string Texto = Editor.Text;
            this.facturaBindingSource.EndEdit();
            List<Cliente> T = FactoryClientes.getItems(Texto);
            switch (T.Count)
            {
                case 0:
                    cliente = new Cliente();
                    cliente.CedulaRif = Basicas.CedulaRif(Texto);
                    break;
                case 1:
                    cliente = T[0];
                    break;
                default:
                    FrmBuscarEntidades F = new FrmBuscarEntidades();
                    F.BuscarClientes(Texto);
                    if (F.registro != null)
                    {
                        cliente = (Cliente)F.registro;
                        cliente = FactoryClientes.Item(cliente.CedulaRif);
                    }
                    else
                    {
                        cliente = null;
                    }
                    break;
            }
            LeerCliente();
        }
        private void LeerCliente()
        {
            factura.CedulaRif = cliente.CedulaRif;
            factura.RazonSocial = cliente.RazonSocial;
            factura.Direccion = cliente.Direccion;
            this.facturaBindingSource.ResetCurrentItem();
        }
        void ConsumoInterno_Click(object sender, EventArgs e)
        {
            PagarConsumoInterno();
        }

        void CestaTicket_Click(object sender, EventArgs e)
        {
            PagarCestaTicket();
        }

        void Tarjeta_Click(object sender, EventArgs e)
        {
            PagarTarjeta();
        }

        void Cheque_Click(object sender, EventArgs e)
        {
            PagarCheque();
        }

        void Efectivo_Click(object sender, EventArgs e)
        {
            PagarEfectivo();
        }

        void TarjetaTextEdit_Validating(object sender, CancelEventArgs e)
        {
            factura.calcularSaldo();
            DevExpress.XtraEditors.CalcEdit editor = (DevExpress.XtraEditors.CalcEdit)sender;
            factura.Tarjeta = (double)editor.Value;
            this.facturaBindingSource.ResetCurrentItem();
        }

        void CestaTicketTextEdit_Validating(object sender, CancelEventArgs e)
        {
            DevExpress.XtraEditors.CalcEdit editor = (DevExpress.XtraEditors.CalcEdit)sender;
            factura.CestaTicket = (double)editor.Value;
            this.facturaBindingSource.ResetCurrentItem();
        }

        void ChequeTextEdit_Validating(object sender, CancelEventArgs e)
        {
            DevExpress.XtraEditors.CalcEdit editor = (DevExpress.XtraEditors.CalcEdit)sender;
            factura.Cheque = (double)editor.Value;
            this.facturaBindingSource.ResetCurrentItem();
        }

        void EfectivoTextEdit_Validating(object sender, CancelEventArgs e)
        {
            DevExpress.XtraEditors.CalcEdit editor = (DevExpress.XtraEditors.CalcEdit)sender;
            factura.Efectivo = (double)editor.Value;
            this.facturaBindingSource.ResetCurrentItem();
        }

        void Editor_Enter(object sender, EventArgs e)
        {
            DevExpress.XtraEditors.CalcEdit editor = (DevExpress.XtraEditors.CalcEdit)sender;
            editor.Value = (decimal)factura.Saldo;
            editor.SelectAll();
        }
        void FrmPagar_KeyDown(object sender, KeyEventArgs e)
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
                case Keys.F4:
                    PagarEfectivo();
                    e.Handled = true;
                    break;
                case Keys.F5:
                    PagarTarjeta();
                    e.Handled = true;
                    break;
                case Keys.F6:
                    PagarCestaTicket();
                    e.Handled = true;
                    break;
                case Keys.F7:
                    PagarCheque();
                    e.Handled = true;
                    break;
                case Keys.F8:
                    PagarConsumoInterno();
                    e.Handled = true;
                    break;
            } 
        }

        private void PagarCheque()
        {
            LimpiarPagos(Basicas.parametros().TasaIva);
            factura.calcularSaldo();
            ChequeTextEdit.Value = (decimal)factura.Saldo;
            ChequeTextEdit.SelectAll();
            this.ChequeTextEdit.Focus();
        }

        private void PagarCestaTicket()
        {
            LimpiarPagos(Basicas.parametros().TasaIva);
            factura.calcularSaldo();
            CestaTicketTextEdit.Value = (decimal)factura.Saldo;
            CestaTicketTextEdit.SelectAll();
            this.CestaTicketTextEdit.Focus();
        }

        private void PagarTarjeta()
        {
            LimpiarPagos(Basicas.parametros().TasaIva);
            TarjetaTextEdit.Value = (decimal)factura.Saldo;
            TarjetaTextEdit.SelectAll();
            this.TarjetaTextEdit.Focus();
        }

        private void PagarEfectivo()
        {
            LimpiarPagos(Basicas.parametros().TasaIva);
            factura.calcularSaldo();
            EfectivoTextEdit.Value = (decimal)factura.Saldo;
            EfectivoTextEdit.SelectAll();
            this.EfectivoTextEdit.Focus();
        }

        private void PagarConsumoInterno()
        {            
            FrmLogin f = new FrmLogin();
            f.TipoUsuario = "ADMINISTRADOR";
            f.ShowDialog();
            if (f.DialogResult != System.Windows.Forms.DialogResult.OK)
                return;
            if (FactoryUsuarios.UsuarioActivo.PuedeDarConsumoInterno.GetValueOrDefault(false) != true)
            {
                return;
            }
            this.SaldoTextEdit.Focus();
            LimpiarPagos();
            factura.ConsumoInterno = factura.MontoTotal;
            factura.Totalizar();
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Close();
        }

        private void LimpiarPagos(double? tasaIva = 12)
        {
            factura.Cheque = null;
            factura.Tarjeta = null;
            factura.CestaTicket = null;
            factura.Efectivo = null;
            factura.ConsumoInterno = null;
            factura.Totalizar(tasaIva);
            factura.calcularSaldo();
        }

        void Cancelar_Click(object sender, EventArgs e)
        {
            LimpiarPagos();
            factura.Totalizar();
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Close();
        }
        void Aceptar_Click(object sender, EventArgs e)
        {

            facturaBindingSource.EndEdit();
            factura.calcularSaldo();
            if (factura.Efectivo.HasValue)
            {
                if (factura.Cambio.GetValueOrDefault(0) > factura.Efectivo.GetValueOrDefault(0))
                {
                    MessageBox.Show("El cambio no puede ser mayor al monto en efectivo", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                factura.Efectivo = factura.Efectivo.GetValueOrDefault(0) - factura.Cambio.GetValueOrDefault(0);
            }
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Close();
        }
        void FrmPagar_Load(object sender, EventArgs e)
        {
            this.facturaBindingSource.DataSource = factura;
            this.facturaBindingSource.ResetBindings(true);
        }
    }
}
