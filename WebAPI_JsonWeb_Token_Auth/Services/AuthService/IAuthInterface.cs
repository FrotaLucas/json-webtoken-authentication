using WebAPI_JsonWeb_Token_Auth.Dtos;
using WebAPI_JsonWeb_Token_Auth.Models;

namespace WebAPI_JsonWeb_Token_Auth.Services.AuthService
{
    public interface IAuthInterface
    {

        Task<Response<UsuarioCriacaoDto>> Registrar(UsuarioCriacaoDto usuarioRegistro);

        Task<Response<string>> Login(UsuarioLoginDto usuarioLogin);


    }
}
