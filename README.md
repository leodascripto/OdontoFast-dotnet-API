# Odontofast API

Bem-vindo ao repositório da **Odontofast API**, uma API desenvolvida em .NET para integração com um aplicativo mobile. Esta API é responsável por gerenciar autenticação e operações CRUD de usuários, persistindo os dados em um banco de dados Oracle.

---

## Explicação da Arquitetura

A arquitetura da Odontofast API segue o padrão **Clean Architecture** com camadas bem definidas, promovendo separação de responsabilidades e facilitando a manutenção e escalabilidade do código. As principais camadas são:

1. **Controllers**: Camada de apresentação que expõe os endpoints da API e lida com as requisições HTTP.
2. **Services**: Camada de lógica de negócio que coordena as operações entre os controllers e os repositórios.
3. **Repositories**: Camada de acesso a dados que interage diretamente com o banco de dados via Entity Framework Core.
4. **Models/DTOs**: Estruturas de dados que representam as entidades do banco e os objetos transferidos entre camadas.
5. **Data**: Configuração do contexto do banco de dados com Entity Framework Core para conexão com o Oracle.

A API utiliza o **Entity Framework Core** como ORM para mapeamento objeto-relacional e conexão com o banco de dados Oracle. A injeção de dependência (DI) é configurada no `Program.cs` para gerenciar as instâncias dos serviços e repositórios.

### Escolha da Arquitetura: Monolítica vs. Microservices

A Odontofast API foi projetada seguindo uma abordagem **monolítica**, onde todos os componentes (autenticação, CRUD de usuários, etc.) estão contidos em uma única aplicação. A escolha por uma arquitetura monolítica foi baseada nos seguintes fatores:

- **Simplicidade**: O escopo inicial do projeto é relativamente pequeno e focado em funcionalidades específicas (login e gerenciamento de usuários). Uma abordagem monolítica reduz a complexidade de implementação e deploy.
- **Integração com Mobile**: Como a API serve como backend direto para um aplicativo mobile, uma única aplicação facilita a integração e a manutenção de uma interface coesa.
- **Recursos Limitados**: Considerando que o desenvolvimento está sendo feito por uma equipe pequena, uma arquitetura monolítica exige menos esforço em termos de gerenciamento de serviços distribuídos, comunicação entre microsserviços e monitoramento.
- **Escalabilidade Inicial**: Para a fase inicial, a escalabilidade vertical (aumentar recursos do servidor) é suficiente. Caso o projeto cresça, a arquitetura pode ser refatorada para microservices no futuro.

**Justificativa contra Microservices**: Uma abordagem de microservices seria mais adequada para sistemas maiores, com múltiplos domínios de negócio e equipes independentes. No entanto, isso traria complexidades adicionais, como a necessidade de orquestração (ex.: Kubernetes), comunicação entre serviços (ex.: gRPC ou RabbitMQ) e maior overhead operacional, o que não se justifica para o escopo atual.

---

## Implementação da API Seguindo a Arquitetura Escolhida

A API foi implementada como uma aplicação monolítica em .NET, utilizando o framework ASP.NET Core. As principais características da implementação incluem:

- **Estrutura Monolítica**: Todos os endpoints (login e CRUD de usuários) estão no mesmo projeto, compartilhando o mesmo contexto de banco de dados (`OdontofastDbContext`) e configurações (como CORS e DI).
- **Separação de Camadas**: Apesar de ser monolítica, a aplicação mantém uma clara separação entre camadas (Controllers, Services, Repositories), o que facilita a evolução para microservices, se necessário.
- **Diferenças em Relação a Microservices**: Em uma arquitetura de microservices, cada funcionalidade (ex.: autenticação e gerenciamento de usuários) seria um serviço independente com seu próprio banco de dados e deploy. Na abordagem monolítica, há um único ponto de entrada (`Program.cs`) e um único banco de dados Oracle, o que simplifica a configuração, mas pode limitar a escalabilidade horizontal em cenários de alta carga, porém, é um aperfeiçoamento futuro.

A implementação atual reflete a escolha monolítica ao centralizar a lógica no mesmo codebase, mas com modularidade suficiente para suportar uma futura refatoração.

