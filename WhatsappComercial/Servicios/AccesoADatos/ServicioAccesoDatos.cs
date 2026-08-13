using System.Collections;
using System.Data;
using System.Reflection;
using AccesoDatos;
using Microsoft.Extensions.Options;
using WhatsappComercial.Modelos;

namespace WhatsappComercial.Servicios.AccesoADatos
{
    public class ServicioAccesoDatos
    {

        #region Propiedades
        readonly AccesoDatosSoapClient ServicioDatos = new(AccesoDatosSoapClient.EndpointConfiguration.AccesoDatosSoap);
        private readonly Microsoft.Extensions.Options.IOptions<ConfiguracionApp> configuracionAplicacion;
        public int Aplicacion;
        public int AplicacionCRM;
        #endregion

        #region Constructor
        public ServicioAccesoDatos(IOptions<ConfiguracionApp> options)
        {
            configuracionAplicacion = options;
            if (!string.IsNullOrEmpty(configuracionAplicacion.Value.Aplicacion))
            {
                Aplicacion = int.Parse(configuracionAplicacion.Value.Aplicacion);
                AplicacionCRM = int.Parse(configuracionAplicacion.Value.AplicacionCRM);
            }
        }
        #endregion

        #region Consultas

        public DataTable EsquemaTabla(string nombreTabla)
        {
            DataTable tablaEsquema = ServicioDatos.TraerEsquemaTabla(nombreTabla, Aplicacion);
            return tablaEsquema;
        }

        public DataTable TraerConsultaNumero(int nroConsulta)
        {
            DataTable dtConsulta = ServicioDatos.TraerConsultaNumero(nroConsulta, Aplicacion);
            return dtConsulta;
        }

        public DataTable TraerTablaParametros(string tabla, string campos, string condicion)
        {
            DataTable dtConsulta = ServicioDatos.TraerTablaParametros(tabla, campos, condicion, Aplicacion);
            return dtConsulta;
        }

        public DataTable TraerTablaConArreglo(int NroConsulta, string[] parametros)
        {
            DataTable dtConsulta = ServicioDatos.TraerTablaConArreglo(NroConsulta, parametros, Aplicacion);
            return dtConsulta;
        }

        public string TraerPrimeraCelda(int nroConsulta, string[] parametros)
        {
            string resultado = ServicioDatos.TraerPrimeraCelda(nroConsulta, parametros, Aplicacion);
            return resultado;
        }

        public async Task<DataTable> TraerTablaConArregloAsincrono(int NroConsulta, string[] parametros)
        {
            DataTable dtConsulta = await ServicioDatos.TraerTablaConArregloAsync(NroConsulta, parametros, Aplicacion);
            return dtConsulta;
        }

        public DataTable TraerTablaNombre(string nombreTabla)
        {
            DataTable dtConsulta = ServicioDatos.TraerTabla(nombreTabla, Aplicacion);
            return dtConsulta;
        }


        public List<tipoObjeto> TraerListaObjeto<tipoObjeto>(int nroConsulta, string[] parametros) where tipoObjeto : new()
        {
            var lista = new List<tipoObjeto>();
            DataTable dtConsulta = TraerTablaConArreglo(nroConsulta, parametros);
            lista = Convertir.ConvertirDataTableALista<tipoObjeto>(dtConsulta);
            return lista;
        }

        public List<tipoObjeto> TraerListaObjetoSinParametros<tipoObjeto>(int nroConsulta) where tipoObjeto : new()
        {
            var lista = new List<tipoObjeto>();
            DataTable dtConsulta = TraerConsultaNumero(nroConsulta);
            lista = Convertir.ConvertirDataTableALista<tipoObjeto>(dtConsulta);
            return lista;
        }


        public List<tipoObjeto> TraerListaObjetoNombreTabla<tipoObjeto>(string nombreTabla) where tipoObjeto : new()
        {
            var lista = new List<tipoObjeto>();
            DataTable dtConsulta = TraerTablaNombre(nombreTabla);
            lista = Convertir.ConvertirDataTableALista<tipoObjeto>(dtConsulta);
            return lista;
        }

        #endregion

        #region Consultas Afiliaciones
        public DataTable TraerTablaParametrosAfiliaciones(string tabla, string campos, string condicion)
        {
            DataTable dtConsulta = ServicioDatos.TraerTablaParametros(tabla, campos, condicion, AplicacionCRM);
            return dtConsulta;
        }


        #endregion

        #region Inserciones
        public void InsertarTabla(DataTable tablaGrabar)
        {
            ServicioDatos.InsertarTabla(tablaGrabar, tablaGrabar.TableName, Aplicacion);
        }

