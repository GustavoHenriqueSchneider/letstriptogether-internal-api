# LetsTripTogether - Internal API

## 📋 Sobre o Projeto

**LetsTripTogether** é uma API interna desenvolvida para facilitar o planejamento colaborativo de viagens em grupo. O sistema permite que usuários criem grupos de viagem, convidem outros usuários, votem em destinos e recebam sugestões de matches baseados nas preferências coletivas do grupo.

### Objetivo

O objetivo principal desta API é fornecer uma plataforma robusta e escalável para gerenciar:
- **Grupos de Viagem**: Criação e gerenciamento de grupos para planejar viagens colaborativas
- **Sistema de Votação**: Mecanismo para membros votarem em destinos de interesse
- **Matching Inteligente**: Algoritmo que identifica destinos compatíveis com as preferências do grupo
- **Convites e Membros**: Sistema completo de convites e gerenciamento de membros
- **Preferências de Viagem**: Sistema flexível de preferências (cultura, entretenimento, comida, tipos de lugares)

## 🏗️ Arquitetura

Este projeto segue os princípios da **Clean Architecture** (Arquitetura Limpa) e **Domain-Driven Design (DDD)**, organizando o código em camadas bem definidas com responsabilidades claras:

```
┌─────────────────────────────────────────┐
│           WebApi (Presentation)         │  ← Controllers, Middleware, Configuração HTTP
├─────────────────────────────────────────┤
│         Application (Use Cases)         │  ← Handlers, Validators, DTOs, Behaviours
├─────────────────────────────────────────┤
│            Domain (Core)                │  ← Entidades, Agregados, Value Objects, Regras de Negócio
├─────────────────────────────────────────┤
│        Infrastructure (External)        │  ← EF Core, Redis, SMTP, Repositories, Services
└─────────────────────────────────────────┘
```

### Camadas

#### 1. **Domain** (Camada de Domínio)
- **Responsabilidade**: Contém a lógica de negócio pura, independente de frameworks
- **Contém**:
  - Agregados (User, Group, Destination, Role)
  - Entidades de domínio
  - Value Objects (TripPreference, Step)
  - Interfaces de repositórios
  - Exceções de domínio
  - Constantes de segurança (Claims, Roles, TokenTypes, NotificationEvents)
- **Características**: Zero dependências externas, regras de negócio encapsuladas

#### 2. **Application** (Camada de Aplicação)
- **Responsabilidade**: Orquestra os casos de uso e coordena o domínio
- **Contém**:
  - Handlers (MediatR) para cada caso de uso
  - Validators (FluentValidation)
  - DTOs (Commands, Queries, Responses)
  - Behaviours (Validation, Exception Handling)
  - Interfaces de serviços
  - Extensions para HttpContext e UserContext
- **Padrões**: CQRS (Command Query Responsibility Segregation) com MediatR

#### 3. **Infrastructure** (Camada de Infraestrutura)
- **Responsabilidade**: Implementa detalhes técnicos e integrações externas
- **Contém**:
  - Entity Framework Core (ORM)
  - Repositórios (implementações concretas)
  - Serviços (Email, Token, Redis, Notification, Password Hash)
  - Configurações (Email, JWT, Notification)
  - Clientes (SMTP, Redis)
  - Migrations do banco de dados
- **Tecnologias**: PostgreSQL, Redis, SMTP

#### 4. **WebApi** (Camada de Apresentação)
- **Responsabilidade**: Expõe a API REST e gerencia requisições HTTP
- **Contém**:
  - Controllers (v1, Error, Health)
  - Startup/Program configuration
  - Middleware pipeline
  - Swagger/OpenAPI
  - Health checks
- **Características**: Versionamento de API, documentação automática

## 🛠️ Tecnologias Utilizadas

### Backend
- **.NET 8.0** - Framework principal
- **C#** - Linguagem de programação
- **ASP.NET Core** - Framework web

### Arquitetura e Padrões
- **MediatR** (v12.4.1) - Implementação do padrão Mediator para CQRS
- **FluentValidation** (v11.11.0) - Validação de dados
- **AutoMapper** (v12.0.1) - Mapeamento de objetos

### Banco de Dados
- **PostgreSQL 16** - Banco de dados relacional principal
- **Entity Framework Core** (v9.0.9) - ORM
- **Npgsql.EntityFrameworkCore.PostgreSQL** (v9.0.4) - Provider PostgreSQL

