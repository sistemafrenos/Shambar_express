using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HK.Formas;
using System.Threading;
using System.Drawing;
using System.Drawing.Printing;
using System.Security.Cryptography;


namespace HK
{

    #region consultas
    public class VentasxPlato : Plato
    {
        int platosVendidos = 0;

        public int PlatosVendidos
        {
            get { return platosVendidos; }
            set { platosVendidos = value; }
        }

        double montoPlatosVendidos = 0;

        public double MontoPlatosVendidos
        {
            get { return montoPlatosVendidos; }
            set { montoPlatosVendidos = value; }
        }
    }
    public class TotalxFormaPago
    {
        string formaPago;

        public string FormaPago
        {
            get { return formaPago; }
            set { formaPago = value; }
        }
        double bolivares = 0;

        public double Bolivares
        {
            get { return bolivares; }
            set { bolivares = value; }
        }
    }
    public class TotalxDia
    {
        int facturas=0;

        public int Facturas
        {
          get { return facturas; }
          set { facturas = value; }
        }

        DateTime fecha;

        public DateTime Fecha
        {
            get { return fecha; }
            set { fecha = value; }
        }
        double bolivares;

        public double Bolivares
        {
            get { return bolivares; }
            set { bolivares = value; }
        }
        double promedio;

        public double Promedio
        {
            get { return promedio; }
            set { promedio = value; }
        }
    }
    public class Valores
    {
        string variable;

        public string Variable
        {
            get { return variable; }
            set { variable = value; }
        }
        double? valor = 0;

        public double? Valor
        {
            get { return valor; }
            set { valor = value; }
        }
    }
    public class IngredientesConsumo : Ingrediente
    {
        double cantidad = 0;

        public double Cantidad
        {
            get { return cantidad; }
            set { cantidad = value; }
        }
    }

    #endregion
    partial class FacturasPlato
    {
        List<string> comentarios;

        public List<string> Comentarios
        {
            get { return comentarios; }
            set { comentarios = value; }
        }
        List<string> contornos;

        public List<string> Contornos
        {
            get { return contornos; }
            set { contornos = value; }
        }
    }
    partial class MesasAbiertasPlato
    {
        List<string> comentarios;

        public List<string> Comentarios
        {
            get { return comentarios; }
            set { comentarios = value; }
        }
        List<string> contornos;