        public string InsertarRegistroDevuelveConsecutivo(DataTable tablaGrabar)
        {
            string consecutivo = ServicioDatos.InsertarFilaDevuelveConsecutivo(tablaGrabar, tablaGrabar.TableName, Aplicacion);
            return consecutivo;
        }

        public void GrabarLista(object lista, string nombreTabla)
        {
            DataTable tablaEsquema = ServicioDatos.TraerEsquemaTabla(nombreTabla, Aplicacion);

            if (lista is IEnumerable enumerable)
            {
                foreach (var item in enumerable)
                {
                    Type tipo = item.GetType();
                    PropertyInfo[] propiedades = tipo.GetProperties();

                    DataRow registroTabla = tablaEsquema.NewRow();

                    foreach (var prop in propiedades)
                    {
                        if (tablaEsquema.Columns.Contains(prop.Name))
                        {
                            object valor = prop.GetValue(item) ?? DBNull.Value;
                            registroTabla[prop.Name] = valor;
                        }
                    }


                    tablaEsquema.Rows.Add(registroTabla);
                    tablaEsquema.TableName = nombreTabla;
                }

                if (tablaEsquema.Rows.Count > 0)
                {
                    InsertarTabla(tablaEsquema);
                }
            }
            else
            {
                Console.WriteLine("El objeto no es una lista válida.");
            }
        }


