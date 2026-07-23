using System.Data;

namespace WhatsappComercial.Servicios.ConfiguracionEstatica
{
    public class ConfiguracionEstaticaApp
    {
        public ConfiguracionEstaticaApp()
        {

        }

        private static DataTable? Configuracion;
        private static DataTable? EsquemaMensaje;
        public void EstablecerConfiguracion(DataTable configuracion)
        {

            Configuracion = configuracion;
        }
        public DataTable TraerConfiguracion() => Configuracion;
        public void EstablecerEsquema(DataTable Esquema)
        {
            EsquemaMensaje = Esquema;
        }
        public string TraerConfiguracionPorCondicion(string condicion)
        {
            if (Configuracion == null || Configuracion.Rows.Count == 0)
                return string.Empty;

            return Configuracion
                .AsEnumerable()
                .Where(row => row.Field<string>("IdentificadorConfiguracion") == condicion)
                .Select(row => row.Field<string>("ValorConfiguracion"))
                .FirstOrDefault() ?? string.Empty;
        }
    }
}
