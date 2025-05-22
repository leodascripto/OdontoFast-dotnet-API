
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

- **Simplicidade**
- **Integração com Mobile**
- **Recursos Limitados**
- **Escalabilidade Inicial**

**Justificativa contra Microservices**: Uma abordagem de microservices traria complexidades adicionais, como orquestração (ex.: Kubernetes), comunicação entre serviços (ex.: gRPC ou RabbitMQ) e maior overhead operacional.

---

## Implementação da API Seguindo a Arquitetura Escolhida

A API foi implementada como uma aplicação monolítica em .NET, utilizando o framework ASP.NET Core. As principais características da implementação incluem:

- **Estrutura Monolítica**
- **Separação de Camadas**
- **Diferenças em Relação a Microservices**

---

## Design Patterns Utilizados

- **Repository Pattern**
- **Dependency Injection (DI)**
- **Singleton**
- **DTO (Data Transfer Object)**

---

## Instruções para Rodar a API

### Pré-requisitos

- **.NET 8 SDK**
- Banco de dados **Oracle**
- Ferramenta para testes de API (ex.: Postman ou Swagger)

### Passos para Executar

```bash
git clone https://github.com/sousa-sara/odontofast-dotnet-api
cd OdontofastAPI
dotnet watch run
```

A documentação Swagger estará disponível em:
```
http://localhost:5058/swagger/index.html
```

---

## Exemplos de Teste dos Endpoints

### 1. Login (`POST /api/login`)
...

(O conteúdo completo segue conforme fornecido, mantendo a formatação original)