        public List<string> Contornos
        {
            get { return contornos; }
            set { contornos = value; }
        }
    }
}
namespace HK.Clases
{
    public class Basicas
    {
        public static double Round(double? valor)
        {
            try
            {
                decimal myValor = (decimal)valor;
                myValor = decimal.Round(myValor, 2);
                return (double)myValor;
            }
            catch
            {
                return 0;
            }
        }
        public static void DisposeImage(Image image)
        {
            image.Dispose();
            image = null;
            GC.Collect();
        }
        public static string CedulaRif(string Texto)
        {
            if (string.IsNullOrEmpty(Texto))
            {
                return Texto;
            }
            Texto = Texto.ToUpper();
            Texto = Texto.Replace(":", "");
            Texto = Texto.Replace("-", "").Replace(".", "").Replace(" ", "").Replace(",", "").Replace("CI", "").Replace("RIF", "").Replace("C", "");
            if (Texto.Length > 0)
            {
                if (IsNumeric(Texto[0]))
                {
                    Texto = "V" + Texto;
                }
            }
            if (Texto[0] == 'J' || Texto[0] == 'G')
            {
                if (Texto.Length > 10)
                {
                    Texto.Substring(0, 10);
                }
            }
            return Texto;
        }
        public static string PrintFix(string Texto, int Largo, int Alineacion)
        {
            string x = "                                        ";
            Texto = Texto.Trim();
            if (Texto.Length > Largo)
            {
                return Texto.Substring(0, Texto.Length);
            }
            switch (Alineacion)
            {
                case 1:
                    return Texto.PadRight(Largo);

                case 2:
                    return Texto.PadLeft(Largo);

                case 3:
                    int Suplemento = (Largo - Texto.Length) / 2;
                    return x.Substring(0, Suplemento) + Texto + x.Substring(0, Suplemento);

            }
            return Texto;
        }
        public static string PrintNumero(double? d, int len)
        {
            if (!d.HasValue)
            {
                d = 0;
            }
            return d.Value.ToString("n2").PadLeft(len);
        }
        public static bool IsNumeric(object Expression)
        {
            bool isNum;
            double retNum;

            isNum = Double.TryParse(Convert.ToString(Expression), System.Globalization.NumberStyles.Any, System.Globalization.NumberFormatInfo.InvariantInfo, out retNum);
            return isNum;
        }
        public static Parametro parametros()
        {
            using (var db = new FeriaEntities())
            {
                Parametro p = db.Parametros.FirstOrDefault();
                return p;
            }
        }
        public static List<TotalxFormaPago> VentasxLapso(DateTime desde, DateTime hasta)
        {
            List<TotalxFormaPago> retorno = new List<TotalxFormaPago>();
            using (var db = new FeriaEntities())
            {
                var consulta = from q in db.Facturas
                               where (q.Fecha.Value>=desde && q.Fecha.Value<=hasta && q.Anulado==false) 
                               select q;
                TotalxFormaPago efectivo = new TotalxFormaPago();
                efectivo.FormaPago = "EFECTIVO";
                double cambio = consulta.Sum(x => x.Cambio).GetValueOrDefault(0);
                efectivo.Bolivares = consulta.Sum(x => x.Efectivo).GetValueOrDefault(0)-cambio;
                retorno.Add(efectivo);
                TotalxFormaPago cheque = new TotalxFormaPago();
                cheque.FormaPago = "CHEQUE";
                cheque.Bolivares = consulta.Sum(x => x.Cheque).GetValueOrDefault(0);
                retorno.Add(cheque);
                TotalxFormaPago tarjeta = new TotalxFormaPago();
                tarjeta.FormaPago = "TARJETA";
                tarjeta.Bolivares = consulta.Sum(x => x.Tarjeta).GetValueOrDefault(0);
                retorno.Add(tarjeta);
                TotalxFormaPago cestaTicket = new TotalxFormaPago();
                cestaTicket.FormaPago = "CESTA TICKET";
                cestaTicket.Bolivares = consulta.Sum(x => x.CestaTicket).GetValueOrDefault(0);
                retorno.Add(cestaTicket);
                TotalxFormaPago consumoInterno = new TotalxFormaPago();
                consumoInterno.FormaPago = "CONSUMO INTERNO";
                consumoInterno.Bolivares = consulta.Sum(x => x.ConsumoInterno).GetValueOrDefault(0);
                retorno.Add(consumoInterno);
                return retorno;
            }
        }
        public static List<TotalxFormaPago> VentasxLapso(DateTime desde, DateTime hasta, Usuario cajero)
        {
            List<TotalxFormaPago> retorno = new List<TotalxFormaPago>();
            using (var db = new FeriaEntities())
            {
                var consulta = from q in db.Facturas
                               where (q.Fecha.Value >= desde && q.Fecha.Value <= hasta && q.Anulado == false && q.IdCajero==cajero.IdUsuario)
                               select q;
                TotalxFormaPago efectivo = new TotalxFormaPago();
                efectivo.FormaPago = "EFECTIVO";
                double cambio = consulta.Sum(x => x.Cambio).GetValueOrDefault(0);
                efectivo.Bolivares = consulta.Sum(x => x.Efectivo).GetValueOrDefault(0)-cambio;
                retorno.Add(efectivo);
                TotalxFormaPago cheque = new TotalxFormaPago();
                cheque.FormaPago = "CHEQUE";
                cheque.Bolivares = consulta.Sum(x => x.Cheque).GetValueOrDefault(0);
                retorno.Add(cheque);
                TotalxFormaPago tarjeta = new TotalxFormaPago();
                tarjeta.FormaPago = "TARJETA";
                tarjeta.Bolivares = consulta.Sum(x => x.Tarjeta).GetValueOrDefault(0);
                retorno.Add(tarjeta);
                TotalxFormaPago cestaTicket = new TotalxFormaPago();
                cestaTicket.FormaPago = "CESTA TICKET";
                cestaTicket.Bolivares = consulta.Sum(x => x.CestaTicket).GetValueOrDefault(0);
                retorno.Add(cestaTicket);
                TotalxFormaPago consumoInterno = new TotalxFormaPago();
                consumoInterno.FormaPago = "CONSUMO INTERNO";
                consumoInterno.Bolivares = consulta.Sum(x => x.ConsumoInterno).GetValueOrDefault(0);
                retorno.Add(consumoInterno);
                return retorno;
            }
        }
        public static List<TotalxFormaPago> VentasxHoras(DateTime desde, DateTime hasta)
        {
            List<TotalxFormaPago> retorno = new List<TotalxFormaPago>();
            using (var db = new FeriaEntities())
            {
                var consulta = from q in db.Facturas
                               where (q.Fecha.Value == DateTime.Today.Date && q.Hora.Value >= desde && q.Hora.Value <= hasta && q.Anulado==false)
                               select q;
                TotalxFormaPago efectivo = new TotalxFormaPago();
                double cambio = consulta.Sum(x => x.Cambio).GetValueOrDefault(0);
                efectivo.FormaPago = "EFECTIVO";
                efectivo.Bolivares = consulta.Sum(x => x.Efectivo).GetValueOrDefault(0)-cambio;
                retorno.Add(efectivo);
                TotalxFormaPago cheque = new TotalxFormaPago();
                cheque.FormaPago = "CHEQUE";
                cheque.Bolivares = consulta.Sum(x => x.Cheque).GetValueOrDefault(0);
                retorno.Add(cheque);
                TotalxFormaPago tarjeta = new TotalxFormaPago();
                tarjeta.FormaPago = "TARJETA";
                tarjeta.Bolivares = consulta.Sum(x => x.Tarjeta).GetValueOrDefault(0);
                retorno.Add(tarjeta);
                TotalxFormaPago cestaTicket = new TotalxFormaPago();
                cestaTicket.FormaPago = "CESTA TICKET";
                cestaTicket.Bolivares = consulta.Sum(x => x.CestaTicket).GetValueOrDefault(0);
                retorno.Add(cestaTicket);
                TotalxFormaPago consumoInterno = new TotalxFormaPago();
                consumoInterno.FormaPago = "CONSUMO INTERNO";
                consumoInterno.Bolivares = consulta.Sum(x => x.ConsumoInterno).GetValueOrDefault(0);
                retorno.Add(consumoInterno);
                return retorno;
            }
        }
        public static List<TotalxDia> VentasDiariasxLapso(DateTime desde, DateTime hasta)
        {
            using (var db = new FeriaEntities())
            {
                var consulta = from q in db.Facturas
                               where (q.Fecha.Value >= desde && q.Fecha.Value <= hasta && q.Anulado==false)
                               group q by q.Fecha into ventaxDia
                               select new TotalxDia 
                               {
                                   Fecha = ventaxDia.Key.Value,
                                   Facturas = (int)ventaxDia.Count(),
                                   Promedio = (double)ventaxDia.Average(x=> x.MontoTotal),
                                   Bolivares = (double)ventaxDia.Sum(x=> x.MontoTotal)
                               };
                return consulta.ToList();
            }
        }
        public static List<TotalxDia> VentasDiariasxLapso(DateTime desde, DateTime hasta,Usuario cajero)
        {
            using (var db = new FeriaEntities())
            {
                var consulta = from q in db.Facturas
                               where (q.Fecha.Value >= desde && q.Fecha.Value <= hasta && q.Anulado == false && q.IdCajero == cajero.IdUsuario)
                               group q by q.Fecha into ventaxDia
                               select new TotalxDia
                               {
                                   Fecha = ventaxDia.Key.Value,
                                   Facturas = (int)ventaxDia.Count(),
                                   Promedio = (double)ventaxDia.Average(x => x.MontoTotal),
                                   Bolivares = (double)ventaxDia.Sum(x => x.MontoTotal)
                               };
                return consulta.ToList();
            }
        }
        public static List<TotalxDia> VentasDiariasxHoras(DateTime desde, DateTime hasta)
        {
            using (var db = new FeriaEntities())
            {
                var consulta = from q in db.Facturas
                               where (q.Hora.Value >= desde && q.Hora.Value <= hasta && q.Fecha==DateTime.Today && q.Anulado==false)
                               group q by q.Fecha into ventaxDia
                               select new TotalxDia
                               {
                                   Fecha = ventaxDia.Key.Value,
                                   Facturas = (int)ventaxDia.Count(),
                                   Promedio = (double)ventaxDia.Average(x => x.MontoTotal),
                                   Bolivares = (double)ventaxDia.Sum(x => x.MontoTotal)
                               };
                return consulta.ToList();
            }
        }
        public static List<Factura> LibroDeVentas(DateTime fecha)
        {
            using (var db = new FeriaEntities())
            {
                var consulta = from q in db.Facturas
                               where (q.Fecha.Value.Month >= fecha.Month && q.Fecha.Value.Year <= fecha.Year) && q.Tipo=="FACTURA"
                               orderby q.Numero
                               select q;
                List<Factura> x = consulta.ToList();
                return x;
            }
        }
        public static List<VentasxPlato> VentasLapsoxPlatos(DateTime desde, DateTime hasta, Usuario cajero)
        {
            using (FeriaEntities db= new FeriaEntities())
            {
                var q = from factura in db.Facturas
                        join facturaplato in db.FacturasPlatos on factura.IdFactura equals facturaplato.IdFactura
                        where factura.Fecha.Value >= desde && factura.Fecha.Value <= hasta && factura.Anulado == false && cajero.IdUsuario == factura.IdCajero
                        select new VentasxPlato
                        {
                            Grupo = facturaplato.Grupo,
                            Descripcion = facturaplato.Descripcion,
                            PlatosVendidos = facturaplato.Cantidad.Value,
                            MontoPlatosVendidos = facturaplato.Total.Value
                        };

                var ResumenxPlato = from p in q.ToList()
                                    group p by new { p.Grupo, p.Descripcion } into ventaxPlato
                                    select new VentasxPlato
                                    {
                                        Grupo = ventaxPlato.Key.Grupo,
                                        Descripcion = ventaxPlato.Key.Descripcion,
                                        PlatosVendidos = ventaxPlato.Sum(x => x.PlatosVendidos),
                                        MontoPlatosVendidos = ventaxPlato.Sum(x => x.MontoPlatosVendidos)
                                    };
                return ResumenxPlato.ToList();
            }
        }
        public static List<IngredientesConsumo> ConsumoPorFechaCajero(DateTime fecha, Usuario cajero )
        {
            using (FeriaEntities db = new FeriaEntities())
            {
                
                List<IngredientesConsumo> q = (from factura in db.Facturas
                                               join facturaplato in db.FacturasPlatos on factura.IdFactura equals facturaplato.IdFactura
                                               join productoIngrediente in db.PlatosIngredientes on facturaplato.Idplato equals productoIngrediente.IdPlato
                                               where factura.Fecha.Value == fecha && factura.IdCajero == cajero.IdUsuario && (factura.Tipo == "FACTURA" || factura.Tipo == "CONSUMO") 
                                               select new IngredientesConsumo
                                               {
                                                   IdIngrediente = productoIngrediente.IdIngrediente,
                                                   Descripcion = productoIngrediente.Ingrediente,
                                                   Cantidad = (double)facturaplato.Cantidad * (double)productoIngrediente.Cantidad
                                               }).ToList();

                var ResumenxIngrediente = from p in q
                                          group p by p.Descripcion into ConsumoxProducto
                                          select new IngredientesConsumo
                                          {
                                              Descripcion = ConsumoxProducto.Key,
                                              Cantidad = ConsumoxProducto.Sum(x => x.Cantidad)
                                          };
                var ordenado = from a in ResumenxIngrediente
                               orderby a.Descripcion
                               select a;
                return ordenado.ToList();
            }
        }
        public static List<IngredientesConsumo> ConsumoPorFecha(DateTime fecha)
        {
            using (FeriaEntities db = new FeriaEntities())
            {

                List<IngredientesConsumo> q = (from factura in db.Facturas
                                               join facturaplato in db.FacturasPlatos on factura.IdFactura equals facturaplato.IdFactura
                                               join productoIngrediente in db.PlatosIngredientes on facturaplato.Idplato equals productoIngrediente.IdPlato
                                               where factura.Fecha.Value == fecha && ( factura.Tipo=="FACTURA" ||factura.Tipo=="CONSUMO") 
                                               select new IngredientesConsumo
                                               {
                                                   IdIngrediente = productoIngrediente.IdIngrediente,
                                                   Descripcion = productoIngrediente.Ingrediente,
                                                   Cantidad = (double)facturaplato.Cantidad * (double)productoIngrediente.Cantidad
                                               }).ToList();

                var ResumenxIngrediente = from p in q
                                          group p by p.Descripcion into ConsumoxProducto
                                          select new IngredientesConsumo
                                          {
                                              Descripcion = ConsumoxProducto.Key,
                                              Cantidad = ConsumoxProducto.Sum(x => x.Cantidad)
                                          };
                var ordenado = from a in ResumenxIngrediente
                               orderby a.Descripcion
                               select a;
                return ordenado.ToList();
            }
        }

