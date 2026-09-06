FROM node:20-alpine AS builder

WORKDIR /app

COPY package*.json ./

# npm ci respeita o package-lock.json: build reproduzivel, sem upgrade silencioso
# de dependencia entre a maquina de dev e o servidor.
RUN npm ci

COPY . .

# URL da API embutida no bundle em tempo de build.
# Vazio = mesma origem (proxy /api do nginx abaixo).
ARG EXPO_PUBLIC_API_URL=""
ENV EXPO_PUBLIC_API_URL=$EXPO_PUBLIC_API_URL

RUN npx expo export --platform web

FROM nginx:alpine

RUN rm /etc/nginx/conf.d/default.conf

COPY nginx.conf /etc/nginx/conf.d/default.conf

COPY --from=builder /app/dist /usr/share/nginx/html

EXPOSE 80

CMD ["nginx", "-g", "daemon off;"]