### Cache e Armazenamento
- **Redis** (v7.2.0) - Cache distribuído e armazenamento de sessões/tokens
- **StackExchange.Redis** (v2.9.25) - Cliente Redis para .NET

### Autenticação e Segurança
- **JWT (JSON Web Tokens)** - Autenticação stateless
- **BCrypt.Net-Next** (v4.0.3) - Hash de senhas
- **Microsoft.AspNetCore.Authentication.JwtBearer** (v8.0.20)

### Comunicação
- **SMTP** - Envio de emails (confirmação, reset de senha)
- **HTTP Client** - Comunicação com serviços externos (notificações)
- **Notification Service** - Serviço de notificações para eventos do sistema (matches, convites, etc.)

### Documentação e Testes
- **Swashbuckle.AspNetCore** (v9.0.6) - Swagger/OpenAPI
- **NUnit** - Framework de testes
- **Moq** - Mocking para testes unitários
- **FluentAssertions** - Assertions expressivas em testes

### DevOps
- **Docker** - Containerização
- **Docker Compose** - Orquestração de containers

## 🎯 Conceitos Principais

### Domain-Driven Design (DDD)

#### Agregados
- **User**: Representa um usuário do sistema com suas preferências (UserPreference), roles (UserRole) e convites (UserGroupInvitation)
- **Group**: Agregado raiz que gerencia grupos de viagem, membros (GroupMember), convites (GroupInvitation), matches (GroupMatch), preferências (GroupPreference) e votos (GroupMemberDestinationVote)
- **Destination**: Representa destinos turísticos com suas atrações (DestinationAttraction)
- **Role**: Define papéis e permissões no sistema

#### Value Objects
- **TripPreference**: Preferências de viagem categorizadas em:
  - Cultura (TripCulturePreferences)
  - Entretenimento (TripEntertainmentPreferences)
  - Tipos de Lugares (TripPlaceTypes)
  - Shopping e Gastronomia (categorias diretas)
- **Step**: Representa etapas do processo de registro (validate-email, set-password)

#### Repositórios
Interfaces definidas no domínio, implementadas na infraestrutura:
- `IUserRepository`, `IGroupRepository`, `IDestinationRepository`, `IRoleRepository`
- `IGroupMemberRepository`, `IGroupInvitationRepository`, `IGroupMatchRepository`
- `IGroupMemberDestinationVoteRepository`, `IGroupPreferenceRepository`
- `IUserPreferenceRepository`, `IUserRoleRepository`, `IUserGroupInvitationRepository`

### CQRS (Command Query Responsibility Segregation)

O projeto utiliza **MediatR** para separar comandos (mudanças de estado) de queries (consultas):

- **Commands**: Operações que modificam estado (CreateGroup, VoteAtDestination, etc.)
- **Queries**: Operações de leitura (GetGroupById, GetAllGroups, etc.)

Cada caso de uso possui:
- `Handler`: Lógica de processamento
- `Validator`: Validação de entrada (FluentValidation)
- `Command/Query`: DTO de entrada
- `Response`: DTO de saída

### Clean Architecture

- **Independência de Frameworks**: O domínio não depende de nenhum framework
- **Testabilidade**: Cada camada pode ser testada independentemente
- **Inversão de Dependências**: Interfaces no domínio, implementações na infraestrutura

### Padrões Implementados

1. **Repository Pattern**: Abstração de acesso a dados
2. **Unit of Work**: Gerenciamento transacional
3. **Mediator Pattern**: Desacoplamento via MediatR
4. **Strategy Pattern**: Diferentes estratégias de validação e comportamento
5. **Factory Pattern**: Criação de entidades complexas

## 🚀 Como Executar

### Pré-requisitos

- **.NET SDK 8.0** ou superior
- **Docker** e **Docker Compose**
- **PostgreSQL 16** (via Docker)
- **Redis 7.2** (via Docker)

### Configuração Inicial

1. **Clone o repositório**
```bash
git clone <repository-url>
cd letstriptogether-internal-api
```

2. **Subir os containers do Docker Compose**

**Para subir apenas PostgreSQL e Redis (útil para rodar a API na IDE):**
```powershell
docker compose up -d
```

**Para subir PostgreSQL, Redis e a API:**
```powershell
docker compose --profile api up -d
```

Isso irá iniciar:
- PostgreSQL na porta `5432`
- Redis na porta `6379`
- Internal API na porta `5088` (apenas se usar `--profile api`)

