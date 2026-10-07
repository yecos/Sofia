# SOFIA — Hermes Setup Report

- Fecha: 2026-10-07T10:24:53-05:00
- Repositorio: `https://github.com/yecos/Sofia`
- Rama: `setup/unity-bootstrap`
- Ruta repo: `C:\Dev\Sofia`
- Ruta proyecto: `C:\Dev\Sofia\game\SofiaUnityProject`

## Hardware

- Windows: Windows 11 Home 10.0.26200, 64-bit
- CPU: Intel(R) Core(TM) i7-10700 CPU @ 2.90GHz; 8 cores / 16 logical processors
- GPU: NVIDIA GeForce RTX 3060
- Driver GPU: `32.0.16.1062`
- RAM: 15.89 GB total; 5.16 GB libre en la inspección inicial
- Espacio C: 445.5 GB total; 38.98 GB libres al finalizar
- Excepción: el usuario acepta explícitamente continuar con menos de 100 GB libres.

## Herramientas

- Git: `2.55.0.windows.2`
- Git LFS: `3.7.1`
- GitHub CLI: `2.102.0`
- Node: `v22.23.3`
- npm: `10.9.9`
- Python: `3.11.16` disponible en el entorno Hermes; el bootstrap pidió Python 3.13 pero WinGet dejó Python 3.11 instalado.
- FFmpeg: `8.1.2`
- VS Code: `1.127.0`
- Blender: `5.2.2`
- Krita: `5.3.4.0`
- REAPER: `7.82`
- FMOD Studio: pendiente manual; no bloquea este bootstrap.

## Unity y MCP

- Unity usado: `6000.6.4f1` (`12bfff696524`)
- Ejecutable: `C:\Program Files\Unity\Hub\Editor\6000.6.4f1\Editor\Unity.exe`
- `unity-mcp-cli`: `0.93.2`
- GameDev MCP Server: `win-x64 v9.2.9`, checksum verificado
- Plugin MCP: `com.ivanmurzak.unity.mcp 0.93.2`
- Login OAuth: autorizado; credencial guardada fuera del repo en `C:\Users\yecos\.ai-game-dev\credentials.json`
- Estado MCP: `ping`, `assets-find`, `scene-open` y `editor-application-get-state` respondieron mediante el MCP cloud.
- MCP local: `localhost:27825` no respondió; la sesión usada fue MCP cloud.

### Extensiones MCP

Instaladas y resueltas:

- Cinemachine `1.0.9`
- InputSystem `1.0.9`
- ParticleSystem `1.2.9`
- Splines `1.0.9`
- Tilemap `1.0.9`
- Timeline `1.0.9`

No instaladas por incompatibilidad de API con Unity 6000.6.4f1:

- Animation `1.2.9`: errores `CS0619` por uso de `Object.GetInstanceID()` obsoleto.
- ProBuilder `1.2.9`: mismos errores `CS0619`.

Se retiraron del manifest para mantener compilación limpia; no se modificó `Library/PackageCache`.

## Paquetes Unity

Resueltos por Unity 6000.6.4f1:

- Universal Render Pipeline `17.6.0`
- Input System `1.20.0`
- Cinemachine `6.6.0`
- Timeline `6.6.0` resuelto por el editor
- Test Framework `1.8.0` resuelto por el editor
- Módulos de física 2D, partículas y Tilemap incluidos en el editor

Descargadas durante la prueba, pero retiradas porque rompen la compilación de Unity 6000.6.4f1:

- 2D Animation `9.2.2`: 17 errores `CS0619`.
- 2D SpriteShape `9.1.1`: 3 errores `CS0619`.
- 2D PSD Importer `9.1.3`: dependiente del stack 2D incompatible.

`2D Sprite` aparece como builtin `com.unity.2d.sprite 1.0.0`.

## Configuración del proyecto

- Asset Serialization: Force Text
- Version Control: Visible Meta Files
- Color Space: Linear
- Target frame rate: 60 FPS
- URP asset creado: `Assets/Sofia/Settings/SofiaURP.asset`
- Estructura `Assets/Sofia/` creada según el runbook
- Escena: `Assets/Sofia/Scenes/Dev/SCN_MCP_SmokeTest.unity`
- Build Settings: la escena de smoke está registrada en índice 0

## Smoke test

La escena contiene:

