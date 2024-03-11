using System.ComponentModel.DataAnnotations;

namespace WebAPI_JsonWeb_Token_Auth.Dtos
{
    public class UsuarioLoginDto
    {
        [Required(ErrorMessage ="Email obrigatório"), EmailAddress(ErrorMessage ="Email Inválido")]
        public string Email { get; set; }

        [Required(ErrorMessage ="Senha Obrigatória")]
        public string Senha { get; set; }
    }
}
