namespace WhatsappComercial.Modelos.DTOs
{
    public class EsquemaFormularioResult
    {
        public List<CampoEsquemaDTO> Campos { get; set; } = new();
        public Dictionary<string, object?> ModeloInicial { get; set; } = new();
    }

    public class ResultadoBusquedaContactoDTO
    {
        public bool Exitoso { get; set; }
        public string Titulo { get; set; } = string.Empty;
        public string Mensaje { get; set; } = string.Empty;
        public bool EsAdvertencia { get; set; }
        public List<Contacto>? BeneficiariosEncontrados { get; set; }
        public int? IdClienteTitularLocal { get; set; }
    }
}
