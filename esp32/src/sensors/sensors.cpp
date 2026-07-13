#include "sensors.h"

#include "dht22.h"
#include "loadcell.h"
#include <math.h>

void sensors_init() {
    dht22_init();
    loadcell_init();
}

SensorData sensors_read() {
  SensorData data;

  data.temperature.type = SensorType::Temperature;
  data.temperature.value = dht22_readTemperature();
  data.temperature.valid = !isnan(data.temperature.value);

  data.humidity.type = SensorType::Humidity;
  data.humidity.value = dht22_readHumidity();
  data.humidity.valid = !isnan(data.humidity.value);

  data.waterLevel.type = SensorType::WaterLevel;

  float rawLiters = loadcell_readLiters();
  const float MAX_VALUE = 0.921053;
  
  // Calcula porcentagem do nível de água
  float waterPercentage = (rawLiters / MAX_VALUE) * 100.0f;
  
  // Evita cenários impossíveis
  if (waterPercentage < 0.0f) waterPercentage = 0.0f;
  if (waterPercentage > 100.0f) waterPercentage = 100.0f;

  data.waterLevel.value = waterPercentage;
  data.waterLevel.valid = true;

  return data;
}
