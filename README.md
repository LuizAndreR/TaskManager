# TaskMangem

**TaskMangem** é uma API de gerenciamento de tarefas desenvolvida com ASP.NET 8, utilizando arquitetura **DDD (Domain-Driven Design)**. A aplicação possui **CRUD completo de tarefas**, autenticação **JWT**, alteração de status e prioridade por endpoints separados, e validação de dados usando **FluentValidation**.

---

## 🛠 Tecnologias Utilizadas

- **Linguagem:** C#
- **Framework:** ASP.NET Core 8
- **Banco de Dados:** PostgreSQL
- **ORM:** Entity Framework Core
- **Validação:** FluentValidation
- **Arquitetura:** Domain-Driven Design (DDD)
- **Testes:** xUnit (100% de cobertura de testes unitários)
- **Autenticação:** JWT

---

## 🚀 Funcionalidades

- **Cadastro de usuários** com autenticação JWT
- **CRUD de tarefas**: criar, ler, atualizar e deletar
- **Alteração de prioridade** de tarefas por endpoint separado
- **Alteração de status** de tarefas por endpoint separado
- **Validação de dados** com FluentValidation
- **Testes unitários** garantindo 100% de cobertura

---

## 📦 Estrutura do Projeto

```bash
    ├─ src/
    │ ├─ TaskManager.Api/ # Camada de API
    │ ├─ TaskManager.Application/ # Camada de Aplicação (Use Cases, DTOs)
    │ ├─ TaskManager.Domain/ # Camada de Domínio (Entidades, Interfaces)
    │ ├─ TaskManager.Infrastructure/ # Camada de Infraestrutura (EF Core, Repositórios)
    │ └─ DDD/ # Estrutura DDD aplicada
    │
    ├─ tests/ # Testes unitários e de integração
    │ ├─ TaskManager.Tests/ # Testes do projeto
    │ └─ Coverage/ # Arquivos de cobertura de testes
```


---

## 🔐 Autenticação

- Utiliza **JWT** para autenticação e autorização
- Os endpoints protegidos exigem token válido
- Para gerar o token, faça login via endpoint de autenticação

---

## ⚡ Endpoints Principais

| Método | Endpoint | Descrição |
|--------|----------|-----------|
| POST   | `/auth/login` | Autenticação do usuário e geração de token JWT |
| POST   | `/tasks` | Cria uma nova tarefa |
| GET    | `/tasks` | Lista todas as tarefas do usuário |
| GET    | `/tasks/{id}` | Consulta uma tarefa específica |
| PUT    | `/tasks/{id}` | Atualiza dados de uma tarefa |
| PATCH  | `/tasks/updatepriority/{id}` | Atualiza a prioridade de uma tarefa |
| PATCH  | `/tasks/updatestatus/{id}` | Atualiza o status de uma tarefa |
| DELETE | `/tasks/{id}` | Deleta uma tarefa |

---

## 🧪 Testes

- Todos os **use cases** e **controllers** possuem testes unitários
- Cobertura de testes: **100%**
- Framework: **xUnit**

---

## 📥 Instalação e Execução

1. Clone o repositório:
```bash
git clone https://github.com/seuusuario/TaskMangem.git
```

2. Acesse a pasta do projeto:
```bash
cd TaskMangem
```

3. Configure a string de conexão com o PostgreSQL no appsettings.json:
```bash
"ConnectionStrings": {
  "DefaultConnection": "Host=localhost;Database=TaskMangemDb;Username=seu_usuario;Password=sua_senha"
}
```

4.Execute as migrações do EF Core:
```bash
dotnet ef database update
```

5. Execute a aplicação:
```bash 
dotnet run --project TaskManager.Api
```
6. Acesse a documentação Swagger (se habilitada) em:
```bash
https://localhost:5001/swagger/index.html
```