3. **Configurar variáveis de ambiente**

⚠️ **IMPORTANTE**: O arquivo `appsettings.Development.json` contém secrets e não deve ser commitado no repositório.

**Para desenvolvimento local:**
1. Copie o arquivo de exemplo:
   ```powershell
   cp src/WebApi/appsettings.Development.example.json src/WebApi/appsettings.Development.json
   ```
2. Edite `src/WebApi/appsettings.Development.json` e substitua os placeholders pelos valores reais:
   - `{POSTGRES_USER}` e `{POSTGRES_PASSWORD}`: Credenciais do PostgreSQL
   - `{REDIS_PASSWORD}`: Senha do Redis
   - `{JWT_SECRET_KEY}`: Chave secreta para geração de tokens JWT (deve ser uma string segura)
   - `{SMTP_SERVER}`, `{SMTP_USERNAME}`, `{SMTP_PASSWORD}`: Credenciais do servidor SMTP

**Para Docker:**
Passe as variáveis diretamente no comando `docker run`:
```bash
docker run -p 5088:5088 \
  -e ConnectionStrings__Postgres="Host=postgres;Port=5432;Database=lets-trip-together;
  Username=your_postgres_user;Password=your_postgres_password" \
  -e ConnectionStrings__Redis="redis:6379,password=your_redis_password" \
  -e JsonWebTokenSettings__SecretKey="your_jwt_secret_key" \
  -e EmailSettings__SmtpServer="your_smtp_server" \
  -e EmailSettings__Username="your_smtp_username" \
  -e EmailSettings__Password="your_smtp_password" \
  letstriptogether-internal-api
```

Ou configure as variáveis de ambiente no `docker-compose.yml`.

4. **Aplicar migrations do banco de dados**
```powershell
dotnet ef database update --project .\src\Infrastructure --startup-project .\src\WebApi
```

5. **Executar a aplicação**
```bash
cd src/WebApi
dotnet run
```

A API estará disponível em:
- **HTTP**: `http://localhost:5088`
- **HTTPS**: `https://localhost:7069`
- **Swagger**: `https://localhost:7069/swagger`
- **Health Check**: `https://localhost:7069/api/health`

## 📝 Comandos Úteis

### Docker Compose

**Para subir apenas PostgreSQL e Redis (sem a API):**
```powershell
docker compose up -d
```

**Para subir PostgreSQL, Redis e a API:**
```powershell
docker compose --profile api up -d
```

**Para interromper os containers do docker compose e apagar volumes respectivos:**
```powershell
docker compose down -v
```

**Para interromper todos os containers incluindo a API:**
```powershell
docker compose --profile api down -v
```

### Entity Framework Migrations

**Para criar uma nova migration:**
```powershell
dotnet ef migrations add <NomeMigration> --project .\src\Infrastructure --startup-project .\src\WebApi
```

**Para atualizar o banco de dados com as migrations:**
```powershell
dotnet ef database update --project .\src\Infrastructure --startup-project .\src\WebApi
```

### Testes

**Para rodar todos os testes:**
```bash
dotnet test tests/Application.UnitTests/Application.UnitTests.csproj tests/Domain.UnitTests/Domain.UnitTests.csproj tests/Infrastructure.UnitTests/Infrastructure.UnitTests.csproj tests/WebApi.UnitTests/WebApi.UnitTests.csproj --verbosity normal
```

**Para rodar testes de um projeto específico:**
```bash
dotnet test tests/Application.UnitTests/Application.UnitTests.csproj --verbosity normal
```

**Para rodar testes de um projeto específico (exemplos):**
```bash
# Testes de Application
dotnet test tests/Application.UnitTests/Application.UnitTests.csproj --verbosity normal

# Testes de Domain
dotnet test tests/Domain.UnitTests/Domain.UnitTests.csproj --verbosity normal

# Testes de Infrastructure
dotnet test tests/Infrastructure.UnitTests/Infrastructure.UnitTests.csproj --verbosity normal

# Testes de WebApi
dotnet test tests/WebApi.UnitTests/WebApi.UnitTests.csproj --verbosity normal
```

## 🔐 Segurança

### Autenticação e Autorização

- **JWT Tokens**: Autenticação stateless com access e refresh tokens
- **BCrypt**: Hash de senhas com salt automático
- **Policies**: Políticas de autorização baseadas em claims
- **Roles**: Sistema de roles (User, Admin)

