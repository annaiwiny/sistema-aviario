#ifndef CONFIG_H
#define CONFIG_H

#define WIFI_SSID "Wokwi-GUEST"
#define WIFI_PASSWORD ""
#define API_URL "https://172.31.80.1:7172/api/sensors/readings"
#define SECRET_KEY "Aviario_Esp32_Key_SuperSecreta123"


// ---- Sensores ----
#define DHT22_PIN 4

#define HX711_DT 18
#define HX711_SCK 19

#define CALIBRATION_FACTOR 2280.0f

#endif
