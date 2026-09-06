# Deploy e Segurança

Guia da configuração de produção do Sistema de Monitoramento IoT para Aviários.
O `README.md` cobre o uso em desenvolvimento; este arquivo cobre o que muda ao
colocar no ar.

---

## 1. Antes de qualquer coisa: rotacionar o que vazou

As credenciais abaixo estavam escritas em arquivos versionados. Elas continuam
no histórico do git mesmo depois de removidas do código, então **precisam ser
trocadas na origem** — remover do arquivo não basta.

| Credencial | Onde estava | O que fazer |
|---|---|---|
| Senha de app do Gmail (a que estava em `Email:Password`) | `appsettings.json` | **Revogar** em [myaccount.google.com/apppasswords](https://myaccount.google.com/apppasswords) e gerar outra |
| Chave JWT | `appsettings.json` | Já substituída por uma chave aleatória nova no `.env` |
| Chave secreta do ESP32 | `appsettings.json`, `esp32/src/config.h`, `README.md` | Já substituída no `.env` e em `esp32/src/secrets.h` |
| Senha do SQL Server (`sa`) | `appsettings.json`, `docker-compose.yml` | Já substituída no `.env` |

A revogação da senha do Gmail é a única que não dá para fazer pelo código —
enquanto ela não for revogada, quem tiver o histórico do repositório consegue
enviar e-mail pela conta de remetente do sistema.

Além disso, considere:

- deixar o repositório **privado**, se ainda for público;
- limpar o histórico (`git filter-repo`) se o repositório precisar seguir público.

---

## 2. Como a configuração funciona agora

Nenhum segredo fica em arquivo versionado. `appsettings.json` guarda apenas o
que não é sensível (issuer do JWT, porta SMTP, nível de log). Todo o resto vem
de variável de ambiente, lida do `.env` pelo Docker Compose.

O `.env` **não vai para o git** (está no `.gitignore`). O modelo versionado é o
`.env.example`, sem valores reais.

A API valida a configuração **no startup** e se recusa a subir se algo estiver
faltando — em vez de rodar com um default inseguro:

| Variável | Obrigatória | Regra |
|---|---|---|
| `ConnectionStrings__DefaultConnection` | sempre | não pode ser vazia |
| `Jwt__Key` | sempre | mínimo 32 caracteres |
| `Esp32Config__SecretKey` | sempre | mínimo 24 caracteres |
| `Frontend__BaseUrl` | em `Production` | precisa ser URL absoluta |
| `Email__*` | em `Production` | host, usuário e senha preenchidos |

Em `Development` as duas últimas são opcionais, para não travar quem só quer
rodar a API local.

Para rodar a API fora do Docker (`dotnet run`), use user-secrets em vez de
editar o `appsettings.json`:

```bash
cd backend/FarmSystemProject
dotnet user-secrets set "Jwt:Key" "<chave de 32+ caracteres>"
dotnet user-secrets set "Esp32Config:SecretKey" "<chave de 24+ caracteres>"
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "<connection string>"
```

---

## 3. Subindo em produção

```bash
# 1. No servidor, a partir da raiz do projeto
cp .env.example .env

# 2. Preencha o .env. No mínimo:
#      ASPNETCORE_ENVIRONMENT=Production
#      MSSQL_SA_PASSWORD, DB_APP_PASSWORD  (senhas novas, exclusivas deste ambiente)
#      JWT_KEY, ESP32_SECRET_KEY           (valores novos, diferentes do dev)
#      FRONTEND_BASE_URL=https://seu-dominio
#      EMAIL_USER / EMAIL_PASSWORD         (senha de app nova do Gmail)
#      SWAGGER_ENABLED=false
#      FRONTEND_BIND=127.0.0.1

# 3. Restrinja o acesso ao arquivo de segredos
chmod 600 .env

# 4. Suba (sem -f, o compose usa só o docker-compose.yml de produção)
docker compose up -d --build
```

> **Não** copie o `.env` de desenvolvimento para o servidor. Gere segredos
> novos para produção — um ambiente comprometido não deve derrubar o outro.

### O que fica exposto

| Serviço | Porta no host | Alcance |
|---|---|---|
| `database` (SQL Server) | nenhuma | só a rede interna do compose |
| `backend` (API) | nenhuma | só via proxy `/api` do frontend |
| `frontend` (nginx) | `127.0.0.1:3000` | só o próprio host |

A única porta publicada é a do frontend, e apenas em `127.0.0.1`. A ideia é que
um proxy com TLS (nginx, Caddy, Traefik) no host receba a internet na 443,
termine o HTTPS e encaminhe para `127.0.0.1:3000`.

Exemplo com Caddy (`Caddyfile`), que já resolve o certificado sozinho:

```
aviario.seudominio.com {
    reverse_proxy 127.0.0.1:3000
}
```

Colocar `FRONTEND_BIND=0.0.0.0` publica a aplicação em HTTP puro, sem
certificado — senhas e tokens JWT trafegariam em texto claro. Só faz sentido
se houver outro elemento de rede fazendo o TLS.

### Firewall do servidor

Deixe abertas apenas 80, 443 e a porta de SSH. A 1433 nunca deve ser
alcançável de fora — com este compose ela não é sequer publicada no host, mas
uma regra de firewall é a segunda barreira.

---

## 4. CORS

A política é uma **lista explícita de origens**, definida em
`CORS_ALLOWED_ORIGIN_0` / `_1`. Não existe mais `AllowAnyOrigin`.

No deploy padrão a lista fica **vazia** — e está certo: o nginx do frontend faz
proxy de `/api` para a API, então o navegador conversa sempre com a mesma
origem e CORS nem entra em jogo. Vazio significa "nenhuma origem externa
liberada", não "todas".

Só preencha se algum cliente em outro domínio precisar chamar a API
diretamente. Nesse caso, ajuste também `EXPO_PUBLIC_API_URL`. Nunca use `*`.

Os métodos permitidos são `GET, POST, PUT, PATCH, DELETE, OPTIONS`, e os
cabeçalhos, apenas `Authorization`, `Content-Type`, `Accept` e `X-Secret-Key`.

---

## 5. Swagger em produção

O Swagger fica **desligado** quando `ASPNETCORE_ENVIRONMENT=Production`.

Para cadastrar sensores (passo 3 do README), ligue temporariamente:

```bash
# no .env do servidor
SWAGGER_ENABLED=true
docker compose up -d backend

# ... cadastre os sensores em https://seu-dominio/swagger ...

# e desligue logo em seguida
SWAGGER_ENABLED=false
docker compose up -d backend
```

Alternativa sem Swagger, direto pela API:

```powershell
Invoke-RestMethod -Method Post -Uri "https://seu-dominio/api/sensors" `
  -Headers @{ "X-Secret-Key" = "<ESP32_SECRET_KEY>" } `
  -ContentType "application/json" `
  -Body '{ "macAddress": "24:0A:C4:00:01:10", "type": 1, "lotId": 1 }'
```

---

## 6. Banco de dados

O backend **não se conecta mais como `sa`**. Na subida, o serviço `db-init`
roda `scripts/db/init-app-user.sql` e cria:

- o banco (`DB_NAME`, padrão `FarmSystemDb`);
- o login `DB_APP_USER` (padrão `farmsystem_app`), sem nenhuma role de
  servidor — ele não enxerga nem administra as outras bases da instância;
- esse usuário como `db_owner` **apenas** do banco da aplicação, porque o EF
  Core aplica as migrations (`CREATE`/`ALTER TABLE`) no startup.

O script é idempotente: roda a cada `docker compose up` sem efeito colateral, e
sincroniza a senha do login com o valor atual do `.env`. A senha do `sa` passa a
ser usada só nesse init.

A conexão usa `Encrypt=True` com `TrustServerCertificate=True` — o tráfego
entre backend e banco é cifrado; o certificado é o autoassinado do container,
e a conexão nunca sai da rede interna.

> **Senhas de banco**: use apenas letras, números e `! # % - _`. Os caracteres
> `$ ' "` quebram a substituição de variáveis do `sqlcmd` no script de init.

### Backup

O volume `sqlvolume` guarda os dados. Um backup lógico:

```bash
docker compose exec database /opt/mssql-tools18/bin/sqlcmd \
  -S localhost -U sa -P "$MSSQL_SA_PASSWORD" -C \
  -Q "BACKUP DATABASE [FarmSystemDb] TO DISK='/var/opt/mssql/data/farmsystem.bak' WITH INIT, COMPRESSION"

docker compose cp database:/var/opt/mssql/data/farmsystem.bak ./farmsystem.bak
```

---

## 7. ESP32

As credenciais saíram de `esp32/src/config.h` (versionado) para
`esp32/src/secrets.h` (no `.gitignore`). O `config.h` só guarda pinagem e
calibração, e falha a compilação com mensagem clara se o `secrets.h` não
existir.

Ao clonar o projeto:

```powershell
Copy-Item esp32\src\secrets.example.h esp32\src\secrets.h
```

Depois preencha:

- `SECRET_KEY` — exatamente o mesmo valor de `ESP32_SECRET_KEY` no `.env`;
- `API_URL` — `https://seu-dominio/api/sensors/readings` em produção;
- `WIFI_SSID` / `WIFI_PASSWORD` — a rede real, se não for simulação.

> Com `API_URL` em `https://`, o firmware hoje usa `setInsecure()`, ou seja,
> **não valida o certificado do servidor**. Para hardware em campo, troque por
> `setCACert()` com a raiz da CA do seu domínio (`esp32/src/wifi/wifi.cpp`).

---

## 8. Cenário: frontend em hospedagem separada

Tudo até aqui assume o padrão: frontend e backend na mesma máquina, mesmo
domínio, o nginx do frontend fazendo proxy de `/api` para o backend. É por
isso que CORS pode ficar vazio e `EXPO_PUBLIC_API_URL` também.

Se o frontend for publicado em outro lugar — hospedagem compartilhada, Vercel,
Netlify — enquanto o backend fica sozinho na VPS, três coisas mudam:

**1. O serviço `frontend` fica atrás de um profile.** Por padrão,
`docker compose up -d --build` sobe só `database` + `db-init` + `backend`. A
pasta `frontend/` nem precisa existir na VPS. Para incluir o frontend também
(stack completa numa máquina só), seria:

```bash
docker compose --profile full up -d --build
```

ou `COMPOSE_PROFILES=full` no `.env` — é o que o `.env` de desenvolvimento
local já usa, para o `docker compose up` do dia a dia continuar subindo tudo.

**2. O backend precisa de domínio e HTTPS próprios.** Sem o nginx do frontend
na frente, um proxy no host da VPS (Caddy, por exemplo) fala com a API pela
porta publicada em loopback:

```
Caddyfile:
    api.seudominio.com {
        reverse_proxy 127.0.0.1:8080
    }
```

**3. CORS deixa de poder ficar vazio.** Preencha com o domínio exato de onde
o frontend é servido:

```ini
FRONTEND_BASE_URL=https://seudominio.com
CORS_ALLOWED_ORIGIN_0=https://seudominio.com
ALLOWED_HOSTS=api.seudominio.com
```

**4. O build do frontend acontece fora da VPS**, na máquina que for gerar os
arquivos estáticos (local, ou um pipeline de CI). Crie um `.env` dentro de
`frontend/FarmSystemProject/` com:

```ini
EXPO_PUBLIC_API_URL=https://api.seudominio.com
```

e rode `npx expo export --platform web`. A pasta `dist/` resultante é o que
sobe para a hospedagem do frontend — puro HTML/JS/CSS, sem Docker.

---

## 9. Checklist antes de publicar

- [ ] Senha de app do Gmail antiga **revogada** no Google
- [ ] `.env` de produção criado no servidor, com segredos diferentes dos de dev
- [ ] `chmod 600 .env`
- [ ] `ASPNETCORE_ENVIRONMENT=Production`
- [ ] `SWAGGER_ENABLED=false`
- [ ] `FRONTEND_BASE_URL` apontando para o domínio real
- [ ] `FRONTEND_BIND=127.0.0.1` e proxy com TLS na frente
- [ ] Firewall liberando só 80, 443 e SSH
- [ ] `docker compose -f docker-compose.yml config` sem erro
- [ ] `docker compose logs backend` sem aviso de Swagger ligado
- [ ] Login e "esqueci minha senha" testados no domínio real
- [ ] Backup do banco agendado

---

## 10. O que ficou de fora

Itens que não foram implementados e que valem uma próxima rodada:

- **Rate limiting** no login e no `/api/auth/forgot-password`. Existe bloqueio
  de conta por 15 minutos após tentativas erradas, mas não há limite por IP.
- **Refresh token**. Hoje o JWT dura 60 minutos e o usuário precisa relogar.
- **Rotação da chave do ESP32** sem recompilar o firmware: a chave é um
  `#define`, então trocá-la exige novo flash em cada dispositivo.
- **Segredos em cofre** (Docker secrets, AWS Secrets Manager, Azure Key Vault)
  em vez de `.env` em disco, se o ambiente crescer.
- **HSTS e headers de segurança na borda**: o nginx do container já envia
  `X-Content-Type-Options`, `X-Frame-Options`, `Referrer-Policy` e
  `Permissions-Policy`, mas `Strict-Transport-Security` precisa vir do proxy
  que termina o TLS.
