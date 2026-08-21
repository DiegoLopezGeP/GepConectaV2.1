using WhatsappComercial.Enums;
using WhatsappComercial.Interfaces.Contactos;
using WhatsappComercial.Modelos;
using WhatsappComercial.Modelos.DTOs;
using WhatsappComercial.Servicios.AccesoADatos;

namespace WhatsappComercial.Servicios.Contactos
{
    public class ActualizarContactoService : IActualizarContacto
    {
        private readonly ServicioAccesoDatos _servicioAccesoDatos;
        private readonly IHelperContacto _helperContacto;

        public ActualizarContactoService(ServicioAccesoDatos servicioAccesoDatos, IHelperContacto helperContacto)
        {
            _servicioAccesoDatos = servicioAccesoDatos;
            _helperContacto = helperContacto;
        }
        public async Task ActualizarContactoDinamicoAsync(TipoContactoEnum tipoContacto, Dictionary<string, object?> datos)
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

            var objetoCreado = ConstruirObjeto(tipoContacto, datos.ToDictionary(kvp => kvp.Key, kvp => kvp.Value));

            _servicioAccesoDatos.ActualizarRegistro(objetoCreado, nombreTabla);
        }
        public object ConstruirObjeto(TipoContactoEnum tipoContacto, Dictionary<string, object?> datos)
        {
            try
            {
                return tipoContacto switch
                {
                    TipoContactoEnum.Titular => MapearDiccionarioAObjeto<Clientes>(datos),
                    TipoContactoEnum.Beneficiario => MapearDiccionarioAObjeto<Beneficiario>(datos),
                    TipoContactoEnum.Relacionista => MapearDiccionarioAObjeto<Contacto>(datos),
                    TipoContactoEnum.Pagadurias => MapearDiccionarioAObjeto<Pagaduria>(datos),
                    TipoContactoEnum.Cobranzas => MapearDiccionarioAObjeto<BaseCobranzas>(datos),
                    _ => throw new ArgumentOutOfRangeException(nameof(tipoContacto), $"Tipo de contacto no soportado: {tipoContacto}")
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al construir objeto para {tipoContacto}: {ex.Message}");
                throw;
            }
        }
        private TObjeto MapearDiccionarioAObjeto<TObjeto>(Dictionary<string, object?> datos) where TObjeto : class, new()
        {
            var instancia = new TObjeto();
            var propiedades = typeof(TObjeto).GetProperties();

            foreach (var prop in propiedades)
            {
                // Busca la clave en el diccionario ignorando mayúsculas/minúsculas
                var kvp = datos.FirstOrDefault(d => d.Key.Equals(prop.Name, StringComparison.OrdinalIgnoreCase));

                if (kvp.Key != null && kvp.Value != null && prop.CanWrite)
                {
                    try
                    {
                        var tipoPropiedad = Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType;

                        // Manejo especial si la propiedad es un Enum
                        object valorConvertido;
                        if (tipoPropiedad.IsEnum)
                        {
                            valorConvertido = Enum.Parse(tipoPropiedad, kvp.Value.ToString()!);
                        }
                        else
                        {
                            valorConvertido = Convert.ChangeType(kvp.Value, tipoPropiedad);
                        }

                        prop.SetValue(instancia, valorConvertido);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error al mapear la propiedad '{prop.Name}': {ex.Message}");
                    }
                }
            }

            return instancia;
        }
    }
}
