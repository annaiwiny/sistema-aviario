#ifndef MODELS_H
#define MODELS_H

#include <stdint.h>

enum class SensorType: uint8_t {
    Temperature = 1,
    Humidity = 2,
    WaterLevel = 3
};

struct SensorReading {
    SensorType type;
    float value;
    bool valid;
};

struct SensorData {
    SensorReading temperature;
    SensorReading humidity;
    SensorReading waterLevel;
};

#endif
