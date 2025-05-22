using System.ComponentModel.DataAnnotations;

namespace OdontofastAPI.DTO
{
  public class ImagemUsuarioDTO
  {
    public long IdUsuario { get; set; }

    [Required(ErrorMessage = "O caminho da imagem é obrigatório")]
    [StringLength(500, ErrorMessage = "O caminho da imagem não pode exceder 500 caracteres")]
    public required string CaminhoImg { get; set; }
  }

  public class ImagemUsuarioCreateDTO
  {
    [Required(ErrorMessage = "O ID do usuário é obrigatório")]
    public long IdUsuario { get; set; }

    [Required(ErrorMessage = "O caminho da imagem é obrigatório")]
    [StringLength(500, ErrorMessage = "O caminho da imagem não pode exceder 500 caracteres")]
    public required string CaminhoImg { get; set; }
  }

  public class ImagemUsuarioUpdateDTO
  {
    [Required(ErrorMessage = "O caminho da imagem é obrigatório")]
    [StringLength(500, ErrorMessage = "O caminho da imagem não pode exceder 500 caracteres")]
    public required string CaminhoImg { get; set; }
  }
}