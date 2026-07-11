
# FIAP Cloud Gaming - Fase 02

Tech Challenge Pos-graduacao FIAP.

# FCG — Users API

Microsserviço de Usuários da FIAP Cloud Games. Responsável por cadastro,
autenticação (JWT) e autorização de usuários.

Parte da Fase 2 do Tech Challenge — arquitetura de microsserviços orientada
a eventos. Este serviço é a evolução do monolito da Fase 1, mantendo apenas
o contexto de Identidade.

---

## Responsabilidades

- Cadastro de usuários com validação de e-mail e senha forte
- Autenticação via JWT
- Dois níveis de acesso: `User` e `Admin`
- Primeiro usuário cadastrado vira Admin automaticamente
- Publica `UserCreatedEvent` após cadastro bem-sucedido

---

## Tecnologias

- .NET 9
- Entity Framework Core + SQLite
- MassTransit + RabbitMQ (publica eventos)
- JWT Bearer Authentication
- Serilog (logs em arquivo)
- Swagger / OpenAPI

---

## Endpoints

| Método | Endpoint | Descrição | Acesso |
|--------|----------|-----------|--------|
| POST | `/usuarios` | Cadastrar usuário | Público |
| POST | `/auth/login` | Autenticar e obter JWT | Público |
| GET | `/usuarios` | Listar todos os usuários | Admin |
| GET | `/usuarios/{id}` | Buscar usuário por Id | Autenticado |
| PUT | `/usuarios/{id}` | Atualizar perfil | Autenticado |

---

## Eventos publicados

### `UserCreatedEvent`

Publicado após o cadastro de um novo usuário. Consumido pelo
`NotificationsAPI` para simular o envio de e-mail de boas-vindas.

```csharp
public record UserCreatedEvent(
    Guid     UsuarioId,
    string   Nome,
    string   Email,
    string   Role,
    DateTime CriadoEm);
```

---

## Variáveis de ambiente

| Variável | Descrição | Exemplo |
|----------|-----------|---------|
| `ConnectionStrings__DefaultConnection` | Connection string do SQLite | `Data Source=/data/users.db` |
| `Jwt__Secret` | Chave secreta para assinatura do JWT (mín. 32 chars) | `fcg-super-secret-key-min-32-chars!!` |
| `Jwt__Issuer` | Emissor do token | `FCG.UsersAPI` |
| `Jwt__Audience` | Audiência do token | `FCG.Client` |
| `Jwt__ExpirationMinutes` | Tempo de expiração do token em minutos | `60` |
| `RabbitMQ__Host` | Host do RabbitMQ | `rabbitmq` (Docker) ou `localhost` |
| `RabbitMQ__Usuario` | Usuário do RabbitMQ | `guest` |
| `RabbitMQ__Senha` | Senha do RabbitMQ | `guest` |

---

## Como rodar localmente

**Pré-requisitos:**
- .NET 9 SDK
- RabbitMQ rodando (local ou via Docker)

**Passos:**
```bash
dotnet restore
dotnet run --project FCG.UsersAPI
```

O banco `users.db` é criado automaticamente na primeira execução via
`EnsureCreated()` — não é necessário rodar migrations manualmente.

Acesse o Swagger em `https://localhost:{porta}/swagger`.

---

## Como rodar com Docker

```bash
docker build -t fcg-users-api .
docker run -p 5001:8080 \
  -e RabbitMQ__Host=host.docker.internal \
  fcg-users-api
```

> Recomendado: use o `docker-compose.yml` do repositório de orquestração
> (`fcg-orchestration`) para subir todos os serviços juntos com o RabbitMQ.

---

## Regras de negócio

- E-mail deve ser único e ter formato válido
- Senha: mínimo 8 caracteres, letra maiúscula, minúscula, número e especial
- Primeiro usuário cadastrado recebe a role `Admin` automaticamente
- Usuário só edita o próprio perfil — Admin pode editar qualquer um

---

*FIAP Cloud Games · Tech Challenge Fase 2 · PosTech FIAP*