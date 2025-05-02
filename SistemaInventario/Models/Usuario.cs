namespace SistemaInventario.Models
{
    public class Usuario
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Correo {  get; set; }
        public string Password { get; set; }

        // Referecia con el Modelo Role
        public int RolId { get; set; } // Id del rol Refernciado
        public Role Role {  get; set; } // Referencia del rol
    }
}
