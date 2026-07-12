#include "dht22.h"
#include "../config.h"

#include <DHT.h>

DHT dht(DHT22_PIN, DHT22);

void dht22_init() {
    dht.begin();
}

float dht22_readTemperature() {
    return dht.readTemperature();
}

float dht22_readHumidity() {
    return dht.readHumidity();
}
