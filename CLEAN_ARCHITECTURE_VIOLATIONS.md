# Relat?rio de Viola??es da Clean Architecture

## ? Viola??es Cr?ticas Encontradas

### 1. **WebApi Referenciando Domain Diretamente**
**Localiza??o:** `src/WebApi/WebApi.csproj` (linha 25)

**Problema:**
```xml
<ProjectReference Include="..\Domain\Domain.csproj" />
```

**Impacto:**
- Viola a regra fundamental: camadas externas n?o devem conhecer camadas internas
- WebApi (camada mais externa) conhece Domain (camada mais interna)
- Cria acoplamento forte entre apresenta??o e dom?nio

**Solu??o:**
- Remover refer?ncia ao Domain do WebApi.csproj
- WebApi deve depender apenas de Application
- Application ? respons?vel por orquestrar Domain

---

### 2. **Controllers Injetando Reposit?rios Diretamente**
**Localiza??o:** Todos os controllers em `src/WebApi/Controllers/`

**Exemplos:**
- `AuthController` injeta `IUserRepository`, `IRoleRepository`, `IUnitOfWork`
- `UserController` injeta m?ltiplos reposit?rios
- `GroupController` injeta reposit?rios

**Problema:**
```csharp
public class AuthController(
    IUserRepository userRepository,
    IRoleRepository roleRepository,
    IUnitOfWork unitOfWork,
    // ...
)
```

**Impacto:**
- Controllers conhecem detalhes de persist?ncia (viola Dependency Rule)
- L?gica de aplica??o fica nos controllers (deveria estar na Application)
- Imposs?vel testar controllers sem mocks de reposit?rios
- Viola??o do Single Responsibility Principle

**Solu??o:**
- Controllers devem injetar apenas servi?os/interfaces da Application
- Criar Use Cases na camada Application usando MediatR
- Controllers ficam apenas com responsabilidade de HTTP

---

### 3. **Controllers Usando Entidades de Domain**
**Localiza??o:** Todos os controllers

**Exemplo:** `src/WebApi/Controllers/v1/AuthController.cs`
```csharp
using LetsTripTogether.InternalApi.Domain.Aggregates.UserAggregate.Entities;
using LetsTripTogether.InternalApi.Domain.Aggregates.UserAggregate;
// ...
var user = await userRepository.GetByIdAsync(...);
```

**Problema:**
- Controllers trabalham diretamente com entidades de Domain
- Exp?e estrutura interna do dom?nio
- Dificulta evolu??o do modelo de dom?nio

**Impacto:**
- Viola camadas: WebApi n?o deve conhecer Domain
- Mudan?as no Domain quebram WebApi
- Dificulta manuten??o

**Solu??o:**
- Controllers devem trabalhar apenas com DTOs
- Application faz convers?o Domain <-> DTO
- Domain permanece isolado

---

### 4. **L?gica de Neg?cio nos Controllers**
**Localiza??o:** Todos os controllers

**Exemplo:** `src/WebApi/Controllers/v1/UserController.cs` (linhas 38-70)
```csharp
var user = await userRepository.GetByIdWithPreferencesAsync(...);
if (user is null) return NotFound(...);
try {
    _ = new UserPreference(user.Preferences);
    return Ok(new GetCurrentUserResponse { ... });
} catch (InvalidOperationException ex) when (...) {
    return UnprocessableEntity(...);
}
```

**Problema:**
- Valida??es e transforma??es est?o nos controllers
- L?gica de neg?cio misturada com responsabilidades HTTP
- Dificulta reutiliza??o da l?gica

**Impacto:**
- Viola Separation of Concerns
- L?gica n?o pode ser reutilizada
- Testes ficam mais complexos

**Solu??o:**
- Mover toda l?gica para Application (Use Cases)
- Controllers apenas: receber request, chamar use case, retornar response

---

### 5. **Mapeamento Domain -> DTO nos Controllers**
**Localiza??o:** Todos os controllers

**Exemplo:** `src/WebApi/Controllers/v1/UserController.cs` (linhas 49-64)
```csharp
return Ok(new GetCurrentUserResponse
{
    Name = user.Name,
    Email = user.Email,
    Preferences = user.Preferences is not null ? 
        new GetCurrentUserPreferenceResponse { ... } : null,
    // ...
});
```

**Problema:**
- Controllers fazem mapeamento manual
- Conhece estrutura de ambas Domain e DTOs
- Duplica??o de c?digo de mapeamento

**Impacto:**
- Viola responsabilidades: mapeamento deveria estar na Application
- Dificulta manuten??o quando modelos mudam

