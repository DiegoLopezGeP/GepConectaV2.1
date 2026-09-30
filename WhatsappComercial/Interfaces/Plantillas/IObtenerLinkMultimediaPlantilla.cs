namespace WhatsappComercial.Interfaces.Plantillas
{
    public interface IObtenerLinkMultimediaPlantilla
    {
        Task<string> ObtenerLinkPlantilla(string idPlantilla);
    }
}
