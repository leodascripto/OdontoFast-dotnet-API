namespace OdontofastAPI.DTO
{
    public class UsuarioDTO
    {
        public long IdUsuario { get; set; }
        public required string NomeUsuario { get; set; }
        public required string SenhaUsuario { get; set; }
        public required string EmailUsuario { get; set; }
        public required string NrCarteira { get; set; }
        public long TelefoneUsuario { get; set; }
    }
}
