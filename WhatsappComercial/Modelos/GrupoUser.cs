namespace WhatsappComercial.Modelos
{
    public class GrupoUser
    {
        public int IdGrupoUser { get; set; }
        public string NameUser { get; set; }
        public int IdGrupoTrabajo { get; set; }
        public int IdRol { get; set; }
        public string NombreUsua { get; set; }
        public bool Estado { get; set; }
        public int PermisoConversacion { get; set; }
    }
}
