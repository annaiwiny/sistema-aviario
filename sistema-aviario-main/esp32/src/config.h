#ifndef CONFIG_H
#define CONFIG_H

// ---------------------------------------------------------------------------
// Credenciais (Wi-Fi, URL da API, chave secreta) ficam em src/secrets.h,
// que NAO e versionado. Copie src/secrets.example.h para src/secrets.h e
// preencha antes de compilar.
// ---------------------------------------------------------------------------
#if defined(__has_include)
#  if __has_include("secrets.h")
#    include "secrets.h"
#  else
#    error "esp32/src/secrets.h nao encontrado. Copie src/secrets.example.h para src/secrets.h e preencha."
#  endif
#else
#  include "secrets.h"
#endif


// ---- Sensores (nao sao segredo: pinagem e calibracao do hardware) ----
#define DHT22_PIN 4

#define HX711_DT 18
#define HX711_SCK 19

#define CALIBRATION_FACTOR 2280.0f

#endif
