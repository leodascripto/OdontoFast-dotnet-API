namespace OdontofastAPI.Model
{
    public class LoginDto
    {
        public required string NrCarteira { get; set; }  // Propriedade para o número da carteira
        public required string Senha { get; set; }  // Propriedade para a senha
    }
}