---

## Design Patterns Utilizados

- **Repository Pattern**: Utilizado na camada de acesso a dados para abstrair as operações no banco e facilitar a substituição do mecanismo de persistência, se necessário.
- **Dependency Injection (DI)**: Aplicado para injetar dependências nos controllers e serviços, promovendo baixo acoplamento e testabilidade.
- **Singleton**: Usado no `ConfiguracaoService` para garantir uma única instância do serviço de configuração ao longo da aplicação.
- **DTO (Data Transfer Object)**: Implementado para transferir dados entre camadas, evitando a exposição direta das entidades do banco.

---

## Instruções para Rodar a API

### Pré-requisitos
- **.NET 8 SDK** instalado (ou a versão compatível com o projeto).
- Banco de dados **Oracle** configurado.
- Ferramenta para testes de API (ex.: Postman ou Swagger).

### Passos para Executar
1. **Clone o Repositório e Execute o Projeto**
```bash
   git clone https://github.com/sousa-sara/odontofast-dotnet-api
   
   cd OdontofastAPI

   dotnet watch run
```

2. **A documentação Swagger irá abrir em:**
```bash
http://localhost:5058/swagger/index.html

```

## Exemplos de Teste dos Endpoints

Abaixo estão exemplos de requisições para os endpoints da API, incluindo métodos HTTP, URLs, corpos de requisição (quando aplicável) e respostas esperadas. Esses exemplos podem ser testados usando o Swagger.

### 1. Endpoint de Login (`POST /api/login`)

#### Descrição
Realiza a autenticação de um usuário com base no número da carteira e senha.

#### Requisição
- **Método**: `POST`
- **URL**: `http://localhost:5058/api/login`
- **Headers**:
  - `Content-Type: application/json`
- **Body**:
```json
{
  "nrCarteira": "CARTEIRA5678",
  "senha": "senha@456"
}
```

#### Resposta Esperada
- **Status**: 200 OK
- **Body**:
```json
{
  "idUsuario": 2,
  "nomeUsuario": "Carlos Silva",
  "emailUsuario": "carlos.silva@example.com",
  "nrCarteira": "CARTEIRA5678",
  "telefoneUsuario": 11988888888
}
```

#### Resposta de Erro (Credenciais Inválidas e Erro Interno)
- **Status**: Error: 500 Internal Server Error
- **Body**:
```json
{
  "message": "Erro interno no servidor.",
  "error": "Credenciais inválidas."
}
```

### 2. Obter Usuário por ID (`GET /api/usuario/{id}`)

#### Descrição
Retorna os dados de um usuário específico com base no ID fornecido.

#### Requisição
- **Método**: `GET`
- **URL**: `http://localhost:5058/api/usuario/2`

#### Resposta Esperada
- **Status**: 200 OK
- **Body**:
```json
{
  "idUsuario": 2,
  "nomeUsuario": "Carlos Silva",
  "senhaUsuario": null,
  "emailUsuario": "carlos.silva@example.com",
  "nrCarteira": "CARTEIRA5678",
  "telefoneUsuario": 11988888888
}
```

#### Resposta de Erro (Usuário Não Encontrado)
- **Status**: 404 Not Found
- **Body**:
```json
{
  "message": "Usuário não encontrado."
}
```

### 3. Atualizar Usuário (`PUT /api/usuario/{id}`)

#### Descrição
Atualiza os dados de um usuário existente com base no ID.

#### Requisição
- **Método**: `PUT`
- **URL**: `http://localhost:5058/api/usuario/2`
- **Headers**:
  - `Content-Type: application/json`
- **Body**:
```json
{
  "idUsuario": 2,
  "nomeUsuario": "Carlos Almeida Silva",
  "senhaUsuario": "carlosSenha123",
  "emailUsuario": "carlossousa@gmail.com",
  "nrCarteira": "CARTEIRA8978",
  "telefoneUsuario": 11988888888
}
```

#### Resposta Esperada
- **Status**: 200 OK
- **Body**:
```json
{
  "idUsuario": 2,
  "nomeUsuario": "Carlos Almeida Silva",
  "senhaUsuario": "carlosSenha123",
  "emailUsuario": "carlossousa@gmail.com",
  "nrCarteira": "CARTEIRA8978",
  "telefoneUsuario": 11988888888
}
```

