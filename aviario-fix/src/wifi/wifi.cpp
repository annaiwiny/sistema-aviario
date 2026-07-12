#include "wifi.h"

#include "../config.h"

#include <WiFi.h>
#include <WiFiClientSecure.h>
#include <HTTPClient.h>

#include <ArduinoJson.h>

void wifi_init() {
  WiFi.mode(WIFI_STA);
  WiFi.begin(WIFI_SSID, WIFI_PASSWORD);

  log_i("Conectando ao Wi-Fi...");

  unsigned long start = millis();
  const unsigned long timeout = 15000;

  while (WiFi.status() != WL_CONNECTED && millis() - start < timeout) {
    delay(500);
  }

  if (WiFi.status() == WL_CONNECTED) {
    log_i("Wi-Fi conectado. IP: %s", WiFi.localIP().toString().c_str());
    log_i("Endereço MAC: %s", WiFi.macAddress().c_str());
  } else {
    log_e("Falha ao conectar no Wi-Fi.");
  }
}

bool enviarLeituras(const SensorData &data)
{
  if (WiFi.status() != WL_CONNECTED)
  {
    log_e("Erro: Wi-Fi desconectado.");
    return false;
  }

  HTTPClient http;
  WiFiClientSecure secureClient;
  WiFiClient plainClient;

  String url = API_URL;
  bool ok;

  if (url.startsWith("https://"))
  {
    // Pula a validacao do certificado - aceitavel para testes/simulacao,
    // mas para producao o ideal e configurar o certificado raiz com setCACert().
    secureClient.setInsecure();
    ok = http.begin(secureClient, url);
  }
  else
  {
    ok = http.begin(plainClient, url);
  }

  if (!ok)
  {
    log_e("Erro ao inicializar HTTPClient.");
    return false;
  }

  http.addHeader("Content-Type", "application/json");
  http.addHeader("X-Secret-Key", SECRET_KEY);

  JsonDocument doc;
  doc["macAddress"] = WiFi.macAddress();
  JsonArray readings = doc["readings"].to<JsonArray>();

  if (data.temperature.valid)
  {
    JsonObject temp = readings.add<JsonObject>();
    temp["type"] = static_cast<int>(data.temperature.type);
    temp["value"] = data.temperature.value;
  }

  if (data.humidity.valid)
  {
    JsonObject hum = readings.add<JsonObject>();
    hum["type"] = static_cast<int>(data.humidity.type);
    hum["value"] = data.humidity.value;
  }

  if (data.waterLevel.valid)
  {
    JsonObject water = readings.add<JsonObject>();
    water["type"] = static_cast<int>(data.waterLevel.type);
    water["value"] = data.waterLevel.value;
  }

  if (readings.size() == 0)
  {
    http.end();
    return false;
  }

  String body;
  serializeJson(doc, body);

  Serial.println(body);
  int responseCode = http.POST(body);

  //
  Serial.printf("Código HTTP: %d\n", responseCode);

if (responseCode > 0) {
    Serial.println(http.getString());
} else {
    Serial.printf("Erro: %s\n", http.errorToString(responseCode).c_str());
}

  //

  http.end();

  return (responseCode >= 200 && responseCode < 300);
}