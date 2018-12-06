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
    public partial class FrmIngredientesInventario : Form
    {
        DateTime dia = DateTime.Today;
        FeriaEntities db = new FeriaEntities();
        List<IngredientesInventario> Lista = new List<IngredientesInventario>();
        public string filtro;
        public FrmIngredientesInventario()
        {
            InitializeComponent();
            this.Load += new EventHandler(FrmIngredientesInventario_Load);
        }

        void FrmIngredientesInventario_Load(object sender, EventArgs e)
        {
            this.FormClosed += new FormClosedEventHandler(FrmIngredientesInventario_FormClosed);
            this.btnBuscar.Click += new EventHandler(btnBuscar_Click);

            this.txtBuscar.Text =  dia.ToString("d"); ;
            this.FormClosed+=new FormClosedEventHandler(FrmIngredientesInventario_FormClosed);
            var hayposteriores = (from q in db.IngredientesInventarios
                                 orderby q.Ingrediente
                                 where q.Fecha.Value>  dia
                                 select q).FirstOrDefault();
            if (hayposteriores == null)
            {
                this.btnGuardar.Click += new EventHandler(btnGuardar_Click);
                this.btnBuscar.Visible = true;
            }
            this.btnImprimir.Click += new EventHandler(btnImprimir_Click);
            this.gridView1.RowUpdated+=new DevExpress.XtraGrid.Views.Base.RowObjectEventHandler(gridView1_RowUpdated);
            this.gridView1.ValidateRow+=new DevExpress.XtraGrid.Views.Base.ValidateRowEventHandler(gridView1_ValidateRow);
            this.repositoryItemCalcEdit1.ValidateOnEnterKey = true;
            this.repositoryItemCalcEdit1.Validating += new CancelEventHandler(repositoryItemCalcEdit1_Validating);
        }

        void repositoryItemCalcEdit1_Validating(object sender, CancelEventArgs e)
        {
            Calcular();
            this.bs.ResetCurrentItem();
        }

        void btnImprimir_Click(object sender, EventArgs e)
        {
            FrmReportes f = new FrmReportes();
            f.IngredienteInventarios(Lista);
        }
        
        void btnGuardar_Click(object sender, EventArgs e)
        {
            this.bs.EndEdit();
            foreach (IngredientesInventario i in Lista)
            {
                Ingrediente item = FactoryIngredientes.Item(i.IdIngrediente);
                i.Ajuste = i.InventarioFisico - i.Final;
                item.Existencia = i.InventarioFisico;
            }
            this.db.SaveChanges();
        }

        void btnBuscar_Click(object sender, EventArgs e)
        {
            Busqueda();
        }

        private void Busqueda()
        {
            if (!DateTime.TryParse(txtBuscar.Text, out dia))
            {
                txtBuscar.Text = DateTime.Today.ToString("d");
                return;
            }
            db = new FeriaEntities();
            IngredientesInventario InventarioSiguiente = (from item in db.IngredientesInventarios
                                                          where item.Fecha > dia
                                                          select item).FirstOrDefault();
            if (InventarioSiguiente != null)
            {
                MessageBox.Show("Atencion hay movimientos posteriores a esta fecha");
            }
            Lista = (from q in db.IngredientesInventarios
                     orderby q.Ingrediente
                     where q.Fecha == dia
                     select q).ToList();
            foreach(Ingrediente item in db.Ingredientes)
            {
                var esta = Lista.FirstOrDefault(x => x.IdIngrediente == item.IdIngrediente);
                if (esta == null)
                {
                    IngredientesInventario nuevoItem=new IngredientesInventario();
                    var DiaAnterior = (from p in db.IngredientesInventarios
                                       where p.IdIngrediente == item.IdIngrediente
                                       && p.Fecha <= dia
                                       orderby p.Fecha descending
                                       select p).FirstOrDefault();
                    if (DiaAnterior != null)
                    {
                        nuevoItem.Inicio = DiaAnterior.InventarioFisico;
                        nuevoItem.Salidas = 0;
                        nuevoItem.Entradas = 0;
                        nuevoItem.Final = DiaAnterior.InventarioFisico;
                    }
                    else
                    {
                        nuevoItem.Inicio = 0;
                        nuevoItem.Salidas = 0;
                        nuevoItem.Entradas = 0;
                        nuevoItem.Final = 0;                        
                    }
                    nuevoItem.Fecha=dia;
                    nuevoItem.IdIngrediente=item.IdIngrediente;
                    nuevoItem.IdIngredienteInventario = FactoryContadores.GetMax("IdIngredienteInventario");
                    nuevoItem.Ingrediente = item.Descripcion;
                    db.IngredientesInventarios.AddObject(nuevoItem);
                    db.SaveChanges();
                }
            }
            Lista = (from q in db.IngredientesInventarios
                     orderby q.Ingrediente
                     where q.Fecha == dia
                     select q).ToList();
            this.bs.DataSource = Lista;
            this.bs.ResetBindings(true);
        }

        void FrmIngredientesInventario_FormClosed(object sender, FormClosedEventArgs e)
        {
            Pantallas.IngredientesInventario = null;
        }
        private void gridView1_RowUpdated(object sender, DevExpress.XtraGrid.Views.Base.RowObjectEventArgs e)
        {
            this.bs.EndEdit();
            IngredientesInventario Item = (IngredientesInventario)e.Row;
            this.btnGuardar.Visible = true;
        }
        private void gridView1_ValidateRow(object sender, DevExpress.XtraGrid.Views.Base.ValidateRowEventArgs e)
        {
            try
            {
                Calcular();
            }
            catch (Exception x)
            {
                MessageBox.Show(x.Message);
                e.Valid = false;
            }
        }
        private void Calcular()
        {
            try
            {
                bs.EndEdit();
                IngredientesInventario Item = (IngredientesInventario)bs.Current;
                Item.Final = Item.Inicio.GetValueOrDefault(0) + Item.Entradas.GetValueOrDefault(0) - Item.Salidas.GetValueOrDefault(0);
                Item.Ajuste = Item.InventarioFisico - Item.Final;
            }
            catch (Exception x)
            {
                throw x;
            }
        }


    }
}

//J299063070
//VAGON GROP,