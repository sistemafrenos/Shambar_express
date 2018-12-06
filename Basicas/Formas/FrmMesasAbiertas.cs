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
    public partial class FrmMesasAbiertas : Form
    {
        List<Button> salones = new List<Button>();
        List<PictureBox> mesas = new List<PictureBox>();
        FeriaEntities db = new FeriaEntities();
        private string salon;
        public FrmMesasAbiertas()
        {
            InitializeComponent();
            this.Load+=new EventHandler(FrmMesasAbiertas_Load);
        }
        void  FrmMesasAbiertas_Load(object sender, EventArgs e)
        {
            salones.AddRange(new Button[] { ubicacion0, ubicacion1, ubicacion2, ubicacion3, ubicacion4 });
            mesas.AddRange(new PictureBox[] { mesa0, mesa1, mesa2, mesa3, mesa4, mesa8, mesa7, mesa9, mesa6, mesa5, mesa10, mesa11, mesa12, mesa13, mesa14, mesa15, mesa16, mesa17, mesa18, mesa19, mesa20, mesa21, mesa22, mesa23, mesa24, mesa25, mesa26, mesa27, mesa28, mesa29 });
            foreach (Button b in salones)
            {
                b.Visible = false;
                b.Font = new System.Drawing.Font("Verdana", 9, FontStyle.Bold);
            }
            OcultarMesas();
            foreach (Button b in salones)
            {
                b.Click += new EventHandler(Salon_Click);                
            }
            foreach (PictureBox b in mesas)
            {
                b.Click += new EventHandler(Mesa_Click);
            }
            foreach (PictureBox control in mesas)
            {
                control.Paint+=new PaintEventHandler(mesa_Paint);
                control.Dock = DockStyle.Fill;
            }
            CargarSalones();
            this.Height = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height - 100;
            this.Width = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Width - 50;
            timer1.Tick += new EventHandler(timer1_Tick);
            this.CenterToScreen();
        }
        void timer1_Tick(object sender, EventArgs e)
        {
            CargarMesas(salon);
        }
        private void OcultarMesas()
        {
            foreach (PictureBox b in mesas)
            {
                b.Visible = false;
                b.Font = new System.Drawing.Font("Verdana", 9, FontStyle.Bold);
            }
        }
        void Salon_Click(object sender, EventArgs e)
        {
            Button item = (Button)sender;
            CargarMesas(item.Text);
            salon = item.Text;
        }
        void Mesa_Click(object sender, EventArgs e)
        {
            PictureBox item = (PictureBox)sender;
            this.timer1.Enabled = false;
            EditarMesa((Mesa)item.Tag);
            this.timer1.Enabled = true;
        }
        void CargarMesas(string salon)
        {

            using (FeriaEntities db = new FeriaEntities())
            {
                var mMesas = (from x in db.Mesas
                             orderby x.Descripcion
                           where x.Ubicacion == salon
                           select x).ToList();
                int i = 0;
                OcultarMesas();
                foreach (Mesa s in mMesas)
                {
                    mesas[i].Visible = true;
                    mesas[i].Tag = s;
                    i++;
                }
            }
        }
        void CargarSalones()
        {
            using (FeriaEntities db = new FeriaEntities())
            {
                List<string> mUbicaciones = FactoryMesas.getUbicaciones();
                int i = 0;
                if (mUbicaciones.Count == 0)
                {
                    return; 
                }
                salon = mUbicaciones[0];
                foreach (string s in mUbicaciones)
                {
                    Mesa p = (from y in db.Mesas
                              where y.Ubicacion == s
                              orderby y.Ubicacion
                              select y).FirstOrDefault();
                    salones[i].Visible = true;
                    salones[i].Text = s;
                    i++;
                }
                CargarMesas(salon);
            }            
        }
        void EditarMesa(Mesa mesa)
        {
            FrmEditarMesa f = new FrmEditarMesa();
            f.mesa = FactoryMesas.Item(db,mesa.IdMesa) ;
            if (f.mesa.MesasAbiertas.Count > 0)
            {
                f.mesaAbierta = f.mesa.MesasAbiertas.FirstOrDefault();
            }
            f.ShowDialog();
            db = new FeriaEntities();
            Application.DoEvents();
        }
        private void mesa_Paint(object sender, PaintEventArgs e)
        {            
            if(((PictureBox)sender).Visible==false)
               return;
            Mesa  m = FactoryMesas.Item(db,( (Mesa)((PictureBox)sender).Tag).IdMesa);
            Graphics control = e.Graphics;
            Font fuente = new Font("Verdana", 10, FontStyle.Bold);
            control.Clear(this.BackColor);
            control.FillRectangle(SystemBrushes.ActiveCaption, 0, 0, control.ClipBounds.Width, 20);
            control.DrawString(m.Descripcion,fuente , SystemBrushes.ActiveCaptionText, 0, 0);
            int cantidad = cantidad = m.MesasAbiertas.Count;
            MesasAbierta mesa = m.MesasAbiertas.FirstOrDefault();
            switch(cantidad)
            {
                case 0:
                    break;
                case 1:
                    control.DrawString(mesa.Numero, fuente, Brushes.Black, new PointF(120, 30));
                    control.DrawString(mesa.Apertura.Value.ToShortTimeString(), fuente, Brushes.Black, new PointF(10, 30));
                    control.DrawString(mesa.MontoTotal.GetValueOrDefault(0).ToString("n2").PadLeft(15), fuente, Brushes.Black, new PointF(50, 50));
                    break;
                default:
                    control.DrawString("(" + cantidad.ToString() + ")", fuente, Brushes.Black, new PointF(120, 30));
                    control.DrawString(mesa.Apertura.Value.ToShortTimeString(), fuente, Brushes.Black, new PointF(10, 50));
                    control.DrawString(m.MesasAbiertas.Sum(x => x.MontoTotal).GetValueOrDefault(0).ToString("N2").PadLeft(15), fuente, Brushes.Black, new PointF(50, 50));
                    break;
            }
        }
    }
}
; 