        public static List<IngredientesConsumo> ConsumoPlatosxLapso(DateTime desde, DateTime hasta)
        {
            using (FeriaEntities db = new FeriaEntities())
            {
                List<IngredientesConsumo> q = (from factura in db.Facturas
                        join facturaplato in db.FacturasPlatos on factura.IdFactura equals facturaplato.IdFactura
                        join productoIngrediente in db.PlatosIngredientes on facturaplato.Idplato equals productoIngrediente.IdPlato
                        where factura.Fecha.Value >= desde && factura.Fecha.Value <= hasta && ( factura.Tipo=="FACTURA" ||factura.Tipo=="CONSUMO") 
                        select new IngredientesConsumo
                        {
                            IdIngrediente= productoIngrediente.IdIngrediente,
                            Descripcion = productoIngrediente.Ingrediente,
                            Cantidad =  (double)facturaplato.Cantidad * (double)productoIngrediente.Cantidad
                        }).ToList();

                var ResumenxIngrediente = from p in q
                                    group p by p.Descripcion  into ConsumoxProducto
                                    select new IngredientesConsumo
                                    {
                                        Descripcion = ConsumoxProducto.Key,
                                        Cantidad = ConsumoxProducto.Sum(x => x.Cantidad)
                                    };
                var ordenado = from a in ResumenxIngrediente
                                                      orderby a.Descripcion
                                                      select a;
                return ordenado.ToList();
            }
        }
        public static List<VentasxPlato> VentasLapsoxPlatos(DateTime desde, DateTime hasta)
        {
            using (FeriaEntities db= new FeriaEntities())
            {
                var q = from factura in db.Facturas
                        join facturaplato in db.FacturasPlatos on factura.IdFactura equals facturaplato.IdFactura
                        where factura.Fecha.Value >= desde && factura.Fecha.Value <= hasta && factura.Anulado==false && ( factura.Tipo=="FACTURA" ||factura.Tipo=="CONSUMO") 
                        select new VentasxPlato
                        {
                            Grupo = facturaplato.Grupo,
                            Descripcion = facturaplato.Descripcion,
                            PlatosVendidos = facturaplato.Cantidad.Value,
                            MontoPlatosVendidos = facturaplato.Total.Value
                        };

                var ResumenxPlato = from p in q.ToList()
                                    group p by new { p.Grupo, p.Descripcion } into ventaxPlato
                                    select new VentasxPlato
                                    {
                                        Grupo = ventaxPlato.Key.Grupo,
                                        Descripcion = ventaxPlato.Key.Descripcion,
                                        PlatosVendidos = ventaxPlato.Sum(x => x.PlatosVendidos),
                                        MontoPlatosVendidos = ventaxPlato.Sum(x => x.MontoPlatosVendidos)
                                    };
                return ResumenxPlato.ToList();
            }
        }
        public static List<VentasxPlato> VentasHorasxPlatos(DateTime desde, DateTime hasta)
        {
            using (FeriaEntities db= new FeriaEntities())
            {
                var q = from factura in db.Facturas
                        join facturaplato in db.FacturasPlatos on factura.IdFactura equals facturaplato.IdFactura
                        where factura.Hora.Value >= desde && factura.Hora.Value <= hasta && factura.Fecha == DateTime.Today.Date && factura.Anulado == false && ( factura.Tipo=="FACTURA" ||factura.Tipo=="CONSUMO") 
                        select new VentasxPlato
                        {
                            Grupo = facturaplato.Grupo,
                            Descripcion = facturaplato.Descripcion,
                            PlatosVendidos = facturaplato.Cantidad.Value,
                            MontoPlatosVendidos = facturaplato.Total.Value
                        };

                var ResumenxPlato = from p in q.ToList()
                                    group p by new { p.Grupo, p.Descripcion } into ventaxPlato
                                    select new VentasxPlato
                                    {
                                        Grupo = ventaxPlato.Key.Grupo,
                                        Descripcion = ventaxPlato.Key.Descripcion,
                                        PlatosVendidos = ventaxPlato.Sum(x => x.PlatosVendidos),
                                        MontoPlatosVendidos = ventaxPlato.Sum(x => x.MontoPlatosVendidos)
                                    };
                return ResumenxPlato.ToList();
            }
        }
        public static void ImprimirOrden(Factura documento)
        {
            //PrintDocument pd = new PrintDocument();
            //String OldPrinter = pd.PrinterSettings.PrinterName;
            int Lineas = 0;
            //  Basicas.SetDefaultPrinter("RECIBOS");
            try
            {
                LPrintWriter l = new LPrintWriter();
                Font Fuente = new Font("Arial", (float)11.0);
                l.Font = Fuente;                
                l.WriteLine("COMANDA");
                l.WriteLine(" ");
                l.WriteLine("  FECHA:{0}",DateTime.Today.ToShortDateString());
                l.WriteLine("   HORA:{0}", DateTime.Now.ToShortTimeString());
                l.WriteLine("CLIENTE:{0}", documento.RazonSocial);
                l.WriteLine("    {0}:{1}", documento.Tipo, documento.NumeroOrden);
                foreach (var item in documento.FacturasPlatos)
                {
                    if (item.Cantidad.GetValueOrDefault(0) > 1)
                    {
                        l.WriteLine(" X {0}", item.Cantidad.Value.ToString("N0"));
                    }
                    l.WriteLine(string.Format(item.Descripcion));
                    if (item.Comentarios != null)
                    {
                        foreach (string p in item.Comentarios)
                        {
                            l.WriteLine(p);
                        }
                    }
                    if (item.Contornos != null)
                    {
                        foreach (string p in item.Contornos)
                        {
                            l.WriteLine(p);
                        }
                    }
                }
                l.WriteLine(" ");
                for (Lineas = 0; Lineas < 6; Lineas++)
                {
                    l.WriteLine(" ");
                }
                l.WriteLine(" ");
                l.WriteLine(".");
                l.WriteLine(" ");
                l.WriteLine(".");
                l.WriteLine(" ");
                l.WriteLine(".");
                l.Flush();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}

 
namespace PassEncrypt
{
        interface Crypto
        {
            /// <summary>
            ///  Gets the Hash key and Salt for a specific
            ///  password. 
            /// </summary>
            /// <returns>
            /// Will return a string with three elements      
            ///    1. Hash Key
            ///    2. Salt
            /// </returns>
            string[] GenerateKeySalt(string plainTxtPass);


            /// <summary>
            ///  Change the password.
            /// 
            /// </summary>
            /// <returns>
            /// Will return a string with three elements
            ///    1. New Password
            ///    2. Hash Key
            ///    3. Salt
            /// </returns>

            string GetHashKey(string input);

        }
    public class Encrypt : Crypto
    {
 
        #region Generate the Hash key and salt for a given password
 
        /// <summary>
        /// Returns a salt value and a hash key to be stored
        /// in the database for the given user.
        /// </summary>
        /// <param name=”plainTxtPass”></param>
        /// <returns></returns>
        public string[] GenerateKeySalt(string plainTxtPass)
        {
            string[] s = new string[2];
 
            string salt = GenerateRandomSalt();
            string passAndSalt = plainTxtPass + salt;
            string hash = GetHashKey(passAndSalt);
 
            s[0] = hash;
            s[1] = salt;
            return s;
        }
 
        #endregion
 
 
        #region Generate random salt
 
        /// <summary>
        /// Generates a cryptographically strong salt string
        /// </summary>
        /// <returns></returns>
        public string GenerateRandomSalt()
        {
            RNGCryptoServiceProvider crypto = new RNGCryptoServiceProvider();
            byte[] saltInBytes = new byte[5];
            crypto.GetBytes(saltInBytes);
 
            return Convert.ToBase64String(saltInBytes);
        }
 
        #endregion
 
        #region Get the hash key for a given string
 
        public string GetHashKey(string input)
        {           
            byte[] tmpSource;
            byte[] tmpHash;
 
            //Convert input to a Byte Array (needed for hash function)
            tmpSource = ASCIIEncoding.ASCII.GetBytes(input);
 
            //Compute hash based on source data.
            tmpHash = new MD5CryptoServiceProvider().ComputeHash(tmpSource);
 
            return Convert.ToBase64String(tmpHash);
        }
 
        #endregion
 
    }
}