### Validação

- **FluentValidation**: Validação robusta em todas as camadas
- **Domain Validation**: Regras de negócio validadas no domínio
- **Input Validation**: Validação de entrada nos handlers

## 📊 Funcionalidades Principais

### Notificações
- Sistema de notificações para eventos importantes:
  - Criação de matches quando todos os membros aprovam um destino
- Integração com serviço externo de notificações via HTTP Client
- Notificações enviadas automaticamente aos usuários

### Autenticação
- Registro de usuário com confirmação por email
- Login com JWT
- Refresh token
- Reset de senha
- Alteração de senha (requer senha atual)
- Logout

### Gestão de Usuários
- Consultar informações do usuário atual
- Atualizar informações do usuário atual
- Alterar senha do usuário atual
- Definir preferências de viagem
- Excluir conta
- Anonimizar dados pessoais

### Gestão de Grupos
- Criar grupos de viagem
- Consultar grupos
- Adicionar/remover membros
- Gerenciar preferências do grupo
- Definir data esperada da viagem
- Consultar membros do grupo

### Sistema de Votação
- Votar em destinos (aprovar/rejeitar)
- Atualizar votos
- Consultar votos de membros
- Consultar destinos não votados

### Matching
- Matching automático quando todos aprovam um destino
- Notificações quando um match é criado
- Consulta de matches do grupo
- Remover matches do grupo

### Convites
- Criar convites para grupos
- Aceitar/recusar convites
- Cancelar convites ativos
- Consultar convites
- Consultar detalhes de convite por token (informações do grupo e criador)

### Destinos
- Consultar destinos disponíveis
- Consultar atrações de destinos
- Dados pré-carregados de cidades e atrações

### Administração
- CRUD completo de usuários, grupos, destinos
- Anonimização de usuários
- Consultas administrativas detalhadas de:
  - Usuários (listagem, detalhes, preferências, votos)
  - Grupos (listagem, detalhes, membros, convites, matches, votos)
  - Destinos (listagem, detalhes)
  - Votos de destinos por grupo
  - Membros de grupos
  - Convites de grupos
  - Matches de grupos

## 🧪 Testes

O projeto possui cobertura de testes em todas as camadas:

- **Domain.UnitTests**: Testes de entidades, value objects e regras de negócio
- **Application.UnitTests**: Testes de handlers, validators e comportamentos
- **Infrastructure.UnitTests**: Testes de repositórios e serviços
- **WebApi.UnitTests**: Testes de controllers e endpoints

### Estrutura de Testes

Cada teste segue o padrão **AAA** (Arrange-Act-Assert):
- **Arrange**: Configuração do cenário
- **Act**: Execução da ação
- **Assert**: Verificação do resultado

### Tecnologias de Teste

- **NUnit** (v4.2.2) - Framework de testes
- **Moq** (v4.20.72) - Mocking para testes unitários
- **FluentAssertions** (v6.12.1) - Assertions expressivas em testes
- **Microsoft.EntityFrameworkCore.InMemory** (v9.0.9) - Banco em memória para testes
- **Npgsql.EntityFrameworkCore.PostgreSQL** (v9.0.4) - Provider PostgreSQL para testes de integração

## 📚 Documentação da API

A documentação interativa da API está disponível via **Swagger/OpenAPI** quando a aplicação está em execução:

- Acesse: `https://localhost:7069/swagger`
- A API está versionada (v1)
- Todos os endpoints estão documentados com exemplos
- Endpoints de sistema (Error, Health) também estão disponíveis

## 🔄 Fluxo de Dados

1. **Request** → Controller recebe requisição HTTP
2. **Validation** → FluentValidation valida o input
3. **Handler** → MediatR despacha para o handler apropriado
4. **Domain** → Handler utiliza repositórios e entidades de domínio
5. **Infrastructure** → Repositórios executam queries no banco
6. **Response** → DTO de resposta é retornado ao cliente

## 🤝 Contribuindo

Este é um projeto interno. Para contribuições:

1. Siga os padrões de código estabelecidos
2. Mantenha a cobertura de testes
3. Documente mudanças significativas
4. Siga os princípios de Clean Architecture e DDD

## 📄 Licença

Este projeto é de uso interno.

---

**Desenvolvido com ❤️ usando .NET 8 e Clean Architecture**
