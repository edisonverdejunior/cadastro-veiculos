# Cadastro de Veículos API

Uma API REST desenvolvida em .NET 8 para cadastro e consulta de veículos, com autenticação JWT e gerenciamento de usuários.

## Características

- .NET 8
- ASP.NET Core Web API com Controllers
- Autenticação e Autorização com JWT
- Entity Framework Core InMemory
- MediatR para CQRS (Commands e Queries)
- FluentValidation para validações
- OpenAPI/Swagger
- BCrypt para hash de senha
- Arquitetura em camadas (Domain, Application, Infra, WebApi)

## Como Executar a Solução
### Pré-requisitos

- .NET 8 SDK instalado
- Visual Studio 2022 ou VS Code (opcional)

### Passos

1. Clone o repositório:
```bash
git clone https://github.com/edisonverdejunior/cadastro-veiculos.git
cd cadastro-veiculos
```

2. Navegue até a pasta do projeto API:
```bash
cd src/CadastroVeiculos.API
```

3. Restaure as dependências:
```bash
dotnet restore
```

4. Execute a aplicação:
```bash
dotnet run
```

5. A API estará disponível em `https://localhost:5001` (ou a porta exibida no console)

---

## Autenticação - Cadastro e Login

### Usuários Pré-cadastrados

A aplicação já vem com dois usuários de exemplo:

| Login    | Senha      | Descrição          |
|----------|------------|-------------------|
| admin    | Admin@123  | Usuário administrador |
| usuario  | User@123   | Usuário padrão    |

### 1. Cadastrar Novo Usuário

**Endpoint:** `POST /api/usuarios`

**Descrição:** Cria um novo usuário na aplicação.

**Exemplo de Requisição:**
```json
{
  "nome": "João Silva",
  "login": "joao.silva",
  "senha": "Senha@123"
}
```

**Resposta (201 Created):**
```json
{
  "id": "550e8400-e29b-41d4-a716-446655440000",
  "nome": "João Silva",
  "login": "joao.silva"
}
```

### 2. Fazer Login

**Endpoint:** `POST /api/auth/login`

**Descrição:** Realiza autenticação e retorna um JWT token.

**Exemplo de Requisição:**
```json
{
  "login": "joao.silva",
  "senha": "Senha@123"
}
```

**Resposta (200 OK):**
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."
}
```

---

## Como Usar o Token no Swagger

1. Acesse o Swagger em: `https://localhost:5001/swagger/index.html`

2. Clique no botão **"Authorize"** (cadeado) no topo da página

3. Na janela de diálogo, selecione **"Bearer"** se não estiver já selecionado

4. Cole o token JWT (apenas o valor, sem a palavra "Bearer"):
   ```
   eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
   ```

5. Clique em **"Authorize"** para confirmar

6. Agora você pode executar requisições protegidas no Swagger

---

## Exemplos de JSON

### Usuários

#### Criar Usuário (POST /api/usuarios)
```json
{
  "nome": "Maria Santos",
  "login": "maria.santos",
  "senha": "MariaSenha@2024"
}
```

#### Atualizar Usuário (PUT /api/usuarios/{id})
```json
{
  "nome": "Maria Santos Silva",
  "login": "maria.santos.silva",
  "senha": "NovaS@nha@2024"
}
```

### Veículos

#### Criar Veículo (POST /api/veiculos)
```json
{
  "descricao": "Sedan executivo com teto solar",
  "marca": 1,
  "modelo": "Corolla",
  "opcionais": "Teto solar, Ar-condicionado automático, Freios ABS",
  "valor": 135000.00
}
```

## Tabela de Marcas (Enum)

| Código | Marca      |
|--------|-----------|
| 1      | Toyota    |
| 2      | Honda     |
| 3      | Hyundai   |
| 4      | Volkswagen|
| 5      | Chevrolet |
| 6      | Ford      |
| 7      | BMW       |
| 8      | Mercedes  |
| 9      | Audi      |
| 10     | Fiat      |
| 11     | Renault   |
| 12     | Peugeot   |
| 13     | Nissan    |
| 14     | Kia       |
| 15     | Jeep      |


#### Atualizar Veículo (PUT /api/veiculos/{id})
```json
{
  "descricao": "Sedan executivo com teto solar panorâmico",
  "marca": 1,
  "modelo": "Corolla 2024",
  "opcionais": "Teto solar panorâmico, Ar-condicionado automático, Freios ABS, Câmera traseira",
  "valor": 142000.00
}
```

#### Obter Veículo (GET /api/veiculos/{id})
Retorna um veículo específico por ID.

#### Listar Veículos (GET /api/veiculos)
Retorna a lista de todos os veículos cadastrados.

---


## Notas Importantes

- Todos os endpoints de veículos requerem autenticação (JWT token)
- O cadastro de usuários não requer autenticação
- Os tokens expiram conforme configurado na aplicação
- Use HTTPS em produção