namespace OrderTrack.Models.CustomResponses
{
    public class AuthResponse
    {
        public string Correo { get; set; } = null!;
        public string NombreUsuario { get; set; } = null!;
        public string Token { get; set; } = null!;
        public string OpcionesMenu { get; set; } = null!;   

    }
}