        public void GrabarRegistro<T>(T objeto, string nombreTabla)
        {
            try
            {
                DataTable tablaEsquema = ServicioDatos.TraerEsquemaTabla(nombreTabla, Aplicacion);
                DataRow fila = tablaEsquema.NewRow();
                PropertyInfo[] propiedades = objeto.GetType().GetProperties();

                foreach (PropertyInfo propiedad in propiedades)
                {
                    if (tablaEsquema.Columns.Contains(propiedad.Name) && propiedad.CanRead)
                    {
                        object valor = propiedad.GetValue(objeto) ?? DBNull.Value;
                        fila[propiedad.Name] = valor;
                    }
                }

                tablaEsquema.Rows.Add(fila);

                if (tablaEsquema.Rows.Count > 0)
                {
                    InsertarTabla(tablaEsquema);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

        }

        public string GrabarRegistroDevuelveConsecutivo<T>(T objeto, string nombreTabla)
        {
            try
            {
                DataTable tablaEsquema = ServicioDatos.TraerEsquemaTabla(nombreTabla, Aplicacion);

                DataRow fila = tablaEsquema.NewRow();

                PropertyInfo[] propiedades = typeof(T).GetProperties();

                foreach (PropertyInfo propiedad in propiedades)
                {
                    if (tablaEsquema.Columns.Contains(propiedad.Name) && propiedad.CanRead)
                    {
                        object valor = propiedad.GetValue(objeto) ?? DBNull.Value;
                        fila[propiedad.Name] = valor;
                    }
                }

                tablaEsquema.Rows.Add(fila);

                if (tablaEsquema.Rows.Count > 0)
                {
                    return InsertarRegistroDevuelveConsecutivo(tablaEsquema);
                }

                return string.Empty;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }

        public string GrabarCamposCoincidentes<T>(T objeto, string nombreTabla)
        {
            DataTable tablaEsquema = ServicioDatos.TraerEsquemaTabla(nombreTabla, Aplicacion);
            Type tipo = objeto.GetType();
            PropertyInfo[] propiedades = tipo.GetProperties(BindingFlags.Public | BindingFlags.Instance);

            DataRow row = tablaEsquema.NewRow();
            foreach (PropertyInfo prop in propiedades)
            {
                if (tablaEsquema.Columns.Contains(prop.Name))
                {
                    object valor = prop.GetValue(objeto, null) ?? DBNull.Value;
                    row[prop.Name] = valor;
                }
            }

            tablaEsquema.Rows.Add(row);
            string consecutivo = InsertarRegistroDevuelveConsecutivo(tablaEsquema);
            return consecutivo;
        }

        public async Task InsertarEnBloque(DataTable dtGrabar, string nombreTabla)
        {
            await ServicioDatos.InsertarTablaEnBloqueAsync(dtGrabar, nombreTabla, Aplicacion);
        }

        #endregion

        #region Actualizaciones
        public void ActualizarTabla(DataTable tablaActualizar)
        {
            ServicioDatos.ActualizarFilas(tablaActualizar, Aplicacion);
        }

        public void ActualizarCampo(string tabla, string campo, string valor, string condicion)
        {
            ServicioDatos.ActualizarCampo(tabla, campo, valor, condicion, Aplicacion);
        }

        public void ActualizarRegistro<T>(T objeto, string nombreTabla)
        {
            DataTable tablaEsquema = ServicioDatos.TraerEsquemaTabla(nombreTabla, Aplicacion);
            DataRow fila = tablaEsquema.NewRow();
            PropertyInfo[] propiedades = typeof(T).GetProperties();

            foreach (PropertyInfo propiedad in propiedades)
            {
                if (tablaEsquema.Columns.Contains(propiedad.Name) && propiedad.CanRead)
                {
                    object valor = propiedad.GetValue(objeto) ?? DBNull.Value;
                    fila[propiedad.Name] = valor;
                }
            }

            tablaEsquema.Rows.Add(fila);

            if (tablaEsquema.Rows.Count > 0)
            {
                ActualizarTabla(tablaEsquema);
            }
        }

        public void ActualizarCamposCoincidentes<T>(T objeto, string nombreTabla)
        {
            DataTable tablaEsquema = ServicioDatos.TraerEsquemaTabla(nombreTabla, Aplicacion);

            Type tipo = objeto.GetType();
            PropertyInfo[] propiedades = tipo.GetProperties(BindingFlags.Public | BindingFlags.Instance);

            DataRow row = tablaEsquema.NewRow();

            foreach (PropertyInfo prop in propiedades)
            {
                if (tablaEsquema.Columns.Contains(prop.Name))
                {
                    object valor = prop.GetValue(objeto, null) ?? DBNull.Value;
                    row[prop.Name] = valor;
                }
            }

            tablaEsquema.Rows.Add(row);
            ActualizarTabla(tablaEsquema);
        }

        public void ActualizarLista(object lista, string nombreTabla)
        {
            DataTable tablaEsquema = ServicioDatos.TraerEsquemaTabla(nombreTabla, Aplicacion);

            if (lista is IEnumerable enumerable)
            {
                foreach (var item in enumerable)
                {
                    Type tipo = item.GetType();
                    PropertyInfo[] propiedades = tipo.GetProperties();

                    DataRow registroTabla = tablaEsquema.NewRow();

                    foreach (var prop in propiedades)
                    {
                        if (tablaEsquema.Columns.Contains(prop.Name))
                        {
                            object valor = prop.GetValue(item) ?? DBNull.Value;
                            registroTabla[prop.Name] = valor;
                        }
                    }


                    tablaEsquema.Rows.Add(registroTabla);
                    tablaEsquema.TableName = nombreTabla;
                }

                if (tablaEsquema.Rows.Count > 0)
                {
                    ActualizarTabla(tablaEsquema);
                }
            }
            else
            {
                Console.WriteLine("El objeto no es una lista válida.");
            }
        }

        public void ActualizarCamposEspecificos<T>(T objeto, string nombreTabla, string[] arregloCampos, string nombreLlave)
        {
            PropertyInfo[] propiedades = typeof(T).GetProperties();
            string valorLlave = propiedades.FirstOrDefault(p => p.Name.Equals(nombreLlave, StringComparison.OrdinalIgnoreCase))?.GetValue(objeto)?.ToString();
            for (int campoIndex = 0; campoIndex < arregloCampos.Length; campoIndex++)
            {
                string campo = arregloCampos[campoIndex];
                string valor = propiedades.FirstOrDefault(p => p.Name.Equals(campo, StringComparison.OrdinalIgnoreCase)).GetValue(objeto)?.ToString();
                ActualizarCampo(nombreTabla, campo, valor, $"{nombreLlave}={valorLlave}");
            }
        }

        #endregion

        #region Eliminaciones
        public void EliminarRegistro(DataTable registroEliminar)
        {
            ServicioDatos.EliminarFila(registroEliminar, Aplicacion);
        }
        public void EliminarDato<T>(T objeto, string nombreTabla)
        {
            DataTable tablaEsquema = ServicioDatos.TraerEsquemaTabla(nombreTabla, Aplicacion);
            DataRow fila = tablaEsquema.NewRow();
            PropertyInfo[] propiedades = typeof(T).GetProperties();

            foreach (PropertyInfo propiedad in propiedades)
            {
                if (tablaEsquema.Columns.Contains(propiedad.Name) && propiedad.CanRead)
                {
                    object valor = propiedad.GetValue(objeto) ?? DBNull.Value;
                    fila[propiedad.Name] = valor;
                }
            }

            tablaEsquema.Rows.Add(fila);

            if (tablaEsquema.Rows.Count > 0)
            {
                EliminarRegistro(tablaEsquema);
            }
        }

        public void EliminarId(string tabla, string campo, string llaves)
        {
            ServicioDatos.EliminarListadeLlaves(tabla, campo, llaves, Aplicacion);
        }

        #endregion
    }
}