#### Resposta de Erro (Usuário Não Encontrado)
- **Status**: 404 Not Found
- **Body**:
```json
{
  "message": "Usuário não encontrado."
}
```
O endpoint utilizando o método DELETE não faz parte do escopo da nossa solução de integração da API à aplicação mobile. Em vez disso, iremos implementar a inativação dos usuários, mantendo os registros no banco de dados para fins de auditoria e rastreamento de histórico.

### 4. Registrar Progresso (`POST /api/progresso`)

#### Descrição
Registra o progresso de tratamento de um usuário e pode enviar email motivacional automaticamente.

#### Requisição
- **Método**: `POST`
- **URL**: `http://localhost:5058/api/progresso`
- **Headers**:
  - `Content-Type: application/json`
- **Body**:
```json
{
  "idUsuario": 1,
  "progresso": 75.5
}
```

#### Resposta Esperada
- **Status**: 200 OK
- **Body**:
```json
{
  "message": "Progresso recebido e processado."
}
```

#### Resposta de Erro (Erro Interno)
- **Status**: 500 Internal Server Error
- **Body**:
```json
{
  "message": "Erro interno no servidor.",
  "error": "Processing error"
}
```

### 5. Obter Imagem de Usuário (`GET /api/imagensusuario/{id}`)

#### Descrição
Retorna a imagem de perfil de um usuário específico com base no ID fornecido.

#### Requisição
- **Método**: `GET`
- **URL**: `http://localhost:5058/api/imagensusuario/1`

#### Resposta Esperada
- **Status**: 200 OK
- **Body**:
```json
{
  "idUsuario": 1,
  "caminhoImg": "/images/perfil/usuario1.jpg"
}
```

#### Resposta de Erro (Imagem Não Encontrada)
- **Status**: 404 Not Found
- **Body**:
```json
{
  "message": "Imagem de usuário não encontrada."
}
```

### 6. Criar Imagem de Usuário (`POST /api/imagensusuario`)

#### Descrição
Cria uma nova imagem de perfil para um usuário.

#### Requisição
- **Método**: `POST`
- **URL**: `http://localhost:5058/api/imagensusuario`
- **Headers**:
  - `Content-Type: application/json`
- **Body**:
```json
{
  "idUsuario": 1,
  "caminhoImg": "/images/perfil/usuario1.jpg"
}
```

#### Resposta Esperada
- **Status**: 201 Created
- **Body**:
```json
{
  "idUsuario": 1,
  "caminhoImg": "/images/perfil/usuario1.jpg"
}
```

#### Resposta de Erro (Usuário Não Encontrado)
- **Status**: 400 Bad Request
- **Body**:
```json
{
  "message": "Usuário com ID 1 não encontrado."
}
```

### 7. Atualizar Imagem de Usuário (`PUT /api/imagensusuario/{id}`)

#### Descrição
Atualiza a imagem de perfil de um usuário existente.

#### Requisição
- **Método**: `PUT`
- **URL**: `http://localhost:5058/api/imagensusuario/1`
- **Headers**:
  - `Content-Type: application/json`
- **Body**:
```json
{
  "caminhoImg": "/images/perfil/usuario1_novo.jpg"
}
```

#### Resposta Esperada
- **Status**: 200 OK
- **Body**:
```json
{
  "idUsuario": 1,
  "caminhoImg": "/images/perfil/usuario1_novo.jpg"
}
```

#### Resposta de Erro (Imagem Não Encontrada)
- **Status**: 404 Not Found
- **Body**:
```json
{
  "message": "Imagem de usuário não encontrada."
}
```

### 8. Excluir Imagem de Usuário (`DELETE /api/imagensusuario/{id}`)

#### Descrição
Remove a imagem de perfil de um usuário.

#### Requisição
- **Método**: `DELETE`
- **URL**: `http://localhost:5058/api/imagensusuario/1`

#### Resposta Esperada
- **Status**: 200 OK
- **Body**:
```json
{
  "message": "Imagem de usuário excluída com sucesso."
}
```

