

# 🚀 ProcessadorTarefasF360

![.NET](https://img.shields.io/badge/.NET-9.0-purple)
![ASP.NET Core](https://img.shields.io/badge/ASP.NET%20Core-Web%20API-blue)
![MongoDB](https://img.shields.io/badge/MongoDB-NoSQL-green)
![RabbitMQ](https://img.shields.io/badge/RabbitMQ-Mensageria-orange)
![Docker](https://img.shields.io/badge/Docker-Compose-blue)
![xUnit](https://img.shields.io/badge/Testes-xUnit-informational)


Sistema de processamento de tarefas em background desenvolvido em **.NET 9**, utilizando **MongoDB**, **RabbitMQ** e **Worker Service**, com ambiente dockerizado via **Docker Compose**.

---

# 🧠 Visão geral

Este projeto demonstra a construção de um sistema assíncrono completo:

- API recebe tarefas
- MongoDB persiste
- RabbitMQ faz o desacoplamento
- Worker processa em background

---

# 🏗️ Arquitetura

```text
[Cliente]
   │
   ▼
[API]
   │
   ├── MongoDB (persistência)
   │
   └── RabbitMQ (fila)
            │
            ▼
        [Worker]
            │
            ▼
       Atualiza status
````

---

# 🔄 Fluxo do sistema

1. API recebe requisição
2. Salva tarefa como `Pendente`
3. Envia para RabbitMQ
4. Worker consome
5. Processa tarefa
6. Atualiza status:

   * Concluída
   * Erro (com retry)

---

# 📁 Estrutura do projeto

```text
src/
 ├── Api
 ├── Core
 └── Worker

tests/
 └── Tests

docker-compose.yml
```

---

# ⚙️ Tecnologias

* .NET 9
* ASP.NET Core
* MongoDB
* RabbitMQ
* Worker Service
* Docker
* xUnit + Moq

---

# ▶️ Como rodar

## 🔹 Com Docker

```bash
docker compose up --build
```

### Acessos

| Serviço  | URL                                                            |
| -------- | -------------------------------------------------------------- |
| API      | [http://localhost:8081](http://localhost:8081)                 |
| Swagger  | [http://localhost:8081/swagger](http://localhost:8081/swagger) |
| RabbitMQ | [http://localhost:15672](http://localhost:15672)               |

Login RabbitMQ:

```
guest / guest
```

---

## 🔹 Sem Docker

1. Suba MongoDB e RabbitMQ localmente
2. Ajuste `appsettings.json` para `localhost`

```bash
dotnet run --project src/ProcessadorTarefasF360.Api
dotnet run --project src/ProcessadorTarefasF360.Worker
```

---

# 📡 Endpoints

## Criar tarefa

```http
POST /api/tarefas
```

```json
{
  "descricao": "Processar arquivo"
}
```

---

## Listar tarefas

```http
GET /api/tarefas
```

---

## Buscar por ID

```http
GET /api/tarefas/{id}
```

---

# 📊 Status da tarefa

| Status          | Valor |
| --------------- | ----- |
| Pendente        | 0     |
| EmProcessamento | 1     |
| Concluida       | 2     |
| Erro            | 3     |

---

# 🧪 Testes automatizados

Rodar todos os testes:

```bash
dotnet test
```

Tecnologias:

* xUnit
* Moq

---

# 🧪 Testes manuais

## Criar tarefa

```bash
curl -X POST http://localhost:8081/api/tarefas \
  -H "Content-Type: application/json" \
  -d '{"descricao":"teste"}'
```

---

## Listar tarefas

```bash
curl http://localhost:8081/api/tarefas
```

---

## Buscar tarefa

```bash
curl http://localhost:8081/api/tarefas/{id}
```

---

## Forçar erro (testar retry)

```json
{
  "descricao": "erro"
}
```

Resultado:

* múltiplas tentativas
* status final = Erro

---

# ⚔️ Concorrência

* Atualização atômica no MongoDB
* Evita processamento duplicado

---

# 🔁 Retry

* Tentativas incrementais
* Reenvio automático para fila
* Limite máximo definido

---

# ⚠️ Pontos de atenção

* RabbitMQ pode demorar a subir → Worker usa retry
* Docker pode ter conflito de porta
* API roda em HTTP (sem HTTPS no container)

---




