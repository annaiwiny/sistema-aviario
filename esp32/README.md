# esp32-aviario

## O que precisa preencher antes de compilar
Em `src/config.h`:
- `WIFI_SSID` / `WIFI_PASSWORD`: nome e senha da sua rede Wi-Fi.
- `API_URL`: endereço do servidor que vai receber as leituras (ex: `http://192.168.0.10:3000/api/leituras`).


## Simulando no Wokwi
### VS Code
1. Instale a extensão **Wokwi for VS Code** e ative a licença gratuita.
2. Compile o projeto normalmente (`pio run`), pra gerar `.pio/build/esp32dev/firmware.bin` e `.elf`.
3. Abra `diagram.json` no VS Code (a extensão renderiza o circuito) e clique em "Start Simulator".



### Configurações específicas pra simulação
No `config.h`, pra rodar no Wokwi use:
```cpp
#define WIFI_SSID "Wokwi-GUEST"
#define WIFI_PASSWORD ""
#define API_URL "http://SEU_ENDPOINT_PUBLICO_AQUI"
```
A rede `Wokwi-GUEST` dá acesso real à internet através do simulador — então pra testar o envio HTTP de verdade, aponte `API_URL` pra um endpoint público (ex.: um teste rápido em [webhook.site](https://webhook.site), que gera uma URL única pra você ver as requisições chegando em tempo real).

