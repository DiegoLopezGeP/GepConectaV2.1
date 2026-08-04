using System.Data;
using WhatsappComercial.Enums;
using WhatsappComercial.Modelos;

namespace WhatsappComercial.Interfaces.Contactos
{
    public interface IGuardarContacto
    {
        // Obtiene el DataTable vacío con el esquema de la tabla de BD adecuada
        Task<DataTable> ObtenerEsquemaTablaAsync(TipoContactoEnum tipoContacto);

        // Guarda los campos dinámicos construyendo el INSERT mediante parámetros SQL o un Stored Procedure
        Task GuardarContactoDinamicoAsync(TipoContactoEnum tipoContacto, Dictionary<string, object?> datos);
    }
}
