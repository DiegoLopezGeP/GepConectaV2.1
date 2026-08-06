using WhatsappComercial.Enums;
using WhatsappComercial.Modelos;
using WhatsappComercial.Modelos.DTOs;

namespace WhatsappComercial.Interfaces.Contactos
{
    public interface IFormularioContactoService
    {
        // Genera el esquema de campos a partir del tipo de contacto
        EsquemaFormularioResult GenerarEsquema(TipoContactoEnum tipo);

        // Procesa la validación en blur según el tipo de contacto
        Task<ResultadoBusquedaContactoDTO> ProcesarValidacionBlurAsync(
            TipoContactoEnum tipo,
            CampoEsquemaDTO campo,
            Dictionary<string, object?> modeloDinamico);

        // Mapea la selección manual de un beneficiario al modelo dinámico
        void MapearBeneficiarioSeleccionado(
            object idContacto,
            List<Contacto> beneficiarios,
            int? idClienteTitularLocal,
            Dictionary<string, object?> modeloDinamico);
    }
}
