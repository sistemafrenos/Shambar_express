using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HK.Clases
{
    public class FactoryContadores
    {
        public static int GetContador(string Variable)
        {
            try
            {
                using (var oEntidades = new FeriaEntities())
                {
                    Contadore Contador = oEntidades.Contadores.FirstOrDefault(x => x.Variable == Variable);
                    if (Contador == null)
                    {
                        Contador = new Contadore();
                        Contador.Variable = Variable;
                        Contador.Valor = 1;
                        oEntidades.Contadores.AddObject(Contador);
                    }
                    else
                    {
                        Contador.Valor++;
                        if (Contador.Valor > 99)
                        {
                            Contador.Valor = 1;
                        }
                    }
                    oEntidades.SaveChanges();
                    return Contador.Valor.GetValueOrDefault(0);
                }
            }
            catch (Exception ex)
            {
                string x = ex.Message;
            }
            return 1;
        }
        public static string GetMax(string Variable)
        {
            try
            {
                using (var oEntidades = new FeriaEntities())
                {
                    Contadore Contador = oEntidades.Contadores.FirstOrDefault(x => x.Variable == Variable);
                    if (Contador == null)
                    {
                        Contador = new Contadore();
                        Contador.Variable = Variable;
                        Contador.Valor = 1;
                        oEntidades.Contadores.AddObject(Contador);
                    }
                    else
                    {                        
                        Contador.Valor++;

                    }
                    oEntidades.SaveChanges();
                    return ((int)Contador.Valor).ToString("000000");
                }
            }
            catch (Exception ex)
            {
                string x = ex.Message;
            }
            return "";
        }

        //public static string GetMax(string Id)
        //{
        //    try
        //    {
        //        string q = null;
        //        Int32? q2 = null;
        //        using (var db = new FeriaEntities())
        //        {
        //            switch (Id)
        //            {
        //                case "IdCajero":
        //                    q = (from p in db.Cajeros
        //                         select p.IdCajero).Max();
        //                    break;
        //                case "IdFactura":
        //                    q = (from p in db.Facturas
        //                         select p.IdFactura).Max();
        //                    break;
        //                case "NumeroFactura":
        //                    q = (from p in db.Facturas
        //                         select p.Numero).Max();
        //                    break;
        //                case "IdPlato":
        //                    q = (from p in db.Platos
        //                         select p.IdPlato).Max();
        //                    break;
        //                case "IdFacturaPlato":
        //                    q = (from p in db.FacturasPlatos
        //                         select p.IdFacturaPlato).Max();
        //                    break;
        //                default:
        //                    throw new Exception("No imprementado contador " + Id);
        //            }
        //            Int32 x;
        //            if (q != null && Int32.TryParse(q, out x))
        //                return (Convert.ToInt32(q) + 1).ToString();
        //            else
        //                return "1";
        //        }
        //    }
        //    catch (Exception x)
        //    {
        //        throw x;
        //    }
        //}
    }
}
