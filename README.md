# Developer Evaluation — Sales API

API de vendas feita em cima do template Ambev (.NET 8 + DDD).  
CRUD de sales, regras de desconto no domínio e eventos publicados no log (`SaleCreated`, `SaleModified`, `SaleCancelled`, `ItemCancelled`).

O código fica em `template/backend`.

## O que precisa

- .NET 8 SDK
- Docker (Postgres local e também pros testes de integração/funcionais com Testcontainers)
- IDE à escolha (VS / Rider / VS Code)

## Banco

Eu usei Postgres na porta **5434**. A connection string está no `appsettings.json` da WebApi:

```
Host=localhost;Port=5434;Database=DeveloperEvaluation;Username=postgres;Password=docker
```

Se a sua porta/usuário forem outros, só ajustar ali.

Migrations: tem a migration de Sales no projeto ORM. Na primeira subida, aplica com:

```bash
cd template/backend
dotnet ef database update --project src/Ambev.DeveloperEvaluation.ORM --startup-project src/Ambev.DeveloperEvaluation.WebApi
```

(Se preferir, sobe o Postgres do `docker-compose.yml` e aponta a connection string pro container.)

## Rodar a API

```bash
cd template/backend
dotnet run --project src/Ambev.DeveloperEvaluation.WebApi
```

Swagger sobe em Development. Endpoints principais:

| Método | Rota | O que faz |
|--------|------|-----------|
| POST | `/api/sales` | cria |
| GET | `/api/sales/{id}` | busca |
| GET | `/api/sales` | lista (paginação/filtro) |
| PUT | `/api/sales/{id}` | atualiza |
| DELETE | `/api/sales/{id}` | cancela a venda |
| DELETE | `/api/sales/{id}/item/{itemId}` | cancela um item |

## Testes

Docker precisa estar rodando pros Integration e Functional (Testcontainers sobe o Postgres sozinho).

```bash
cd template/backend

dotnet test tests/Ambev.DeveloperEvaluation.Unit
dotnet test tests/Ambev.DeveloperEvaluation.Integration
dotnet test tests/Ambev.DeveloperEvaluation.Functional
```

Ou tudo de uma vez na solution:

```bash
dotnet test Ambev.DeveloperEvaluation.sln
```

## Regras de desconto (resumo)

- < 4 itens iguais: sem desconto  
- 4 a 9: 10%  
- 10 a 20: 20%  
- acima de 20: não permite  

## Docs do template

Ainda valem os arquivos em [`.doc/`](.doc/) (overview, stack, etc.) se quiser o contexto original do desafio.
