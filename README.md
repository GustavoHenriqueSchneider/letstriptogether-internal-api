# lets-trip-together-internal-api

para interromper os containers do dockercompose e apagar volumes respectivos
``` powershell
docker compose down -v
```

para subir os containers do dockercompose de forma livre do console
``` powershell
docker compose up -d
```

para criar migration do ef
``` powershell
dotnet ef migrations add <NomeMigration> --project .\src\Infrastructure\ --startup-project .\src\WebApi\
```

para atualizar migrations do ef no container
``` powershell
dotnet ef database update --project .\src\Infrastructure\ --startup-project .\src\WebApi\
```

para rodar todos testes
```
dotnet test tests/Application.UnitTests/Application.UnitTests.csproj tests/Infrastructure.UnitTests/Infrastructure.UnitTests.csproj tests/WebApi.UnitTests/WebApi.UnitTests.csproj --verbosity normal
```