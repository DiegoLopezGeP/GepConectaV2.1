using System.Data;
using WhatsappComercial.Enums;
using WhatsappComercial.Interfaces.Contactos;
using WhatsappComercial.Modelos;
using WhatsappComercial.Servicios.AccesoADatos;

namespace WhatsappComercial.Servicios.Contactos
{
    public class GuardarContacto : IGuardarContacto
    {
        private readonly ServicioAccesoDatos _servicioAccesoDatos;
        private readonly IHelperContacto _helperContacto;

        public GuardarContacto(ServicioAccesoDatos servicioAccesoDatos, IHelperContacto helperContacto)
        {
            _servicioAccesoDatos = servicioAccesoDatos;
            _helperContacto = helperContacto;
        }

        public async Task GuardarContactoDinamicoAsync(TipoContactoEnum tipoContacto, Dictionary<string, object?> datos)
        {
            if (datos == null || !datos.Any())
                throw new ArgumentException("No hay datos para guardar.");

            string nombreTabla = _helperContacto.ObtenerNombreTabla(tipoContacto);

            // 1. Filtrar solo los campos que tengan un valor asignado o sean explícitamente procesados
            var camposValidos = datos.Where(kvp => kvp.Value != null).ToList();

            if (!camposValidos.Any())
                throw new InvalidOperationException("Todos los campos del formulario se encuentran vacíos.");

            // 2. Construir la consulta INSERT dinámica
            // Ejemplo: INSERT INTO ContactoTitular (NombreContacto, Celular) VALUES (@NombreContacto, @Celular)
            _servicioAccesoDatos.GrabarRegistro(datos, nombreTabla);
        }

        public async Task<DataTable> ObtenerEsquemaTablaAsync(TipoContactoEnum tipoContacto)
        {
            string nombreTabla = _helperContacto.ObtenerNombreTabla(tipoContacto);

            DataTable dtEsquemaTabla = _servicioAccesoDatos.TraerTablaNombre(nombreTabla);

            return dtEsquemaTabla;
        }

    }
}
