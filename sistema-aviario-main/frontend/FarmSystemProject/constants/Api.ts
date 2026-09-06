/**
 * URL base da API.
 *
 * Vazio (padrao) = mesma origem: o nginx do container do frontend faz proxy
 * reverso de /api para o backend, entao o navegador nunca fala direto com a API
 * e nao ha requisicao cross-origin.
 *
 * Para servir a API em outro dominio, defina EXPO_PUBLIC_API_URL no .env antes
 * do build (a variavel e embutida no bundle em tempo de build) e adicione a
 * origem do frontend em CORS_ALLOWED_ORIGIN_0.
 */
export const API_URL = (process.env.EXPO_PUBLIC_API_URL ?? '').replace(/\/+$/, '');
