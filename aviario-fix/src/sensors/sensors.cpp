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
  data.waterLevel.value = loadcell_readLiters();
  data.waterLevel.valid = true;

  return data;
}
