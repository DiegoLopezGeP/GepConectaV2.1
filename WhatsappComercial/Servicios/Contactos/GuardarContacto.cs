using WhatsappComercial.Interfaces.Contactos;
using WhatsappComercial.Modelos;
using WhatsappComercial.Servicios.AccesoADatos;

namespace WhatsappComercial.Servicios.Contactos
{
    public class GuardarContacto : IGuardarContacto
    {
        private readonly ServicioAccesoDatos _servicioAccesoDatos;

        public GuardarContacto(ServicioAccesoDatos servicioAccesoDatos)
        {
            _servicioAccesoDatos = servicioAccesoDatos;
        }
        public async Task GuardarContactoAsync(Contacto contacto)
        {
			try
			{
                 _servicioAccesoDatos.GrabarRegistro(contacto, "Contactos");

            }
			catch (Exception ex)
			{

				throw;
			}
        }
    }
}
