using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebAPI_JsonWeb_Token_Auth.Dtos;
using WebAPI_JsonWeb_Token_Auth.Services.AuthService;

namespace WebAPI_JsonWeb_Token_Auth.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthInterface _authInterface;

        public AuthController(IAuthInterface authInterface)
        {
            _authInterface = authInterface;
        }

        [HttpPost("Login")]

        public async Task<ActionResult> Login(UsuarioLoginDto usuario)
        {

            var resposta = await _authInterface.Login(usuario);
            return Ok(resposta);
        }

        [HttpPost("Register")]

        public async Task<ActionResult> Register(UsuarioCriacaoDto usuarioRegister)
        {

            var resposta = await _authInterface.Registrar(usuarioRegister);
            return Ok(resposta);
        }
    }
}
