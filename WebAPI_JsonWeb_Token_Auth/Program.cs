using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;
using System.Text;
using WebAPI_JsonWeb_Token_Auth.DataContext;
using WebAPI_JsonWeb_Token_Auth.Services.AuthService;
using WebAPI_JsonWeb_Token_Auth.Services.SenhaSerice;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IAuthInterface, AuthService>();
builder.Services.AddScoped<ISenhaInterface, SenhaService>();

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});


//criando botao para receber Token que libera o methodo criado no Controller UsuarioController
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
    {
        Description = "aqui temos Standard Authorization header using the Bearer scheme (\"bearer {token}\")",
        In = ParameterLocation.Header,
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey

    });


    options.OperationFilter<SecurityRequirementsOperationFilter>();

});


//Parte de funcionalidade do botao.Como usar: Copiar o token que vem pelo payload da api Login e colar
//no campo value dentro do botao Authorize. O Servidor avalia se o client tem permissao para acessar aquela informacao. 

// DUVIDA
//a senha que coloquei fixa dentro do appsetting esta sendo lida aqui.
//Pq as outros componentes que formam o token nao estao sendo recebidos aqui?? 
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration.GetSection("AppSettings:Token").Value)),
        ValidateAudience = false,
        ValidateIssuer = false

    };
});



var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

//Precisa adicionar autencication
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
