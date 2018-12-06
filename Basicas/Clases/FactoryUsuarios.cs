using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HK.Clases
{
    public class FactoryUsuarios
    {
        public static Usuario UsuarioActivo = null;
        public static Usuario CajeroActivo = null;
        public static Usuario MesoneroActivo = null;
        public static List<string> getAdministradores()
        {
            using (var db = new FeriaEntities())
            {
                var q = from p in db.Usuarios
                        orderby p.Nombre
                        where p.Activo == true && p.TipoUsuario == "ADMINISTRADOR"
                        select p.Nombre;
                return q.ToList();
            }
        }
        public static List<string> getCajeros()
        {
            using (var db = new FeriaEntities())
            {
                var q = from p in db.Usuarios
                        orderby p.Nombre
                        where p.Activo == true && p.TipoUsuario=="CAJERO"
                        select p.Nombre;
                return q.ToList();
            }
        }
        public static List<string> getMesoneros()
        {
            using (var db = new FeriaEntities())
            {
                var q = from p in db.Usuarios
                        orderby p.Nombre
                        where p.Activo == true && p.TipoUsuario == "MESONEROS"
                        select p.Nombre;
                return q.ToList();
            }
        }
        public static List<Usuario> getItems(string texto,string Tipo)
        {
            using (var db = new FeriaEntities())
            {
                var q = from p in db.Usuarios
                        orderby p.Nombre
                        where (p.Cedula.Contains(texto) || p.Nombre.Contains(texto) || texto.Length == 0) && p.Activo == true && p.TipoUsuario == Tipo
                        select p;
                return q.ToList();
            }
        }
        public static List<Usuario> getItems(FeriaEntities db,string texto,string Tipo)
        {
            var q = from p in db.Usuarios
                    orderby p.Nombre
                    where (p.Cedula.Contains(texto) || p.Nombre.Contains(texto) || texto.Length == 0) && p.Activo == true && p.TipoUsuario == Tipo
                    select p;
            return q.ToList();
        }
        public static Usuario Item(string ID)
        {
            using (var db = new FeriaEntities())
            {
                var q = from p in db.Usuarios
                        where p.IdUsuario == ID
                        select p;
                return q.FirstOrDefault();
            }
        }
        public static Usuario ItemNombre(string nombre)
        {
            using (var db = new FeriaEntities())
            {
                var q = from p in db.Usuarios
                        where p.Nombre == nombre
                        select p;
                return q.FirstOrDefault();
            }
        }
        public static Usuario Item(string Usuario,string Contraseña)
        {
            using (var db = new FeriaEntities())
            {
                var q = from p in db.Usuarios
                        where p.Nombre == Usuario && p.Clave == Contraseña && p.Activo == true
                        select p;
                return q.FirstOrDefault();
            }
        }
        public static void Validar(Usuario registro)
        {
            if (string.IsNullOrEmpty(registro.Cedula))
                throw new Exception("Error el campo cedula no puede estar vacio");
            if (registro.Cedula.Length > 20)
                throw new Exception("Error el campo cedula no puede tener mas de 20 caracteres");
            if (string.IsNullOrEmpty(registro.Codigo))
                throw new Exception("Error el codigo no puede estar vacio");
            if (registro.Codigo.Length > 20)
                throw new Exception("Error codigo no puede tener mas de 20 caracteres");
            if (string.IsNullOrEmpty(registro.Nombre))
                throw new Exception("Error el Nombre  no puede estar vacio");
            if (string.IsNullOrEmpty(registro.TipoUsuario))
                throw new Exception("Error el Tipo Usuario no puede estar vacio");
        }
        public static Usuario CrearUsuario(string TipoUsuario)
        {
            Usuario usuario = new Usuario();
            usuario.Activo = true;
            usuario.Codigo = "00";
            usuario.Nombre = TipoUsuario;
            usuario.Cedula = "V0123456789";
            usuario.TipoUsuario = TipoUsuario;
            usuario.IdUsuario = FactoryContadores.GetMax("IdUsuario");
            usuario.Clave = "**";
            return usuario;
        }
    }
}