#### Resposta de Erro (Imagem Não Encontrada)
- **Status**: 404 Not Found
- **Body**:
```json
{
  "message": "Imagem de usuário não encontrada."
}
```

### 9. Verificar Existência de Imagem (`GET /api/imagensusuario/{id}/exists`)

#### Descrição
Verifica se um usuário possui imagem de perfil cadastrada.

#### Requisição
- **Método**: `GET`
- **URL**: `http://localhost:5058/api/imagensusuario/1/exists`

#### Resposta Esperada
- **Status**: 200 OK
- **Body**:
```json
{
  "idUsuario": 1,
  "possuiImagem": true
}
```

### 10. Predição de Duração de Tratamento (`POST /api/iaodontologica/prever-tratamento`)

#### Descrição
Prediz a duração estimada de um tratamento odontológico com base nas características do paciente.

#### Requisição
- **Método**: `POST`
- **URL**: `http://localhost:5058/api/iaodontologica/prever-tratamento`
- **Headers**:
  - `Content-Type: application/json`
- **Body**:
```json
{
  "idUsuario": 1,
  "tipoTratamento": "Ortodontia",
  "complexidadeTratamento": 4.5,
  "possuiComorbidades": false,
  "indiceSaude": 0.7
}
```

#### Resposta Esperada
- **Status**: 200 OK
- **Body**:
```json
{
  "duracaoEstimadaSemanas": 104,
  "mensagemEstimativa": "Tratamento de longa duração (aproximadamente 24 meses)",
  "recomendacoesIniciais": [
    "Manter boa higiene bucal com escovação após as refeições",
    "Evitar alimentos duros ou pegajosos que possam danificar o aparelho"
  ]
}
```

#### Resposta de Erro (Usuário Não Encontrado)
- **Status**: 400 Bad Request
- **Body**:
```json
{
  "mensagem": "Usuário com ID 1 não encontrado"
}
```

### 11. Gerar Recomendações Personalizadas (`POST /api/iaodontologica/recomendar`)

#### Descrição
Gera recomendações personalizadas de cuidados com base no progresso e tipo de tratamento.

#### Requisição
- **Método**: `POST`
- **URL**: `http://localhost:5058/api/iaodontologica/recomendar`
- **Headers**:
  - `Content-Type: application/json`
- **Body**:
```json
{
  "idUsuario": 1,
  "progressoAtual": 0.6,
  "tipoTratamento": "Ortodontia"
}
```

#### Resposta Esperada
- **Status**: 200 OK
- **Body**:
```json
{
  "recomendacoesCuidados": [
    "Use escova interdental para limpar ao redor do aparelho",
    "Mantenha os elásticos conforme orientação do ortodontista"
  ],
  "recomendacoesProximasEtapas": [
    "Mantenha o uso dos elásticos conforme orientação",
    "Programe os próximos ajustes com antecedência"
  ],
  "mensagemPersonalizada": "Olá, Carlos Silva! Você já alcançou 60% do seu tratamento de Ortodontia! Continue com o bom trabalho e mantenha a constância nos cuidados recomendados."
}
```

#### Resposta de Erro (Usuário Não Encontrado)
- **Status**: 400 Bad Request
- **Body**:
```json
{
  "mensagem": "Usuário com ID 1 não encontrado"
}
```

### 12. Treinar Modelo de Duração (`POST /api/iaodontologica/treinar-modelo-duracao`)

#### Descrição
Solicita o treinamento ou atualização do modelo de predição de duração de tratamentos.

#### Requisição
- **Método**: `POST`
- **URL**: `http://localhost:5058/api/iaodontologica/treinar-modelo-duracao`

#### Resposta Esperada
- **Status**: 200 OK
- **Body**:
```json
{
  "mensagem": "Modelo treinado com sucesso."
}
```

#### Resposta de Erro (Falha no Treinamento)
- **Status**: 500 Internal Server Error
- **Body**:
```json
{
  "mensagem": "Não foi possível treinar o modelo."
}
```

## Integrantes do Grupo

- Felipe Amador - RM553528
- Leonardo Oliveira - RM554024
- Sara Sousa - RM552656#   O d o n t o F a s t - d o t n e t - A P I 
 
 
