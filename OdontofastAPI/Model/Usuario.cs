using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OdontofastAPI.Model
{
    [Table("C_OP_USUARIO")]
    public class Usuario
    {
        [Key]
        [Column("ID_USUARIO")]
        public long IdUsuario { get; set; }

        [Column("NOME_USUARIO")]
        public required string NomeUsuario { get; set; }

        [Column("SENHA_USUARIO")]
        public required string SenhaUsuario { get; set; }

        [Column("EMAIL_USUARIO")]
        public required string EmailUsuario { get; set; }

        [Column("NR_CARTEIRA")]
        public required string NrCarteira { get; set; }

        [Column("TELEFONE_USUARIO")]
        public long TelefoneUsuario { get; set; }
    }
}
