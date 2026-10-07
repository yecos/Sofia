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
- Espacio C: 445.5 GB total; 59.49 GB libres
- Observación: el runbook recomienda 100 GB libres; el equipo quedó por debajo de ese umbral.

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

Pendientes por incompatibilidad con Unity 6000.6:

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

Pendientes por fallo DNS de `download.packages.unity.com` durante la instalación:

- 2D Animation
- 2D SpriteShape
- 2D PSD Importer

`2D Sprite` no apareció como paquete independiente en el registro consultado; la funcionalidad base de sprites está disponible en los módulos del editor.

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

Nota MCP: `screenshot-game-view` fue invocado y devolvió `Game View render texture is not available. Ensure the Game View window is open and visible.` El canal desktop disponible no permitió abrir/mostrar esa ventana. La captura entregada es una captura real renderizada por la cámara de Unity como evidencia alternativa; no se afirma que sea una captura obtenida por esa llamada MCP específica.

## Consola y errores

- Código SOFIA y escena: compilan correctamente.
- Tests: sin errores ni excepciones de test.
- Warnings revisados: avisos de firma del Licensing Client y `NameResolutionFailure` al intentar conectar el MCP cloud durante algunos arranques; no provocaron fallos de compilación ni de tests.
- La consola interactiva no puede certificarse como cero warnings porque el canal MCP cloud tuvo reconexiones y errores de red durante Play Mode.

## Errores encontrados y solucionados

1. Unity Hub CLI no localizaba correctamente la instalación MSIX; se usó el ejecutable exacto de Unity 6000.6.4f1.
2. El plugin base se fijó a 0.93.2 porque OpenUPM no resolvía `latest`.
3. Las extensiones MCP usan versionado independiente; se corrigieron a sus versiones reales del catálogo.
4. Animation y ProBuilder no compilan en Unity 6000.6 por `GetInstanceID()` obsoleto; se retiraron para preservar compilación.
5. Se añadió assembly separado para EditMode y PlayMode tests.
6. Se registró la escena en Build Settings.
7. Se creó URP asset y se asignó a Graphics/Quality.
8. Se cambió el shader de proxies para evitar render magenta.

## Git y pendientes

- Git LFS inicializado y `.gitattributes` activo.
- `Library/`, `Temp/`, `Logs/`, `UserSettings/`, `Obj/` y builds permanecen ignorados y no deben subirse.
- Pendientes manuales/bloqueos: espacio libre inferior a 100 GB; paquetes 2D bloqueados por DNS; Animation/ProBuilder MCP incompatibles con esta versión; captura específica Game View MCP requiere una ventana visible; FMOD Studio requiere descarga/login manual.
- SHA del commit de bootstrap: se registra después del commit inicial en la revisión de la rama.

## Estado final honesto

`SOFIA_PC_READY = FALSE`

No se marca READY porque no se cumplen simultáneamente todos los criterios solicitados.
