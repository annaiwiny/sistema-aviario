#ifndef SECRETS_H
#define SECRETS_H

// ---------------------------------------------------------------------------
// Modelo. Copie este arquivo para secrets.h e preencha:
//
//   cp src/secrets.example.h src/secrets.h            (Linux/macOS)
//   Copy-Item src\secrets.example.h src\secrets.h     (PowerShell)
//
// src/secrets.h esta no .gitignore e nao deve ser versionado.
// ---------------------------------------------------------------------------

// Rede. "Wokwi-GUEST" com senha vazia e a rede virtual do simulador.
// Em hardware real, coloque aqui o SSID e a senha do Wi-Fi da granja.
#define WIFI_SSID "Wokwi-GUEST"
#define WIFI_PASSWORD ""

// Endpoint que recebe as leituras.
//   Simulacao no Wokwi ...: http://host.wokwi.internal:5000/api/sensors/readings
//   Producao ............: https://aviario.seudominio.com/api/sensors/readings
#define API_URL "http://host.wokwi.internal:5000/api/sensors/readings"

// Precisa ser exatamente o mesmo valor de ESP32_SECRET_KEY no .env do backend.
#define SECRET_KEY "cole-aqui-o-mesmo-valor-de-ESP32_SECRET_KEY"

#endif
