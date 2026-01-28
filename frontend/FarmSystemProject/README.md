# Sistema Aviário - Frontend 🐔

Este é o aplicativo móvel do Sistema de Gestão Aviária, desenvolvido com **React Native**, **Expo** e **TypeScript**. O objetivo do aplicativo é permitir que pequenos e médios produtores gerenciem seus lotes, acompanhem a mortalidade, produção de ovos e recebam notificações importantes sobre a granja.

## 📱 Funcionalidades

### 1. Autenticação e Cadastro
*   **Login (`/`)**: Tela inicial para acesso. Utiliza autenticação via Token JWT conectado ao backend.
    *   Redireciona para o **Dashboard** se o usuário já tiver configurado sua granja.
    *   Redireciona para a tela de **Boas-vindas** (`/welcome`) no primeiro acesso.
*   **Cadastro (`/register`)**: Formulário para novos produtores.
    *   Campos: Nome, Email, Senha, Endereço.
    *   Feedback visual com **Modal de Sucesso** personalizado antes de redirecionar para o login.
*   **Recuperação de Senha (`/forgot-password`)**: *(Em desenvolvimento)* Fluxo para redefinição de senha.

### 2. Onboarding (Primeiro Acesso)
*   **Boas-vindas (`/welcome`)**:
    *   Exibida apenas no primeiro login.
    *   Solicita o **Nome do Aviário** (ex: "Granja feliz").
    *   Salva essa preferência localmente e prepara o ambiente para o produtor.

### 3. Painel Principal (Dashboard)
*   **Dashboard (`/dashboard`)**: O centro de controle do aplicativo.
    *   **Cabeçalho**: Exibe o nome do aviário configurado e ícone de perfil.
    *   **Notificações**: Ícone de sino com indicador de alertas (ex: alta mortalidade, início de postura). Ao clicar, abre um modal com a lista de alertas.
    *   **Lista de Lotes**: Exibe os lotes cadastrados, buscando dados reais do backend (`GET /api/Lot`). Mostra ID do lote, data de alojamento, raça e quantidade de aves.
    *   **Ação Rápida**: Botão flutuante **(+)** para cadastrar novos lotes.

### 4. Gestão de Lotes
*   **Cadastrar Lote (`/create-batch`)**:
    *   Formulário para registrar um novo lote de aves.
    *   Permite adicionar múltiplas **linhagens** (Raça e Quantidade) dinamicamente.
    *   Envia os dados para a API (`POST /api/Lot`), criando automaticamente a granja e as raças se necessário.

### 5. Perfil do Usuário
*   **Visualizar Perfil (`/profile`)**:
    *   Exibe um "Card" com as informações do produtor: Email, CPF, Estado, Cidade e Telefone.
    *   Os dados são carregados em tempo real do backend (`GET /api/User/me`).
    *   Botão de **Logout** para sair do aplicativo.
*   **Editar Perfil (`/edit-profile`)**:
    *   Permite atualizar os dados cadastrais.
    *   Carrega os dados atuais nos campos ao abrir.
    *   Salva as alterações no servidor (`PUT /api/User/me`).

## 🛠️ Tecnologias Utilizadas

*   **Framework**: [Expo](https://expo.dev/) (React Native)
*   **Linguagem**: TypeScript
*   **Estilização**: [NativeWind](https://www.nativewind.dev/) (TailwindCSS para React Native)
*   **Navegação**: Expo Router (navegação baseada em arquivos)
*   **Armazenamento Local**: Async Storage (para Tokens e preferências)
*   **Ícones**: Expo Vector Icons (Ionicons, FontAwesome, MaterialCommunityIcons)
*   **Gráficos/SVG**: React Native SVG

## 🚀 Como Rodar o Projeto

1.  **Instale as dependências**:
    ```bash
    npm install
    ```

2.  **Configure a API**:
    *   Verifique o arquivo `constants/Api.ts`.
    *   Certifique-se de que o `API_URL` aponta para o IP da sua máquina onde o Backend está rodando (ex: `http://192.168.x.x:5015` ou `http://localhost:5015` se for emulador).

3.  **Inicie o servidor de desenvolvimento**:
    ```bash
    npx expo start
    ```

4.  **Teste**:
    *   Use o aplicativo **Expo Go** no seu celular (escaneie o QR Code).
    *   Ou pressione `w` para rodar no navegador (Web).
    *   Ou pressione `a` para rodar no emulador Android.

## 📂 Estrutura de Pastas

*   `app/`: Telas e rotas da aplicação (file-based routing).
*   `components/`: Componentes reutilizáveis (Botões, Modais, Inputs).
*   `constants/`: Constantes globais (Configuração da API, Cores).
*   `assets/`: Imagens e fontes.

---
*Desenvolvido para modernizar a avicultura.* 🐥
