#include "Arduino.h"

#include "sensors/sensors.h"
#include "wifi/wifi.h"

void setup() {
  Serial.begin(115200);
  sensors_init();
  wifi_init();
}

void loop() {
  SensorData data = sensors_read();

  bool ok = enviarLeituras(data);

  if (!ok) {
    log_e("Falha ao enviar leituras.");
  }

  delay(30000);
}