**Solu??o:**
- Usar AutoMapper na Application
- Application retorna DTOs prontos
- Controllers apenas retornam o DTO

---

### 6. **Valida??es nos Controllers**
**Localiza??o:** V?rios controllers

**Exemplo:** `src/WebApi/Controllers/v1/AuthController.cs` (linha 47)
```csharp
if (existsUserWithEmail)
{
    return Conflict(new ErrorResponse("There is already an user using this email."));
}
```

**Problema:**
- Valida??es de neg?cio nos controllers
- Deveriam estar na Application (use cases)
- FluentValidation j? est? configurado mas n?o est? sendo usado efetivamente

**Impacto:**
- L?gica de neg?cio espalhada
- Dificulta testes
- Viola??o do Single Responsibility

**Solu??o:**
- Usar FluentValidation na Application (j? configurado)
- Use cases fazem valida??es de neg?cio
- Controllers apenas chamam use cases

---

### 7. **Infrastructure Conhecendo Domain**
**Localiza??o:** `src/Infrastructure/EntityFramework/Context/AppDbContext.cs`

**Status:** ? **CORRETO**
- Infrastructure pode conhecer Domain (? permitido)
- Implementa interfaces definidas no Domain
- Segue Dependency Inversion Principle

---

### 8. **Application Conhecendo Domain**
**Localiza??o:** `src/Application/Application.csproj`

**Status:** ? **CORRETO**
- Application pode conhecer Domain (? permitido)
- Application orquestra Domain
- Segue regras da Clean Architecture

---

## ?? Resumo das Viola??es

| Viola??o | Severidade | Localiza??o | Status |
|----------|------------|-------------|--------|
| WebApi ? Domain | ?? Cr?tica | WebApi.csproj | ? |
| Controllers ? Reposit?rios | ?? Cr?tica | Todos controllers | ? |
| Controllers ? Entidades Domain | ?? Cr?tica | Todos controllers | ? |
| L?gica nos Controllers | ?? Cr?tica | Todos controllers | ? |
| Mapeamento nos Controllers | ?? Alta | Todos controllers | ? |
| Valida??es nos Controllers | ?? Alta | V?rios controllers | ? |

## ? Pontos Corretos

1. **Domain isolado** - Domain n?o conhece nenhuma outra camada ?
2. **Application conhece Domain** - Correto, Application orquestra Domain ?
3. **Infrastructure conhece Domain** - Correto, implementa interfaces ?
4. **Interfaces no Domain** - Reposit?rios e UnitOfWork definidos no Domain ?
5. **Dependency Inversion** - Infrastructure implementa interfaces do Domain ?

## ?? Solu??o Recomendada

### Estrutura Correta:

```
WebApi (Camada Externa)
  ? depende apenas de
Application (Camada de Aplica??o)
  ? depende de
Domain (Camada de Dom?nio)
  ? implementado por
Infrastructure (Camada Externa)
```

### Fluxo Correto:

1. **Controller** recebe HTTP Request (DTO)
2. **Controller** chama **Use Case** da Application (via MediatR)
3. **Use Case** usa **Reposit?rios** (interfaces do Domain)
4. **Use Case** orquestra **Entidades** do Domain
5. **Use Case** retorna **DTO** mapeado
6. **Controller** retorna HTTP Response

### Implementa??o:

```csharp
// Controller (WebApi)
[HttpPost]
public async Task<IActionResult> CreateUser([FromBody] CreateUserRequest request)
{
    var command = new CreateUserCommand { ... };
    var result = await _mediator.Send(command);
    return Ok(result);
}

// Use Case (Application)
public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, CreateUserResponse>
{
    private readonly IUserRepository _userRepository;
    
    public async Task<CreateUserResponse> Handle(CreateUserCommand request, CancellationToken ct)
    {
        // Toda l?gica aqui
        var user = new User(...);
        await _userRepository.AddAsync(user, ct);
        return _mapper.Map<CreateUserResponse>(user);
    }
}
```

## ?? Checklist de Corre??o

- [ ] Remover `ProjectReference` de Domain do WebApi.csproj
- [ ] Criar Use Cases na Application para cada opera??o
- [ ] Remover inje??o de reposit?rios dos controllers
- [ ] Controllers injetam apenas MediatR
- [ ] Mover toda l?gica de neg?cio para Use Cases
- [ ] Usar AutoMapper na Application para Domain <-> DTO
- [ ] Validadores FluentValidation na Application
- [ ] Controllers ficam apenas com responsabilidade HTTP
