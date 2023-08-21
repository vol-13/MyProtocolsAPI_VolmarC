namespace MyProtocolsAPI_VolmarC.ModelsDTOs
{
    public class UserDTO
    {
        //a modo de ejemplo los atributos del modelo estarán en español

        //un escenario como este puede servir para que un equipo de trabajo del app
        //que solo sepa español, entienda de qué trata 

        public int IDUsuario { get; set; }
        public string Correo { get; set; } = null!;
        public string Contrasennia { get; set; } = null!;
        public string Nombre { get; set; } = null!;
        public string CorreoRespaldo { get; set; } = null!;
        public string Telefono { get; set; } = null!;
        public string? Direccion { get; set; }
        public bool? Activo { get; set; }
        public bool? EstaBloqueado { get; set; }
        public int IDRol { get; set; }
        public string DescripcionRol { get; set; } = null!;

    }

}