- Main Camera ortográfica con seguimiento suave
- Ground
- Platform_A, Platform_B, Platform_C
- Player proxy azul con Rigidbody2D, BoxCollider2D, movimiento horizontal y salto con Space/W/Up
- Hele proxy amarilla con seguimiento suavizado y hover vertical
- Materiales proxy compatibles con el render actual

Resultados:

- Compilación Unity/builder: PASS, exit code 0
- EditMode: PASS, 2/2 tests
- PlayMode: PASS, 1/1 test
- Duración PlayMode smoke: 10.27 s
- El test verifica escena, movimiento del Player, distancia de seguimiento de Hele y permanencia de 10 segundos.
- La ejecución batch no reportó fallos de test.

## Evidencia

- Captura real de cámara de la escena: `docs/evidence/setup/mcp-smoke-test.png`
- Tamaño: 1280x720, 12,152 bytes
- Verificación visual: contiene suelo, tres plataformas, Player azul y Hele amarilla.
- XML EditMode: `docs/evidence/setup/editmode-results.xml`
- XML PlayMode: `docs/evidence/setup/playmode-results.xml`
- Logs de pruebas: `docs/evidence/setup/editmode.log`, `docs/evidence/setup/playmode.log`

Nota MCP: `screenshot-game-view` fue invocado y devolvió `Game View render texture is not available. Ensure the Game View window is open and visible.` La captura entregada es una captura real renderizada por la cámara de Unity como evidencia alternativa.

## Consola y errores

- Código SOFIA y escena: compilan correctamente.
- Tests: sin errores ni excepciones de test.
- Warnings revisados: avisos de firma del Licensing Client y reconexiones cloud intermitentes; no provocaron fallos de compilación ni de tests.
- MCP reconnect: 3/3 ciclos completos Play/Edit de 10 segundos con `ping` posterior PASS; un intento aislado falló por red y fue reintentado una vez.

## Errores encontrados y solucionados

1. Unity Hub CLI no localizaba correctamente la instalación MSIX; se usó el ejecutable exacto de Unity 6000.6.4f1.
2. El plugin base se fijó a 0.93.2 porque OpenUPM no resolvía `latest` en el primer intento.
3. Las extensiones MCP usan versionado independiente; se corrigieron a sus versiones reales del catálogo.
4. Animation y ProBuilder MCP no compilan en Unity 6000.6 por `GetInstanceID()` obsoleto; se retiraron para preservar compilación.
5. 2D Animation y SpriteShape oficiales descargan, pero también contienen `GetInstanceID()` obsoleto para esta revisión de Unity; se retiraron para preservar compilación limpia.
6. Se añadió assembly separado para EditMode y PlayMode tests.
7. Se registró la escena en Build Settings.
8. Se creó URP asset y se asignó a Graphics/Quality.
9. Se cambió el shader de proxies para evitar render magenta.

## Git y pendientes

- Git LFS inicializado y `.gitattributes` activo.
- `Library/`, `Temp/`, `Logs/`, `UserSettings/`, `Obj/` y builds permanecen ignorados y no deben subirse.
- Pendientes manuales/bloqueos: paquetes 2D y extensiones Animation/ProBuilder incompatibles con esta revisión de Unity; captura específica Game View MCP requiere una ventana visible; FMOD Studio requiere descarga/login manual.
- DNS: recuperado; `ai-game.dev`, OpenUPM y los registros Unity responden por HTTPS al finalizar.
- Disco: excepción aceptada por el usuario (`DISK_REQUIREMENT = USER_ACCEPTED_EXCEPTION`).
- FMOD: `WAITING_FOR_USER_LOGIN` / descarga oficial manual.
- SHA del commit de bootstrap: `fea3d25405f14fd632532352d1bcd1650b11ce87`.

## Estado final honesto

`SOFIA_PC_READY = FALSE`

No se marca READY porque no se cumplen simultáneamente todos los criterios solicitados.

## VS01 — 2026-10-07
Rama feature/vs01-despertar-90s desde setup/unity-bootstrap. Escena Despertar jugable, 4 pruebas PlayMode y 2 EditMode PASS, cuatro capturas reales y medición de cámara a 60 FPS. Se corrigió el renderer URP vacío del bootstrap. Detalles y limitaciones en ../../07_vertical_slice/vs01_implementation_0_90.md. No se avanza a 2:00+; duración narrativa y feel humano pendientes de revisión.
