﻿# ✅ To-DoList API

A **To-DoList API** é um sistema robusto e escalável para gerenciamento de tarefas, desenvolvido com **.NET 8** e estruturado com os princípios de **Clean Architecture** e **Domain-Driven Design (DDD)**. A aplicação oferece recursos modernos como autenticação JWT, validações avançadas, logs estruturados, testes automatizados e persistência de dados com **PostgreSQL em ambiente Docker**.

Projetada com foco em **escalabilidade**, **segurança**, **manutenibilidade** e **boas práticas de engenharia de software**, esta API é ideal tanto para aprendizado quanto para uso em ambientes profissionais.

---

## 📌 Visão Geral do Projeto

A To-DoList API permite que os usuários:

- 🔐 Autentiquem-se de forma segura via JWT  
- 🗂️ Criem, leiam, atualizem e excluam tarefas  
- 🔍 Apliquem filtros por data, status e prioridade  
- 🌐 Tenham suporte multi-idioma (Português e Inglês)  
- 📑 Acessem documentação completa via Swagger  
- 🛡️ Protejam endpoints sensíveis contra abuso e ataques de força bruta  
- 🚀 Implantem facilmente em ambientes Docker ou na nuvem

---

## 🛠️ Tecnologias e Plataformas

### Plataforma
- **.NET 8** – Framework moderno e de alto desempenho para desenvolvimento de APIs
- **Docker Compose** – Ambiente containerizado para banco de dados PostgreSQL
- (Opcional) **Azure App Service** – Hospedagem em nuvem com CI/CD

### Tecnologias Utilizadas
- **ASP.NET Core** – Criação de APIs RESTful
- **Entity Framework Core** – ORM para acesso a dados
- **PostgreSQL** – Banco de dados relacional via Docker
- **JWT Authentication** – Autenticação segura com tokens
- **FluentValidation** – Validação robusta de dados de entrada
- **Swagger / Swashbuckle** – Documentação interativa da API
- **Serilog** – Logging estruturado (console e arquivos)
- **xUnit + FluentAssertions** – Testes automatizados e legíveis
- **AutoMapper** – Mapeamento entre entidades e DTOs
- **GitHub Actions (CI/CD)** – Integração e entrega contínua
- **Localization** – Suporte a múltiplos idiomas
- **Rate Limiting / Segurança de headers** – Proteção contra abusos, XSS e injeções

---

## 🚀 Funcionalidades

- ✅ **Autenticação & Autorização** – Login seguro via JWT Bearer Token
- ✅ **Gerenciamento de Tarefas** – CRUD completo com filtros e paginação
- 🌍 **Localização** – Suporte a Português e Inglês
- 🧾 **Validações** – Precisas e reutilizáveis com FluentValidation
- ⚙️ **Middlewares personalizados** – Tratamento global de exceções, logs e cultura
- 📊 **Logs estruturados** – Serilog com escrita em arquivos e console
- 🧪 **Testes automatizados** – xUnit com FluentAssertions para controller, domínio e middlewares
- 🐳 **Banco de dados via Docker** – PostgreSQL pronto para desenvolvimento

---

## 📁 Estrutura de Projeto

ToDoList/
├── ToDoList.API/ # Camada de apresentação (controllers, middleware, Swagger)
├── ToDoList.Application/ # Casos de uso, validações e serviços
├── ToDoList.Domain/ # Entidades, enums, interfaces, regras de negócio
├── ToDoList.Infrastructure/ # Acesso a dados, EF Core, repositórios
├── ToDoList.Tests/ # Testes automatizados com xUnit
└── docker-compose.yml # Banco de dados PostgreSQL containerizado

---

## 🐳 Executando com Docker Compose

1. **Suba o banco de dados PostgreSQL**:

```bash
	docker-compose up -d
```

2. Aplique as migrações e execute a aplicação:
```bash
	dotnet ef database update --project ToDoList.Infrastructure
	dotnet run --project ToDoList.API
```

3. Acesse a documentação da API: 
```bash
	http://localhost:5000/swagger
```

---

## 🧠 Arquitetura e Boas Práticas

- Clean Architecture: Separação clara entre domínio, aplicação, infraestrutura e apresentação
- DDD: Entidades ricas com regras encapsuladas
- SOLID: Código limpo e de fácil manutenção
- Middlewares customizados: Centralização de logs, exceções e localização
- Segurança: Headers de proteção, autenticação JWT, rate limiting e validações fortes
- CI/CD: Pipeline de integração contínua com GitHub Actions (opcional)

---

## 📄 Licença
Este projeto está licenciado sob os termos da MIT License.
	
---

## ✉️ Contato
Luiz André
📧 [luizandre12042004@gmail.com]
🔗 https://www.linkedin.com/in/luizandr/