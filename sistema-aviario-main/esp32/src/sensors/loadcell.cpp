#include "loadcell.h"
#include "../config.h"

#include <HX711.h>
#include <math.h>

HX711 loadcell;

// 30 amostras já dão uma média estável sem travar o loop por muito tempo
// (1000 amostras podiam levar dezenas de segundos, bloqueando o resto do sistema)
constexpr int SAMPLE_COUNT = 30;
constexpr float SIGMA = 3.0f;
constexpr unsigned long READY_TIMEOUT_MS = 1000;

void loadcell_init() {
  loadcell.begin(HX711_DT, HX711_SCK);
  loadcell.set_scale(CALIBRATION_FACTOR);
  loadcell.tare();
}

float loadcell_readLiters() {
  float readings[SAMPLE_COUNT];
  int count = 0;

  // coleta de amostras, com timeout para não travar caso o HX711 nao responda
  for (int i = 0; i < SAMPLE_COUNT; i++) {
    unsigned long start = millis();
    while (!loadcell.is_ready()) {
      if (millis() - start > READY_TIMEOUT_MS) {
        break;
      }
    }

    if (loadcell.is_ready()) {
      readings[count] = loadcell.get_units();
      count++;
    }
  }

  if (count == 0) {
    return 0.0f;
  }

  // media das amostras
  float mean = 0.0f;
  for (int i = 0; i < count; i++) {
    mean += readings[i];
  }
  mean /= count;

  // desvio padrao das amostras
  float variance = 0.0f;
  for (int i = 0; i < count; i++) {
    variance += pow(readings[i] - mean, 2);
  }
  variance /= count;

  float stddev = sqrt(variance);

  // filtragem de outliers
  float filteredMean = 0.0f;
  int filteredCount = 0;

  for (int i = 0; i < count; i++) {
    if (fabs(readings[i] - mean) <= SIGMA * stddev) {
      filteredMean += readings[i];
      filteredCount++;
    }
  }

  if (filteredCount == 0) {
    return 0.0f;
  }

  float finalWeight = filteredMean / filteredCount;

  if (finalWeight < 0.0f) {
    return 0.0f;
  }

  return finalWeight;
}
