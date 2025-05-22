using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OdontofastAPI.Model
{
  [Table("C_OP_IMG_USUARIO")]
  public class ImagemUsuario
  {
    [Key]
    [Column("ID_USUARIO")]
    public long IdUsuario { get; set; }

    [Column("CAMINHO_IMG")]
    [Required]
    public required string CaminhoImg { get; set; }

    // Propriedade de navegação para o usuário
    [ForeignKey("IdUsuario")]
    public virtual Usuario? Usuario { get; set; }
  }
}