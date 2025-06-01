# DigitalWallet 💳

## Sobre o Projeto

O **DigitalWallet** é uma API REST robusta para gerenciamento de carteiras digitais, desenvolvida em **.NET 9** seguindo os princípios de Clean Architecture e Domain-Driven Design (DDD). O sistema permite aos usuários criar e gerenciar múltiplas carteiras, realizar transações, transferências entre carteiras e acompanhar o histórico financeiro.

## 🏗️ Arquitetura

O projeto está estruturado seguindo os princípios de Clean Architecture, dividido em camadas bem definidas:

```
├── DigitalWallet.API/          # Camada de Apresentação (Controllers/Endpoints)
├── DigitalWallet.Application/  # Camada de Aplicação (Use Cases, DTOs, Interfaces)
├── DigitalWallet.Domain/       # Camada de Domínio (Entidades, Value Objects, Regras de Negócio)
├── DigitalWallet.Infrastructure/ # Camada de Infraestrutura (Repositórios, Context, Migrations)
├── DigitalWallet.CrossCutting/ # Recursos Transversais (Validações, Enums, Extensões)
└── Tests/                      # Testes Unitários
```

## 🚀 Funcionalidades

### 👤 Gestão de Usuários
- ✅ Criação de usuários com autenticação JWT
- ✅ Consulta de dados do usuário
- ✅ Sistema de autenticação e autorização

### 💰 Gestão de Carteiras
- ✅ Criação de múltiplas carteiras por usuário
- ✅ Consulta de saldo de carteiras
- ✅ Adição de crédito às carteiras
- ✅ Ativação/Desativação de carteiras
- ✅ Suporte a múltiplas moedas (BRL, USD, EUR)

### 🔄 Transações
- ✅ Transferências entre carteiras
- ✅ Histórico de transações por usuário
- ✅ Filtros por data nas consultas
- ✅ Controle de status das transações

### 🔐 Segurança
- ✅ Autenticação JWT
- ✅ Autorização baseada em roles
- ✅ Validação de dados com FluentValidation
- ✅ Auditoria de operações

## 🛠️ Tecnologias Utilizadas

- **.NET 9** - Framework principal
- **ASP.NET Core** - Web API
- **Entity Framework Core** - ORM
- **PostgreSQL** - Banco de dados
- **JWT Bearer** - Autenticação
- **ASP.NET Core Identity** - Gestão de usuários
- **Swagger/OpenAPI** - Documentação da API
- **FluentValidation** - Validação de dados
- **AutoFixture** - Testes unitários
- **xUnit** - Framework de testes
- **Docker** - Containerização

## 📋 Pré-requisitos

- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- [Docker](https://www.docker.com/get-started) e [Docker Compose](https://docs.docker.com/compose/)
- [PostgreSQL](https://www.postgresql.org/) (se executar sem Docker)

## 🐳 Executando com Docker

### Método Rápido (Recomendado)

1. **Clone o repositório:**
```bash
git clone https://github.com/marciocrmendes/DigitalWallet.git
cd DigitalWallet
```

2. **Execute com Docker Compose:**
```bash
docker-compose up -d
```

3. **Acesse a aplicação:**
- API: http://localhost:8080
- Swagger UI: http://localhost:8080/swagger

### Configuração Manual

1. **Configurar variáveis de ambiente:**
```bash
# Crie um arquivo .env na raiz do projeto
DATABASE_CONNECTION_STRING=Host=localhost:5432;Database=digitalwallet_dev;Username=postgres;Password=postgres;
JWT_SECRET=iVb3-He2kKefscneQ!Ybu-GTnqW2b!2q
JWT_ISSUER=http://localhost
JWT_AUDIENCE=http://localhost
```

2. **Executar o banco PostgreSQL:**
```bash
docker run -d \
  --name postgres_digitalwallet \
  -e POSTGRES_USER=postgres \
  -e POSTGRES_PASSWORD=postgres \
  -e POSTGRES_DB=digitalwallet_dev \
  -p 5432:5432 \
  postgres:15
```

3. **Executar as migrations:**
```bash
cd DigitalWallet.API
dotnet ef database update
```

4. **Executar a aplicação:**
```bash
dotnet run
```

## 🔧 Desenvolvimento Local

### 1. Configuração do Ambiente

```bash
# Instalar dependências
dotnet restore

# Configurar banco de dados (PostgreSQL deve estar rodando)
cd DigitalWallet.API
dotnet ef database update
```

### 2. Executar em modo de desenvolvimento

```bash
cd DigitalWallet.API
dotnet run --environment Development
```

### 3. Executar testes

```bash
# Executar todos os testes
dotnet test

# Executar testes com cobertura
dotnet test --collect:"XPlat Code Coverage"
```

## 📖 Documentação da API

### Autenticação

A API utiliza autenticação JWT. Para acessar os endpoints protegidos:

1. **Registre um usuário:**
```http
POST /api/v1/users
Content-Type: application/json

{
  "firstName": "João",
  "lastName": "Silva",
  "email": "joao@exemplo.com",
  "password": "MinhaSenh@123"
}
```

2. **Faça login:**
```http
POST /api/identity
Content-Type: application/json

{
  "email": "joao@exemplo.com",
  "password": "MinhaSenh@123"
}
```

3. **Use o token retornado:**
```http
Authorization: Bearer {seu_token_jwt}
```

### Principais Endpoints

#### 👤 Usuários
- `POST /api/v1/users` - Criar usuário
- `GET /api/v1/users/{id}` - Buscar usuário por ID
- `GET /api/v1/users/{id}/wallets` - Listar carteiras do usuário
- `GET /api/v1/users/{id}/transactions` - Histórico de transações

#### 💰 Carteiras
- `POST /api/v1/wallets` - Criar carteira
- `GET /api/v1/wallets/{id}/balance` - Consultar saldo
- `POST /api/v1/wallets/{id}/balance` - Adicionar saldo

#### 🔄 Transações
- `POST /api/v1/transaction/transfer` - Transferir entre carteiras

### Exemplo de Uso Completo

```bash
# 1. Criar usuário
curl -X POST "http://localhost:8080/digital-wallet/api/v1/users" \
  -H "Content-Type: application/json" \
  -d '{
    "firstName": "João",
    "lastName": "Silva", 
    "email": "joao@exemplo.com",
    "password": "MinhaSenh@123"
  }'

# 2. Fazer login
curl -X POST "http://localhost:8080/digital-wallet/api/identity" \
  -H "Content-Type: application/json" \
  -d '{
    "email": "joao@exemplo.com",
    "password": "MinhaSenh@123"
  }'

# 3. Criar carteira (usando o token recebido)
curl -X POST "http://localhost:8080/digital-wallet/api/v1/wallets" \
  -H "Authorization: Bearer {TOKEN}" \
  -H "Content-Type: application/json" \
  -d '{
    "userId": "{USER_ID}",
    "name": "Minha Carteira",
    "currency": "BRL",
    "description": "Carteira principal"
  }'

# 4. Adicionar saldo
curl -X POST "http://localhost:8080/digital-wallet/api/v1/wallets/{WALLET_ID}/balance" \
  -H "Authorization: Bearer {TOKEN}" \
  -H "Content-Type: application/json" \
  -d '{
    "amount": 1000.00,
    "description": "Depósito inicial"
  }'
```

## 🗄️ Estrutura do Banco de Dados

### Principais Entidades

- **Users** - Dados dos usuários
- **Wallets** - Carteiras digitais
- **Transactions** - Histórico de transações
- **Roles** - Perfis de usuário (Identity)

### Relacionamentos

- User → Wallets (1:N)
- Wallet → Transactions (1:N)

## 🧪 Testes

O projeto inclui testes unitários abrangentes:

```bash
# Executar todos os testes
dotnet test

# Executar testes específicos
dotnet test --filter "ClassName=WalletTests"

# Gerar relatório de cobertura
dotnet test --collect:"XPlat Code Coverage" --results-directory ./TestResults
```

## 📁 Estrutura de Pastas Detalhada

```
DigitalWallet/
├── DigitalWallet.API/
│   ├── Endpoints/              # Minimal APIs endpoints
│   ├── Extensions/             # Extensões e DI
│   ├── Configurations/         # Configurações da API
│   └── Program.cs             # Ponto de entrada
├── DigitalWallet.Application/
│   ├── UseCases/              # Casos de uso da aplicação
│   ├── DTOs/                  # Data Transfer Objects
│   ├── Interfaces/            # Contratos da aplicação
│   └── Extensions/            # Extensões da aplicação
├── DigitalWallet.Domain/
│   ├── Entities/              # Entidades do domínio
│   ├── ValueObjects/          # Objetos de valor
│   └── Interfaces/            # Contratos do domínio
├── DigitalWallet.Infrastructure/
│   ├── Context/               # DbContext
│   ├── Repositories/          # Implementação dos repositórios
│   ├── Migrations/            # Migrations do EF Core
│   └── Extensions/            # Configurações de infraestrutura
└── DigitalWallet.CrossCutting/
    ├── Enums/                 # Enumeradores
    ├── Validation/            # Validações customizadas
    └── Extensions/            # Extensões utilitárias
```

## 🔧 Configurações

### appsettings.json

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost:5432;Database=digitalwallet_dev;Username=postgres;Password=postgres;"
  },
  "JwtSettings": {
    "Issuer": "http://localhost",
    "Audience": "http://localhost", 
    "TokenExpirationInMinutes": 60,
    "TokenSecurityKey": "sua-chave-secreta-muito-forte"
  }
}
```
