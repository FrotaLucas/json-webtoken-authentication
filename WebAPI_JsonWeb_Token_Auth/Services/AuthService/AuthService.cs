using Microsoft.EntityFrameworkCore;
using WebAPI_JsonWeb_Token_Auth.DataContext;
using WebAPI_JsonWeb_Token_Auth.Dtos;
using WebAPI_JsonWeb_Token_Auth.Models;
using WebAPI_JsonWeb_Token_Auth.Services.SenhaSerice;

namespace WebAPI_JsonWeb_Token_Auth.Services.AuthService
{
    //Duvida:pq AuthSerice que herda IAuthService ? 
    //O Programa deveria acessar IAuthService que por sua ver herda AuthService.Nao ?
    public class AuthService : IAuthInterface
    {
        private readonly AppDbContext _context;
        private readonly ISenhaInterface _senhaInterface;

        public AuthService(AppDbContext context,ISenhaInterface senhaInterface)
        {
            _context = context;
            _senhaInterface = senhaInterface;
        }
        public async Task<Response<UsuarioCriacaoDto>> Registrar(UsuarioCriacaoDto usuarioRegistro)
        {
            Response<UsuarioCriacaoDto> respostaServico = new Response<UsuarioCriacaoDto>();
            
            try
            {
                if(VerificaSeEmaileUsuarioJaExiste(usuarioRegistro)) 
                {
                    respostaServico.Dados = null;
                    respostaServico.Status = false;
                    respostaServico.Mensagem = "Email/Usuário já cadastrados!";

                    return (respostaServico);
                }

                //Duvida senhaHash e senhaSalt sao vetores ? pq essa notacao [] ??
                _senhaInterface.CriarSenhaHash(usuarioRegistro.Senha, out byte[] senhaHash, out byte[] senhaSalt);
                Console.WriteLine("senhasalt after calling:" + senhaSalt);
                UsuarioModel novoUsuario = new UsuarioModel()
                {
                    Usuario = usuarioRegistro.Usuario,
                    Email = usuarioRegistro.Email,
                    Cargo = usuarioRegistro.Cargo,
                    SenhaHash = senhaHash,
                    SenhaSalt = senhaSalt
                };


                _context.Add(novoUsuario);
                //tb funcioana
                //_context.Usuario.Add(novoUsuario);
                await _context.SaveChangesAsync();

                respostaServico.Mensagem = "Usuário cadastrado com sucesso";
            }


            catch (Exception ex)
            {
              respostaServico.Dados = null; 
              respostaServico.Mensagem = ex.Message;
              respostaServico.Status = false;
            }

            return respostaServico;
        }

        public async Task<Response<string>> Login (UsuarioLoginDto usuarioLogin)
        {
            Response<string> resposta = new Response<string>();


            try
            {
                var usuario = await _context.Usuario.FirstOrDefaultAsync(user => user.Email == usuarioLogin.Email);

                if( usuario == null)
                {
                    resposta.Mensagem = "Credenciais Inválidas";
                    resposta.Status = false;
                    return resposta;
                }


                //Pq precisa daquela notacao com byte[] senhaHahs e byte[] senhaSalt la na definicao dessa funcao ??? 
                if (!_senhaInterface.VerificaSenhaHash(usuarioLogin.Senha, usuario.SenhaHash, usuario.SenhaSalt))
                {
                    resposta.Mensagem = "Credenciais Inválidas";
                    resposta.Status = false;
                    return resposta;
                }


                string token = _senhaInterface.CriarToken(usuario);

                resposta.Dados = token;
                resposta.Mensagem = "Usuario logado com sucesso";

            }

            catch (Exception ex)
            {
                resposta.Dados = null;
                resposta.Mensagem = ex.Message;
                resposta.Status = false;
                
            }
            //esse retorno eh uma string, mas o formato que aparece eh como se fosse um json
                return resposta;
        }
        public bool VerificaSeEmaileUsuarioJaExiste(UsuarioCriacaoDto usuarioRegistro)
        {
            var usuario =_context.Usuario.FirstOrDefault(userBanco => userBanco.Email == usuarioRegistro.Email || userBanco.Usuario == usuarioRegistro.Usuario);

            if (usuario != null)
            {
                return true;
            }

            return false;
        
        }
    }
}
