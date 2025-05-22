using Microsoft.EntityFrameworkCore;
using OdontofastAPI.Configuration; // Adicionando namespace para a configuração do ML.NET
using OdontofastAPI.Data;
using OdontofastAPI.Repository;
using OdontofastAPI.Repository.Implementations;
using OdontofastAPI.Repository.Interfaces;
using OdontofastAPI.Service.Implementations;
using OdontofastAPI.Service.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Configuração do banco de dados Oracle.
// O AddDbContext adiciona o contexto de dados ao contêiner de DI, configurando o acesso ao banco de dados.
// UseOracle é o método que configura o Entity Framework para se conectar ao Oracle, usando a string de conexão "OracleConnection" do arquivo de configuração.
builder.Services.AddDbContext<OdontofastDbContext>(options =>
    options.UseOracle(builder.Configuration.GetConnectionString("OracleConnection")));

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()  // Permite qualquer origem
              .AllowAnyMethod()  // Permite qualquer método HTTP
              .AllowAnyHeader();  // Permite qualquer cabeçalho
    });
});

// Registro de dependências para o repositório e serviço de Usuário no container de injeção de dependência (DI).
// AddScoped garante que uma nova instância será criada para cada requisição HTTP.
builder.Services.AddScoped<IProgressoService, ProgressoService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();
builder.Services.AddScoped<IUsuarioService, UsuarioService>();
builder.Services.AddScoped<IImagemUsuarioRepository, ImagemUsuarioRepository>();
builder.Services.AddScoped<IImagemUsuarioService, ImagemUsuarioService>();
// Registro do ConfiguracaoService como um Singleton, o que significa que a mesma instância será utilizada em toda a aplicação.
// Usado, pois o Singleton é adequado para serviços que não precisam ser criados várias vezes e são reutilizados ao longo do ciclo de vida da aplicação.
builder.Services.AddSingleton<IConfiguracaoService>(provider =>
{
    var configuration = provider.GetRequiredService<IConfiguration>();
    return ConfiguracaoService.GetInstance(configuration);
});

// Adiciona serviços de ML.NET ao contêiner de DI
builder.Services.AddMLNetServices();

// Configuração dos controllers, que são usados para mapear os endpoints da API.
builder.Services.AddControllers();

// Habilitação do Swagger para gerar e exibir documentação da API. O Swagger é escolhido, pois facilita a interação com a API durante o desenvolvimento e a geração de documentação.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Criação da instância da aplicação (app) a partir das configurações do builder.
var app = builder.Build();

// Inicializa os modelos de ML.NET após construir a aplicação
// O parâmetro 'false' indica que não será forçado o treinamento dos modelos se já existirem
app.Services.InitializeMLModels(false);

// Condicional que verifica se o ambiente da aplicação é de desenvolvimento.
// Se for, o Swagger será habilitado para gerar a documentação interativa da API.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger(); // Habilita o Swagger
    app.UseSwaggerUI(); // Habilita a interface do Swagger UI
}

// Use CORS
app.UseCors("AllowAll");

// Configuração da autorização na aplicação. Isso é necessário para controlar o acesso aos recursos protegidos da API.
app.UseAuthorization();

// Mapeamento dos controllers para que o ASP.NET Core saiba como lidar com as rotas da API.
app.MapControllers();

// Inicia o servidor web e executa a aplicação.
app.Run();