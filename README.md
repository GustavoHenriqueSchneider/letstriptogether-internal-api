# lets-trip-together-internal-api

para subir o docker compose
``` powershell
docker compose up -d
```

para criar migration do ef
``` powershell
dotnet ef migrations add <NomeMigration>
```

para atualizar migrations do ef no container
``` powershell
dotnet ef database update
```