

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

---

# 📋 Tipo da tarefa

| Tipo              | Valor |
| ----------------- | ----- |
| EnviarEmail       | 1     |
| GerararRelatorio  | 2     |
| ProcessarArquivo  | 3     |

---

Tipo envio de email: 
```json
{
  "tipo": 1,
  "dadosJson": "{\"destinatario\":\"teste@email.com\",\"assunto\":\"Bem-vindo\"}"
}
```

Tipo gerarar relatorio: 
```json
{
  "tipo": 2,
  "dadosJson": "{\"titulo\":\"Relatório de Vendas\",\"geradoPor\":\"João\",\"formato\":\"PDF\",\"filtros\":\"status\":\"Pago\"}"
}
```

Tipo processar arquivo: 
```json
{
  "tipo": 3,
  "dadosJson": "{\"nomeArquivo\":\"clientes_joao_2026.csv\",\"caminho\":\"/uploads/relatorios\",\"acao\":\"ProcessarArquivo\"}"
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

# 📋 Tipo da tarefa

| Tipo              | Valor |
| ----------------- | ----- |
| EnviarEmail       | 1     |
| GerararRelatorio  | 2     |
| ProcessarArquivo  | 3     |

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
 "tipo": 2,
"dadosJson": "{\"titulo\":\"teste erro\"}"
}
```

Ao enviar um `dadosJson` contendo a string erro proposital, o resultado esperado será::

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

# 🐳 Comandos importantes Docker / Docker Compose

Essa seção reúne os comandos mais úteis para subir, debugar e manter o ambiente do projeto.

---

## 🍎 Mac

No terminal:

```bash
open -a Docker
```

Isso abre o Docker Desktop.

Se quiser verificar se subiu corretamente:

```bash
docker ps
```

## 🪟 Windows

CMD
```bash
start "" "C:\Program Files\Docker\Docker\Docker Desktop.exe"
```

PowerShell
```bash
Start-Process "C:\Program Files\Docker\Docker\Docker Desktop.exe"
```

Depois teste:

```bash
docker ps
```

## 🚀 Subir o ambiente

```bash
docker compose up --build
```

### O que faz
- sobe MongoDB
- sobe RabbitMQ
- sobe API
- sobe Worker
- recompila as imagens

---

## ⛔ Derrubar o ambiente

```bash
docker compose down
```

### O que faz
- para todos os containers
- remove a rede criada

---

## 🗑️ Derrubar e apagar volumes

```bash
docker compose down -v
```

### O que faz
- remove containers
- remove volumes
- apaga os dados do MongoDB

> 💡 Muito útil quando houver mudança no schema das entidades.

---

## 🔄 Rebuild completo

```bash
docker compose up --build --force-recreate
```

### Quando usar
- alterações no Dockerfile
- mudanças em dependências
- problemas de cache

---

## 📋 Ver status dos containers

```bash
docker compose ps
```

Permite validar se:
- API está online
- Worker está online
- RabbitMQ está online
- MongoDB está online

---

## 📜 Ver logs de todos os serviços

```bash
docker compose logs -f
```

---

## 📜 Ver logs apenas da API

```bash
docker compose logs -f api
```

Útil para:
- erros HTTP
- problemas no Swagger
- exceções em controllers

---

## 📜 Ver logs apenas do Worker

```bash
docker compose logs -f worker
```

Esse é um dos comandos mais importantes do projeto.

Permite acompanhar:
- consumo da fila
- retries
- falhas
- reenvio de tarefas
- conclusão do processamento

---

## 📜 Ver logs do RabbitMQ

```bash
docker compose logs -f rabbitmq
```

---

## 📜 Ver logs do MongoDB

```bash
docker compose logs -f mongodb
```

---

## 🔁 Reiniciar apenas um serviço

### Reiniciar Worker
```bash
docker compose restart worker
```

### Reiniciar API
```bash
docker compose restart api
```

---

## 🏗️ Build individual da API

```bash
docker compose build api
```

---

## 🏗️ Build individual do Worker

```bash
docker compose build worker
```

---

## 🐚 Entrar no container da API

```bash
docker exec -it processadorf360-api sh
```

---

## 🐚 Entrar no container do Worker

```bash
docker exec -it processadorf360-worker sh
```

---

## 🐚 Entrar no MongoDB

```bash
docker exec -it processadorf360-mongodb mongosh
```

---

# 🍃 MongoDB via Docker — acesso e comandos úteis

Essa seção ajuda na validação manual, debugging e reset rápido do ambiente.

---

## 🐚 Acessar MongoDB dentro do container

```bash
docker exec -it processadorf360-mongodb mongosh
```

---

## 🗄️ Selecionar o banco

```javascript
use ProcessadorTarefasF360Db
```

---

## 📚 Listar collections

```javascript
show collections
```

---

## 🔍 Buscar todas as tarefas

```javascript
db.tarefas.find().pretty()
```

👉 equivalente ao `SELECT *`

---

## 🔍 Buscar tarefa por ID

```javascript
db.tarefas.find({ _id: "SEU_ID" }).pretty()
```

---

## 🔍 Buscar tarefas com erro

```javascript
db.tarefas.find({ Status: 3 }).pretty()
```

---

## 🔍 Buscar tarefas pendentes

```javascript
db.tarefas.find({ Status: 0 }).pretty()
```

---

## 🔍 Buscar tarefas por tipo

```javascript
db.tarefas.find({ Tipo: 1 }).pretty()
```

Exemplo:
- `1` = EnviarEmail
- `2` = GerarRelatorio
- `3` = ProcessarArquivo

---

## 🔢 Contar tarefas

```javascript
db.tarefas.countDocuments()
```

---

## ✏️ Atualizar status

```javascript
db.tarefas.updateOne(
  { _id: "SEU_ID" },
  { $set: { Status: 2 } }
)
```

👉 marca como concluída

---

## ✏️ Atualizar tentativas

```javascript
db.tarefas.updateOne(
  { _id: "SEU_ID" },
  { $set: { Tentativas: 2 } }
)
```

---

## ➕ Incrementar tentativas

```javascript
db.tarefas.updateOne(
  { _id: "SEU_ID" },
  { $inc: { Tentativas: 1 } }
)
```

---

## 🗑️ Deletar uma tarefa

```javascript
db.tarefas.deleteOne({ _id: "SEU_ID" })
```

---

## 🗑️ Deletar todas as tarefas

```javascript
db.tarefas.deleteMany({})
```

👉 ideal para reset rápido

---

## 💥 Dropar a collection

```javascript
db.tarefas.drop()
```

⚠️ remove dados + estrutura

---

## 💥 Dropar banco inteiro

```javascript
db.dropDatabase()
```

⚠️ remove tudo

---

## 🧪 Simular retry manualmente

```javascript
db.tarefas.updateOne(
  { _id: "SEU_ID" },
  {
    $set: {
      Status: 0,
      Tentativas: 0,
      MensagemErro: null
    }
  }
)
```

👉 excelente para demonstração ao vivo

---

## 📋 Exemplo de documento salvo

```json
{
  "_id": "123",
  "Tipo": 1,
  "DadosJson": "{\"destinatario\":\"teste@email.com\"}",
  "Status": 0,
  "Tentativas": 0,
  "MaxTentativas": 3,
  "DataCriacao": "2026-03-30T00:00:00Z"
}
```





