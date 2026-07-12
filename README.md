# Sistema de Monitoramento IoT para Aviários
Uma solução full-stack que integra simulação de hardware IoT (ESP32) para monitoramento em tempo real de temperatura, umidade e nível de água, conectado a uma API REST em .NET e uma interface web para gestão de aviários e lotes.

---

## Arquitetura do Sistema
O projeto é dividido em três camadas principais:

1. **IoT (`/esp32`):** Simulação de um ESP32 com sensores DHT22 (Temperatura/Umidade) e HX711 (Nível de Água) rodando sobre o Wokwi + PlatformIO.
2. **Backend (`/backend`):** API RESTful desenvolvida em ASP.NET Core e Entity Framework integrada a um banco de dados SQL Server 2022, ambos conteinerizados com Docker.
3. **Frontend (`/frontend/FarmSystemProject`):** Aplicação web também conteinerizada com Docker, responsável pelo consumo dos dados, monitoramento de status em tempo real e gestão dos aviários e lotes.

---

## Pré-requisitos
Antes de começar, certifique-se de ter as seguintes ferramentas instaladas em sua máquina:
* [Git](https://git-scm.com/)
* [Docker Desktop](https://www.docker.com/products/docker-desktop/) (com Docker Compose)
* [Visual Studio Code](https://code.visualstudio.com/)
* Extensões obrigatórias no VS Code:
  * **PlatformIO IDE** (para compilar o código C++ do ESP32)
  * **Wokwi Simulator** (para rodar a simulação física virtual)

* ⚠️ **Atenção sobre o Wokwi:** A extensão do Wokwi exige uma ativação de licença gratuita para rodar simulações no seu computador. Assim que instalar a extensão no VS Code, faça o seguinte:
  * Pressione `Ctrl + Shift + P` para abrir a Paleta de Comandos.
  * Digite: `Wokwi: Request a new license` e pressione Enter.
  * Clique em `GET YOUR LICENSE` e faça login com algum dos métodos disponibilizados.
  * Após feito o login, clique em `GET YOUR LICENSE` novamente.

---

## Passo a Passo de Execução
Siga a ordem exata abaixo para garantir que todo o ecossistema (Hardware, Backend, Banco de Dados e Interface Web) consiga se comunicar sem erros.

### 1. Clonando o Projeto e Subindo os Containers (Docker)
1. Abra o seu terminal e faça o clone do repositório para a sua máquina:
```bash
git clone https://github.com/annaiwiny/sistema-aviario.git
```

2. Entre na pasta raiz do projeto recém-clonado:
```bash
cd sistema-aviario
```

3. Certifique-se de que o aplicativo do **Docker Desktop** esteja aberto e rodando. Então, execute o comando abaixo para construir as imagens e iniciar todos os serviços em segundo plano:
```bash
docker-compose up -d --build
```

Aguarde até que o terminal confirme que todos os containers estão rodando. Assim que o processo terminar, seus ambientes estarão acessíveis nos seguintes endereços:
* **Frontend:** `http://localhost:3000`
* **Backend Swagger** `http://localhost:5000/swagger`

---

### 2. Compilando e Rodando o Hardware Virtual (ESP32)
Como o nosso ESP32 é um hardware simulado, nós precisamos primeiro compilar o código escrito em C++ para a linguagem de máquina que o microcontrolador entende.

#### Abrindo a pasta correta do Hardware
Para que o PlatformIO consiga ler as configurações do projeto, ele precisa que a pasta do hardware esteja aberta diretamente no editor:
1. No seu VS Code, vá no menu superior em **File** (Arquivo) > **Open Folder...** (Abrir Pasta...).
2. Navegue até a pasta onde você clonou o repositório e selecione especificamente a subpasta `/esp32`.
3. Clique em **Abrir Pasta**. O VS Code dará uma leve recarregada e ativará as ferramentas do PlatformIO.

⚠️ Assim que a pasta abrir, olhe no canto inferior direito do VS Code. Você verá uma notificação rodando algo como PlatformIO: Configuring project. Aguarde essa barrinha de progresso sumir antes de ir para o próximo passo.

#### Compilando o código pelo Terminal
1. Olhe no canto inferior esquerdo do rodapé do VS Code.
2. Procure e clique no ícone de um terminal chamado **PlatformIO: New terminal**. 
3. Digite o comando abaixo e pressione **Enter**:

```bash
pio run
```

Aguarde até que o terminal exiba uma mensagem verde escrita **`[SUCCESS]`**.

#### Ligando o Simulador
Com o código compilado e pronto podemos ligar o nosso ESP32 virtual:
1. No painel de explorador de arquivos à esquerda, clique para abrir o arquivo `diagram.json`.
2. Abra a Paleta de Comandos do VS Code pressionando os atalhos `Ctrl + Shift + P`.
3. Digite `Wokwi: Start Simulator` e pressione Enter.

Preste atenção no terminal que se abrirá logo abaixo da simulação. O ESP32 vai imprimir o seu **Endereço MAC** (será algo parecido com: `24:0A:C4:00:01:10`). **Copie e guarde esse endereço**, nós vamos precisar dele no próximo passo.

⚠️ Atenção ao Código HTTP 404 ou -1. Assim que ligar a simulação, é normal e esperado que o ESP32 tente enviar os dados e receba uma mensagem de erro no terminal como **`Código HTTP: 404`** (junto com a mensagem "Nenhum sensor encontrado..."). Isso é apenas a segurança e validação do nosso Backend. (O erro `Connection Refused (-1)` só acontecerá se o container do Backend estiver desligado).

---

### 3. A criação dos sensores

Por decisões de arquitetura, o frontend da aplicação não permite a criação de sensores por usuários comuns. A instalação e associação de um hardware físico a um lote de aviário é um processo feito por administradores.

Portanto, para criar e cadastrar os seus sensores no sistema, nós vamos utilizar a nossa interface interativa da API (Swagger) em conjunto com a nossa chave secreta de segurança. Siga os passos abaixo:

#### Criando o seu Lote na Interface Web

1. Abra o seu navegador de preferência e acesse o nosso Frontend digitando: `http://localhost:3000`
2. Faça o seu cadastro e login no sistema.
2. Crie um novo Aviário e, logo em seguida, crie um Lote dentro dele.
3. Anote e guarde o ID desse Lote que você acabou de criar (se for o seu primeiro, será o ID 1).

#### Cadastrando os sensores no Swagger
1. Em uma nova aba do navegador, acesse o nosso Swagger digitando: `http://localhost:5000/swagger`
2. Role a página até encontrar uma barra verde com a rota: `POST /api/sensors`. Clique nela para expandir as opções..
3. Cole a nossa chave secreta de segurança: `Aviario_Esp32_Key_SuperSecreta123`
4. Logo abaixo, na grande caixa de texto chamada Request body, apague o que estiver escrito e cole o JSON abaixo. Lembre-se de colocar o Endereço MAC exato que você copiou do terminal do Wokwi, o ID do Lote que você acabou de criar no Frontend e o tipo de sensor:

```bash
{
  "macAddress": "24:0A:C4:00:01:10",
  "type": 1,
  "lotId": 1
}
```

#### Tipos de Sensores Disponíveis:
* `1` = **Temperatura**
* `2` = **Umidade**
* `3` = **Nível de Água**

---

### 4. Monitoramento e Interatividade em Tempo Real
1. Volte ao seu VS Code, vá no arquivo do simulador (`diagram.json`) e certifique-se de que a simulação do Wokwi está rodando.
2. Olhe o terminal do simulador no rodapé do VS Code. O ESP32 começará a enviar pacotes HTTP, só que agora, em vez de mensagens de erro, você verá as respostas com o código de sucesso `200 OK`

#### Interagindo graficamente com os Sensores no VS Code
* **Para mudar a Temperatura e Umidade:** Com a simulação rodando, clique em cima do sensor **DHT22** lá na tela do Wokwi no VS Code. Vai aparecer uma caixa flutuante com dois controles deslizantes. Mova as barrinhas para os lados e veja o ESP32 detectar as mudanças.
* **Para mudar o Nível de Água:** Clique no sensor de carga **HX711** e deslize a barrinha para simular o esvaziamento ou enchimento do reservatório de água dos frangos.

---

### 5. Encerrando a Simulação e os Containers

#### Desligando o ESP32 Virtual (Wokwi)
1. No VS Code, vá na aba do arquivo `diagram.json` onde a simulação está acontecendo.
2. Clique no ícone de **Pause** bem no topo da tela do simulador ou simplesmente feche o VS Code.

#### Parando os Serviços do Docker
Para desligar o Banco de Dados, o Backend e o Frontend ao mesmo tempo de forma limpa e segura:
1. Abra o terminal na pasta raiz do projeto (`sistema-aviario`).
2. Execute o comando abaixo para derrubar os containers:

```bash
docker-compose down
```

Aguarde até o terminal confirmar a parada de todos os serviços. O Docker manterá os seus dados salvos no banco de dados para a próxima vez que você subir o projeto. 

Caso queira um reset completo use o comando:

```bash
docker-compose down -v
```