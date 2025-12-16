# 💳 Crédito Constituído — API de Consulta e Integração

<p align="center">
  <b>API RESTful desenvolvida em .NET 6 para integração e consulta de créditos constituídos, com processamento assíncrono via mensageria e persistência em banco relacional.</b>
</p>

<p align="center">
  <img src="https://img.shields.io/badge/.NET-512BD4?style=for-the-badge&logo=dotnet&logoColor=white"/>
  <img src="https://img.shields.io/badge/PostgreSQL-336791?style=for-the-badge&logo=postgresql&logoColor=white"/>
  <img src="https://img.shields.io/badge/Kafka-231F20?style=for-the-badge&logo=apachekafka&logoColor=white"/>
  <img src="https://img.shields.io/badge/Docker-2496ED?style=for-the-badge&logo=docker&logoColor=white"/>
</p>

---

## 🧠 Sobre o Projeto

Este projeto foi desenvolvido como **desafio técnico** para implementação de um **microserviço backend** responsável por:

- Receber créditos constituídos via API
- Publicar mensagens em um tópico de mensageria
- Processar essas mensagens em background
- Persistir os dados de forma **individual e idempotente**
- Expor endpoints REST para consulta dos créditos

A aplicação segue princípios de **Clean Architecture**, **SOLID** e **boas práticas de código**, com clara separação de responsabilidades entre camadas.

> [!IMPORTANT] 
> É necessário ter somente o Docker instalado na sua máquina para conseguir executar a aplicação, pois o banco de dados PostgreSQL e o Kafka são orquestrados via Docker Compose.

---

## ⚙️ Stack Técnica

| Camada | Tecnologias |
|:--|:--|
| **API** | .NET 6, ASP.NET Core, Controllers REST |
| **Application** | Casos de uso, DTOs, interfaces, validações |
| **Domain** | Entidades, regras de negócio, Value Objects |
| **Infrastructure** | EF Core, PostgreSQL, Kafka, Repositórios |
| **Mensageria** | Kafka (Producer + Consumer) |
| **Containerização** | Docker + Docker Compose |
| **Testes** | xUnit, FluentAssertions, Testcontainers |

---

## 🐳 Executando o Projeto com Docker

```bash
docker compose up -d --build
```

---

## 📜 Logs da Aplicação

```bash
docker logs -f credito_api
```

---

## 🧪 Testes Automatizados

```bash
dotnet test
```

---

## 👤 Autor

**Matheus Szoke**  

📧 **Email:** [matheusszoke@gmail.com](mailto:matheusszoke@gmail.com)  
💼 **LinkedIn:** [linkedin.com/in/matheusszoke](https://linkedin.com/in/matheusszoke)  
🌐 **Website:** [portfolio.mathszoke.com](https://portfolio.mathszoke.com)

<p align="center">
  <sub>Desenvolvido com 💚 por <strong>Matheus Szoke</strong></sub>
</p>
