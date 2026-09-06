#ifndef WIFI_H
#define WIFI_H

#include "../models.h"

void wifi_init();

bool enviarLeituras(const SensorData& data);

#endif
