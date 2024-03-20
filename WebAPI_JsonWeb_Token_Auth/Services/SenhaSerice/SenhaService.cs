using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using WebAPI_JsonWeb_Token_Auth.Models;

namespace WebAPI_JsonWeb_Token_Auth.Services.SenhaSerice
{
    public class SenhaService : ISenhaInterface
    {
        private readonly IConfiguration _config;

        public SenhaService(IConfiguration config)
        {
            _config = config;
        }
        public void CriarSenhaHash(string senha, out byte[] senhaHash, out byte[] senhaSalt)
        {

            //PQ eu ´preciso usar esse using aqui ???
            using (var hmac = new HMACSHA512())
            {
                senhaSalt = hmac.Key;
                senhaHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(senha));


                //EXPLICACAO TEORICA
                //senhaSalt e senhaHash sao arrays de bites que cada elemento da senha ocupa na memoria
                
                //printa esse mini tamanho de memoria em base decimal
                Console.WriteLine("SenhaSalt: "+ senhaSalt[3]);
                Console.WriteLine("SenhaHash: "+ senhaHash[3]);
                //printa esse mini tamanho de memoria em base hexadecimal
                Console.WriteLine("SenhaSalt: " + senhaSalt[3].ToString("X2"));

                //pega todos os  valores em bites e transforma no que equivale em Exadecimal.No final temos a senhaSalt
                //completa que ta no banco dados.
                Console.WriteLine("SenhaSalt: " + string.Join("", senhaSalt.Select(b => b.ToString("X2"))));

                //nao significa nada.Printa System.Byte[]
                Console.WriteLine("SenhaSalt: "+ senhaSalt);

            }

        }


        public bool VerificaSenhaHash(string senha, byte[] senhaHash, byte[] senhaSalt)
        {
            using (var hmac = new HMACSHA512(senhaSalt))
            {
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(senha));
                return computedHash.SequenceEqual(senhaHash);

            }

        }

        public string CriarToken(UsuarioModel usuario)
        {
            //essa claim sao os dados do usuario encriptografado ???
            List<Claim> claims = new List<Claim>()
            {
                new Claim("Cargo", usuario.Cargo.ToString()),
                new Claim("Email", usuario.Email),
                new Claim("Username", usuario.Usuario)
            };

            //Essa key foi criada um unica vez no projeto inteiro. Esta em appsetting.json

            var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(_config.GetSection("AppSettings:Token").Value));

            //HmacSha512Signature aqui eh o algoritmo de criptografia da key acima
            var cred = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            //junta a key enctrptografada com com as clains
            var token = new JwtSecurityToken(
                    claims: claims,
                    expires: DateTime.Now.AddDays(1),
                    signingCredentials: cred
                );

            //transforma em string o token 

            var jwt = new JwtSecurityTokenHandler().WriteToken(token);


            return jwt;


        }

    }
}
