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
    public partial class FrmEditarMesa : Form
    {
        int cantidad = 1;
        List<Button> grupos = new List<Button>();
        List<Button> platos = new List<Button>();
        List<Button> cantidades = new List<Button>();
        List<Plato> mplatos = new List<Plato>();        
        Usuario mesonero = new Usuario();
        Factura factura = new Factura();
        Cliente cliente = null;
        private bool esNuevo = false;
        FeriaEntities db = new FeriaEntities();
        public Mesa mesa = new Mesa();
        public MesasAbierta mesaAbierta = null;
        public FrmEditarMesa()
        {
            InitializeComponent();
            this.Load+=new EventHandler(FrmEditarMesa_Load);
        }
        void  FrmEditarMesa_Load(object sender, EventArgs e)
        {
            cantidades.AddRange(new Button[] { cantidad0, cantidad1, cantidad2, cantidad3, cantidad4, cantidad5, cantidad6, cantidad7, cantidad8 });
            grupos.AddRange(new Button[] { grupo0, grupo1, grupo2, grupo3, grupo4, grupo5, grupo6, grupo7, grupo8, grupo9 });
            platos.AddRange(new Button[] { plato0, plato1, plato2, plato3, plato4, plato5, plato6, plato7, plato8, plato9, plato10, plato11, plato12, plato13, plato14, plato15, plato16, plato17, plato18, plato19, plato20, plato21, plato22, plato23, plato24, plato25, plato26, plato27, plato28, plato29 });
            foreach (Button b in grupos)
            {
                b.Visible = false;
                b.Font = new System.Drawing.Font("Verdana", 9, FontStyle.Bold);
            }
            OcultarPlatos();
            foreach (Button b in grupos)
            {
                b.Click += new EventHandler(grupo_Click);                
            }
            foreach (Button b in platos)
            {
                b.Click += new EventHandler(plato_Click);
            }
            foreach (Button b in cantidades)
            {
                b.Click += new EventHandler(cantidad_Click);
            }
            CargarGrupos();
            if (mesaAbierta == null)
            {
                esNuevo = true;
                mesaAbierta = new MesasAbierta();
                mesaAbierta.IdMesa = mesa.IdMesa;
                mesaAbierta.Mesa = mesa.Descripcion;
                mesaAbierta.Apertura = DateTime.Now;
                mesaAbierta.Estatus = "ABIERTA";
                mesaAbierta.Personas = 1;
                if (FactoryUsuarios.MesoneroActivo == null)
                {
                    var q = from p in db.Usuarios
                            where p.TipoUsuario == "MESONERO"
                            select p;
                    if (q.FirstOrDefault() == null)
                    {
                        mesonero = FactoryUsuarios.CrearUsuario("MESONERO");
                        db.Usuarios.AddObject(mesonero);
                        db.SaveChanges();
                    }
                    FactoryUsuarios.MesoneroActivo = mesonero;
                }
                mesaAbierta.IdMesonero = FactoryUsuarios.MesoneroActivo.IdUsuario;
                mesaAbierta.Mesonero = FactoryUsuarios.MesoneroActivo.Nombre;
                mesonero = FactoryUsuarios.MesoneroActivo;
            }
            else
            {
                mesonero = FactoryUsuarios.Item(mesaAbierta.IdMesonero);
                mesaAbierta = FactoryMesas.MesaAbiertaItem(db, mesaAbierta);
            }
            mesasAbiertaBindingSource.DataSource = mesaAbierta;
            mesasAbiertaBindingSource.ResetBindings(true);
            mesasAbiertasPlatoBindingSource.DataSource = mesaAbierta.MesasAbiertasPlatos;
            mesasAbiertasPlatoBindingSource.ResetBindings(true);
            this.gridControl1.KeyDown += new KeyEventHandler(gridControl1_KeyDown);
            this.Pagos.Click += new EventHandler(Pagos_Click);
            this.Cancelar.Click += new EventHandler(Salir_Click);
            this.Aceptar.Click+=new EventHandler(Aceptar_Click);
            this.Corte.Click += new EventHandler(Corte_Click);
            this.KeyPreview = true;
            this.KeyDown += new KeyEventHandler(FrmCaja_KeyDown);
            this.txtPlato.Validating += new CancelEventHandler(txtPlato_Validating);
            this.txtPlato.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(txtPlato_ButtonClick);
            MesaButtonEdit.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(MesaButtonEdit_ButtonClick);
            MesoneroButtonEdit.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(MesoneroButtonEdit_ButtonClick);
            this.Height = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height - 120;
            this.Width = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Width - 70;
            this.CenterToScreen();
            txtPlato.Focus();
        }

        void Corte_Click(object sender, EventArgs e)
        {
            FiscalBixolon f = new FiscalBixolon();
            f.ImprimeCorte(mesaAbierta);
        }        
        void MesoneroButtonEdit_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            FrmBuscarEntidades F = new FrmBuscarEntidades();
            F.BuscarMesoneros("");
            if (F.registro != null)
            {
                mesonero = (Usuario)F.registro;
            }
            else
            {
                mesonero = new Usuario();
            }
            this.MesoneroButtonEdit.Text = mesonero.Nombre;
        }
        void MesaButtonEdit_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            FrmBuscarEntidades F = new FrmBuscarEntidades();
            F.BuscarMesas("");
            if (F.registro != null)
            {
                mesa = (Mesa)F.registro;
            }
            else
            {
                mesa = new Mesa();
            }
            this.MesaButtonEdit.Text = mesa.Descripcion;
        }
        void  Aceptar_Click(object sender, EventArgs e)
        {
            try
            {
            Validar();
            mesaAbierta.Totalizar();
            Guardar();
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Close();
            }
            catch(Exception x)
            {
                MessageBox.Show(x.Message);
            }
        }
        void Salir_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Close();
        }
        void Pagos_Click(object sender, EventArgs e)
        {
            this.mesasAbiertasPlatoBindingSource.EndEdit();
            try
            {
                Validar();
                CargarFactura();
                FrmPagar pago = new FrmPagar();
                pago.factura = factura;
                pago.ShowDialog();
                if (pago.DialogResult != System.Windows.Forms.DialogResult.OK)
                {
                    return;
                }
                factura.calcularSaldo();
                if(factura.Cambio.GetValueOrDefault()>1)
                {
                    txtCambio.Value = (decimal)factura.Cambio;
                    this.txtCambio.Visible = true;
                    Application.DoEvents();
                }
                if (decimal.Round( (decimal)factura.Saldo.GetValueOrDefault(0),0)==0)
                {
                    if (factura.ConsumoInterno.GetValueOrDefault(0) == 0)
                    {
                        factura.Tipo = "FACTURA";
                        ImprimirFactura();
                    }
                    else
                    {
                        factura.Tipo = "CONSUMO";
                        factura.Hora = DateTime.Now;
                        factura.Fecha = DateTime.Today;                        
                        factura.Numero = FactoryContadores.GetMax("Consumo");
                    }
                    GuardarFactura();
                    EliminarMesaAbierta(mesaAbierta);
                    this.txtCambio.Visible = false;
                }
                else
                {
                    return;
                }
            }
            catch (Exception x)
            {
                MessageBox.Show(x.Message);
                this.txtCambio.Visible = false;
                return;
            }
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Close();
        }
        private void EliminarMesaAbierta(MesasAbierta mesaAbierta)
        {
            using (var db = new FeriaEntities())
            {
                try
                {
                    MesasAbierta m = (from x in db.MesasAbiertas
                                      where x.IdMesaAbierta == mesaAbierta.IdMesaAbierta
                                      select x).FirstOrDefault();                               
                    db.MesasAbiertas.DeleteObject(m);
                    db.SaveChanges();
                }
                catch (Exception x)
                {
                    MessageBox.Show(x.Message);
                }
            }
        }
        private void CargarFactura()
        {
            if (FactoryUsuarios.CajeroActivo == null)
            {
                FrmLogin f = new FrmLogin();
                f.ShowDialog();
                if (f.DialogResult != System.Windows.Forms.DialogResult.OK)
                {
                    return;
                }
            }
            factura = new Factura();
            factura.Cajero = FactoryUsuarios.CajeroActivo.Nombre;
            factura.IdCajero = FactoryUsuarios.CajeroActivo.IdUsuario;
            if (cliente == null)
            {
                cliente = new Cliente();
                cliente.CedulaRif = "V000000000";
                cliente.RazonSocial = "CONTADO";
                cliente.Direccion = Basicas.parametros().Ciudad;
            }
            factura.CedulaRif = cliente.CedulaRif;
            factura.Direccion = cliente.Direccion;
            factura.RazonSocial = cliente.RazonSocial;
            factura.Tipo = "FACTURA";
            foreach (MesasAbiertasPlato item in mesaAbierta.MesasAbiertasPlatos)
            {
                FacturasPlato nuevo = new FacturasPlato();
                nuevo.Cantidad = item.Cantidad;
                nuevo.Codigo = item.Codigo;
                nuevo.Comentarios = item.Comentarios;
                nuevo.Contornos = item.Contornos;
                nuevo.Descripcion = item.Descripcion;
                nuevo.Grupo = item.Grupo;
                nuevo.Idplato = item.Idplato;
                nuevo.Precio = item.Precio;
                nuevo.PrecioConIva = item.PrecioConIva;
                nuevo.TasaIva = item.TasaIva;
                nuevo.Total = item.Total;
                factura.FacturasPlatos.Add(nuevo);
            }
            factura.Totalizar();
        }
        void txtPlato_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            DevExpress.XtraEditors.TextEdit Editor = (DevExpress.XtraEditors.TextEdit)sender;
            string Texto = Editor.Text;
            Editor.Text = "";
            Plato plato = new Plato();
            FrmBuscarEntidades F = new FrmBuscarEntidades();
            F.BuscarPlatos(Texto);
            if (F.registro != null)
            {
                plato = (Plato)F.registro;
            }
            else
            {
                return;
            }
        }
        void txtPlato_Validating(object sender, CancelEventArgs e)
        {
            DevExpress.XtraEditors.TextEdit Editor = (DevExpress.XtraEditors.TextEdit)sender;
            if (!Editor.IsModified)
                return;
            Plato plato = new Plato();
            string Texto = Editor.Text;
            Editor.Text = "";
            List<Plato> T = FactoryPlatos.getItems(Texto);
            switch (T.Count)
            {
                case 0:
                    return;
                case 1:
                    plato = T[0];
                    break;
                default:
                    FrmBuscarEntidades F = new FrmBuscarEntidades();
                    F.BuscarPlatos(Texto);
                    if (F.registro != null)
                    {
                        plato = (Plato)F.registro;
                    }
                    else
                    {
                        return;
                    }
                    break;
            }
            AgregarItem(plato);            
        }
        void FrmCaja_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.F8:
                    MesasAbiertasPlato ultimo = mesaAbierta.MesasAbiertasPlatos.LastOrDefault();
                    EliminarItem(ultimo);
                    e.Handled = true;
                    break;
                case Keys.F10:
                    this.Corte.PerformClick();
                    e.Handled = true;
                    break;
                case Keys.F12:
                    this.Aceptar.PerformClick();
                    e.Handled = true;
                    break;
                case Keys.Escape:
                    this.Cancelar.PerformClick();
                    e.Handled = true;
                    break;
                case Keys.F4:
                    this.Pagos.PerformClick();
                    e.Handled = true;
                    break;
            } 
        }
        private void Validar()
        {
            mesasAbiertaBindingSource.EndEdit();
            mesaAbierta.Totalizar();
            if (mesaAbierta.MesasAbiertasPlatos.Count == 0)
                throw new Exception("Mesas Abierta sin platos");
            if (string.IsNullOrEmpty( mesa.IdMesa))
            {
                throw new Exception("Mesa Erronea o Inexistente");
            }
            if (string.IsNullOrEmpty(mesaAbierta.IdMesonero))
            {
                throw new Exception("Mesonero Erroneo o Inexistente");
            }
            mesaAbierta.IdMesa = mesa.IdMesa;
            mesaAbierta.Mesa = mesa.Descripcion;
            mesaAbierta.IdMesonero = mesonero.IdUsuario;
            mesaAbierta.Mesonero = mesonero.Nombre;            
        }
        private void Guardar()
        {
            {
                if (mesaAbierta.IdMesaAbierta==null)
                {
                    esNuevo = true;
                    mesaAbierta.IdMesaAbierta = FactoryContadores.GetMax("IdMesaAbierta");
                }
                foreach (HK.MesasAbiertasPlato p in mesaAbierta.MesasAbiertasPlatos)
                {
                    if (p.IdMesaAbiertaPlato == null)
                    {
                        p.IdMesaAbiertaPlato = FactoryContadores.GetMax("IdMesaAbiertaPlato");
                    }
                }
                if (esNuevo )
                {
                    db.MesasAbiertas.AddObject(mesaAbierta);
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
        }
        private void GuardarFactura()
        {
            using(var db = new FeriaEntities())
            {
                cliente = FactoryClientes.Item(db, factura.CedulaRif);
                if (cliente == null)
                {
                    cliente = new Cliente();
                    cliente.CedulaRif = factura.CedulaRif;
                    cliente.RazonSocial = factura.RazonSocial;
                    cliente.Direccion = factura.Direccion;
                    db.Clientes.AddObject(cliente);
                }
                else
                {
                    cliente.CedulaRif = factura.CedulaRif;
                    cliente.RazonSocial = factura.RazonSocial;
                    cliente.Direccion = factura.Direccion;
                }
                if (factura.IdFactura == null)
                {
                    esNuevo = true;
                    factura.IdFactura = FactoryContadores.GetMax("IdFactura");
                }
                foreach (FacturasPlato p in  factura.FacturasPlatos)
                {
                    if (p.IdFacturaPlato == null)
                    {
                        p.IdFacturaPlato = FactoryContadores.GetMax("IdFacturaPlato");
                    }
                }
                if (esNuevo)
                {
                    db.Facturas.AddObject(factura);
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
        }
        private void Imprimir()
        {
            FiscalBixolon f = new FiscalBixolon();
            f.ImprimeCorte(mesaAbierta);
        }
        private void ImprimirFactura()
        {
            FiscalBixolon f = new FiscalBixolon();
            f.ImprimeFactura(factura);
        }
        private void EliminarItem(MesasAbiertasPlato ultimo)
        {
            if (ultimo != null)
            {
                mesaAbierta.MesasAbiertasPlatos.Remove(ultimo);
            }
        }
        void gridControl1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete || e.KeyCode == Keys.Subtract)
            {
                MesasAbiertasPlato ultimo = (MesasAbiertasPlato)mesasAbiertasPlatoBindingSource.Current;
                if (ultimo != null)
                {
                    mesaAbierta.MesasAbiertasPlatos.Remove(ultimo);
                }
                e.Handled = true;
            }
        }
        private void OcultarPlatos()
        {
            foreach (Button b in platos)
            {
                b.Visible = false;
                b.Font = new System.Drawing.Font("Verdana", 9, FontStyle.Bold);
            }
        }
        void cantidad_Click(object sender, EventArgs e)
        {
            Button item = (Button)sender;
            cantidad = Convert.ToInt16(item.Text);
        }
        void grupo_Click(object sender, EventArgs e)
        {
            Button item = (Button)sender;
            CargarPlatos(item.Text);
        }
        void plato_Click(object sender, EventArgs e)
        {
            Button item = (Button)sender;
            AgregarItem((Plato)item.Tag);
        }
        void AgregarItem(Plato plato)
        {
            MesasAbiertasPlato item = new MesasAbiertasPlato();
            item.Descripcion = plato.Descripcion;
            item.Precio = plato.Precio;
            item.PrecioConIva = plato.PrecioConIva;
            if (FactoryPlatos.getArrayComentarios(plato).Count() > 0 || FactoryPlatos.getArrayContornos(plato).Count() > 0)
            {
                FrmPedirContornos f = new FrmPedirContornos();
                f.codigoPlato = plato.Codigo;
                f.ShowDialog();
                if (f.presentacion != null)
                {
                    item.Descripcion = plato.Descripcion + "-" + f.presentacion;
                    item.Precio = f.precio;
                    item.PrecioConIva = item.Precio + (item.Precio * plato.TasaIva / 100);
                }
                item.Comentarios = f.Comentarios;
                item.Contornos = f.Contornos;
            }
            item.Cantidad = cantidad;
            item.Codigo = plato.Codigo;
            item.Grupo = plato.Grupo;
            item.Idplato = plato.IdPlato;
            item.TasaIva = plato.TasaIva;
            item.Total = item.PrecioConIva.GetValueOrDefault(0) * cantidad;
            mesaAbierta.MesasAbiertasPlatos.Add(item);
        }
        Image LeerImagen(string codigo)
        {
                string archivo = Application.StartupPath + "\\Imagenes\\" + codigo + ".jpg";
                System.Drawing.Bitmap imagen = new System.Drawing.Bitmap((Image)Image.FromFile(
                                                    Application.StartupPath + "\\Imagenes\\" + codigo + ".jpg"));
                return imagen.GetThumbnailImage(40, 40, null, IntPtr.Zero);
        }
        void CargarPlatos(string grupo)
        {

            using (FeriaEntities db = new FeriaEntities())
            {
                mplatos = (from x in db.Platos
                            orderby x.Descripcion
                            where x.Grupo == grupo
                            select x).Take(30).ToList();
                int i = 0;
                OcultarPlatos();
                foreach (Plato s in mplatos)
                {
                    try
                    {
                        platos[i].Image = LeerImagen(s.Codigo);
                    }
                    catch { platos[i].Image = this.Pagos.Image; } 
                    platos[i].Visible = true;
                    platos[i].Text = s.Descripcion;
                    platos[i].Tag = s;
                    i++;
                }
            }
        }
        void CargarGrupos()
        {
            using(FeriaEntities db = new FeriaEntities())
            {
                List<string> mgrupos = FactoryPlatos.getListGrupos();
                int i = 0;
                foreach( string s in mgrupos)
                {
                    Plato p = (from y in db.Platos
                                                where y.Grupo == s
                                                orderby y.Descripcion
                                                select y).FirstOrDefault();
                    try
                    {
                        grupos[i].Image = LeerImagen(p.Codigo);
                    }
                    catch {  }
                    finally { }
                    grupos[i].Visible = true;
                    grupos[i].Text = s;
                    i++;                    
                }
            }
        }
    }
    }

