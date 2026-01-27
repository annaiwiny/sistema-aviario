# FarmSystem

# Documentação da API - FarmSystem

Esta API fornece recursos para gerenciamento de sistema aviário.

## Endereço Base
O endereço base depende do ambiente, mas por padrão localmente são:
- HTTP: `http://localhost:5015`
- HTTPS: `https://localhost:7172`

## Autenticação
A maioria das rotas, exceto login e cadastro, podem vir a exigir autenticação via Token Bearer JWT.
Para obter um token, utilize a rota de login e inclua o token no header `Authorization` das requisições:
`Authorization: Bearer <seu_token>`

---

## Rotas

### Auth
Rotas relacionadas à autenticação e recuperação de senha.

#### Login
Realiza o login do usuário e retorna o token JWT.

- **Método:** `POST`
- **URL:** `/api/Auth/login`
- **Corpo da Requisição (JSON):**
  ```json
  {
    "email": "user@example.com",
    "password": "senha_secreta"
  }
  ```

#### Esqueci Minha Senha
Envia um e-mail de recuperação de senha (simulado).

- **Método:** `POST`
- **URL:** `/api/Auth/forgot-password`
- **Corpo da Requisição (JSON):**
  ```json
  {
    "email": "user@example.com"
  }
  ```

#### Redefinir Senha
Redefine a senha utilizando o token recebido.

- **Método:** `POST`
- **URL:** `/api/Auth/reset-password`
- **Corpo da Requisição (JSON):**
  ```json
  {
    "token": "token_recebido",
    "newPassword": "nova_senha_secreta"
  }
  ```

---

### User
Gerenciamento de usuários.

#### Criar Usuário
Cadastra um novo usuário no sistema.

- **Método:** `POST`
- **URL:** `/api/User`
- **Corpo da Requisição (JSON):**
  ```json
  {
    "name": "Nome Completo",
    "birthDate": "2000-01-01T00:00:00",
    "email": "email@example.com",
    "password": "senha_forte",
    "cpf": "12345678901",
    "state": "SP",
    "city": "São Paulo",
    "address": "Rua Exemplo, 123",
    "phone": "11999999999"
  }
  ```

#### Obter Meu Perfil
Retorna os dados do usuário logado.

- **Método:** `GET`
- **URL:** `/api/User/me`
- **Headers:** Requer Token Bearer.

#### Atualizar Meu Perfil
Atualiza os dados do usuário logado.

- **Método:** `PUT`
- **URL:** `/api/User/me`
- **Headers:** Requer Token Bearer.
- **Corpo da Requisição (JSON):**
  - Campos opcionais (envie apenas o que deseja alterar)
  ```json
  {
    "name": "Novo Nome",
    "email": "novo@email.com",
    "phone": "11888888888"
    // ... outros campos
  }
  ```

---

### Race (Raças)
Gerenciamento de raças de aves.

#### Listar Todas
- **Método:** `GET`
- **URL:** `/api/Race`

#### Obter por ID
- **Método:** `GET`
- **URL:** `/api/Race/{id}`

#### Criar Raça
- **Método:** `POST`
- **URL:** `/api/Race`
- **Corpo da Requisição (JSON):**
  ```json
  {
    "name": "Nome da Raça"
  }
  ```

#### Atualizar Raça
- **Método:** `PUT`
- **URL:** `/api/Race/{id}`
- **Corpo da Requisição (JSON):**
  ```json
  {
    "id": 1,
    "name": "Nome Editado"
  }
  ```
  *Nota: O ID na URL deve coincidir com o ID no corpo.*

#### Deletar Raça
- **Método:** `DELETE`
- **URL:** `/api/Race/{id}`

---

### Egg (Produção de Ovos)
Monitoramento de produção de ovos.

#### Listar Todas as Coletas
- **Método:** `GET`
- **URL:** `/api/Egg`

#### Obter Total por Data
Retorna o total coletado em uma data específica.
- **Método:** `GET`
- **URL:** `/api/Egg/{collectDate}`
  - Exemplo: `/api/Egg/2023-10-25`

#### Registrar Coleta
- **Método:** `POST`
- **URL:** `/api/Egg`
- **Corpo da Requisição (JSON):**
  ```json
  {
    "collectDate": "2023-10-25T00:00:00",
    "collectQuantity": 150,
    "lotId": 1
  }
  ```

---

### Mortality (Mortalidade)
Monitoramento de mortalidade das aves.

#### Listar Todos os Registros
- **Método:** `GET`
- **URL:** `/api/Mortality`

#### Obter Total por Data
- **Método:** `GET`
- **URL:** `/api/Mortality/{dateDeath}`

#### Registrar Mortalidade
- **Método:** `POST`
- **URL:** `/api/Mortality`
- **Corpo da Requisição (JSON):**
  ```json
  {
    "dateDeath": "2023-10-25T00:00:00",
    "deathQuantity": 5,
    "cutQuantity": 0,
    "reason": "Causa natural",
    "lotId": 1
  }
  ```
