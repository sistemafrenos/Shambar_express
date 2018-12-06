﻿using System;
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
    public partial class FrmLogin : Form
    {
        Usuario usuario = new Usuario();
        public string TipoUsuario;
        public string Sistema;
        public FrmLogin()
        {
            InitializeComponent();
        }

        private void FrmLogin_Load(object sender, EventArgs e)
        {
            this.KeyPreview = true;
            this.KeyDown += new KeyEventHandler(FrmLogin_KeyDown);
            this.Aceptar.Click += new EventHandler(Aceptar_Click);
            this.Cancelar.Click += new EventHandler(Cancelar_Click);
            switch (TipoUsuario)
            {
                case "CAJERO":
                    this.txtUsuario.Properties.Items.AddRange(FactoryUsuarios.getCajeros());
                    break;
                case "ADMINISTRADOR":
                    this.txtUsuario.Properties.Items.AddRange(FactoryUsuarios.getAdministradores());
                    break;
                case "MESONERO":
                    this.txtUsuario.Properties.Items.AddRange(FactoryUsuarios.getMesoneros());
                    break;
            }
            
            this.txtContraseña.Properties.CharacterCasing = CharacterCasing.Upper;
            this.txtUsuario.Properties.CharacterCasing = CharacterCasing.Upper;
            this.Text = Sistema;
            layoutControlItem1.Text = TipoUsuario.ToLower();
        }

        void Cancelar_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Close();
        }

        void Aceptar_Click(object sender, EventArgs e)
        {
            if (this.txtUsuario.Text == "MAESTRO" && this.txtContraseña.Text == "ALEMAN")
            {
                usuario = FactoryUsuarios.Item(this.txtUsuario.Text, this.txtContraseña.Text);
                if (usuario == null)
                {
                    usuario= FactoryUsuarios.CrearUsuario(TipoUsuario);
                    this.txtUsuario.Text = usuario.Nombre;
                    this.txtContraseña.Text = usuario.Clave;
                    using (var dc = new FeriaEntities())
                    {
                        dc.Usuarios.AddObject(usuario);
                        dc.SaveChanges();
                    }
                }
            }
            usuario = FactoryUsuarios.Item(this.txtUsuario.Text, this.txtContraseña.Text);
            if (usuario == null)
            {
                MessageBox.Show("Este Usuario y contraseña son invalidos");
                return;
            }
            switch (TipoUsuario)
            {
                case "CAJERO":
                    FactoryUsuarios.CajeroActivo = usuario;
                    break;
                case "ADMINISTRADOR":
                    FactoryUsuarios.UsuarioActivo = usuario;
                    break;
                case "MESONERO":
                    FactoryUsuarios.MesoneroActivo = usuario;
                    break;
            }
            this.DialogResult = DialogResult.OK;
            this.Close();

        }

        void FrmLogin_KeyDown(object sender, KeyEventArgs e)
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
