# SOFIA — Runbook de preparación del PC con Hermes

## Objetivo

Dejar un PC Windows listo para comenzar el desarrollo real de SOFIA con:

- Unity 6.6 fijado a 6000.6.4f1.
- Proyecto Unity dentro del repo.
- Unity MCP de IvanMurzak instalado y operativo.
- Herramientas MCP para Animation, Cinemachine, Input System, Particle System, ProBuilder, Splines, Tilemap y Timeline.
- Git + Git LFS.
- Node.js LTS.
- Blender, Krita, FFmpeg y editor de código.
- Base preparada para FMOD/Reaper.
- Carpetas, serialización y flujo de ramas listos.
- Smoke test de creación de escena + Play Mode + screenshot + consola limpia.

## Regla de ruta

El proyecto Unity NO puede estar en una ruta con espacios para Unity-MCP.

Usar:

```
C:\Dev\Sofia
C:\Dev\Sofia\game\SofiaUnityProject
```

No usar:

```
C:\Users\Mi Usuario\Documents\Sofia Game
```

## 1. Acciones manuales inevitables

Hermes puede automatizar casi todo, pero debe pedir intervención del usuario si aparece:

- inicio de sesión/licencia de Unity Hub;
- autenticación de GitHub si `gh` no está conectado;
- OAuth de `unity-mcp-cli login`;
- descarga/login de FMOD Studio;
- cualquier elevación UAC que no pueda confirmar por sí mismo.

No almacenar contraseñas ni tokens en el repo.

## 2. Software obligatorio

### Core

- Git
- Git LFS
- GitHub CLI
- Node.js LTS
- Unity Hub
- Unity 6.6 6000.6.4f1
- Visual Studio Code
- Python 3.13
- FFmpeg
- 7-Zip

### Arte

- Blender
- Krita

### Audio

Instalar o dejar preparado:
- REAPER
- FMOD Studio 2.03.x
- FMOD for Unity con la misma major/minor de FMOD Studio

### Opcional, no bloquear inicio

- Spine
- Toon Boom Harmony
- Adobe Photoshop
- DaVinci Resolve
- OBS Studio

## 3. Verificación inicial

Ejecutar y registrar:

```powershell
Get-ComputerInfo | Select-Object WindowsProductName, WindowsVersion, OsBuildNumber
Get-CimInstance Win32_Processor | Select-Object Name
Get-CimInstance Win32_VideoController | Select-Object Name, DriverVersion
Get-CimInstance Win32_ComputerSystem | Select-Object TotalPhysicalMemory
Get-PSDrive -PSProvider FileSystem
```

Comprobar al menos 100 GB libres en la unidad elegida para desarrollo.

## 4. Instalar herramientas base

Usar el script:

```
tools/bootstrap-sofia-windows.ps1
```

Después cerrar/reabrir la terminal para refrescar PATH.

Verificar:

```powershell
git --version
git lfs version
gh --version
node --version
npm --version
python --version
ffmpeg -version
code --version
```

## 5. Git

Configurar si falta:

```powershell
git config --global core.autocrlf true
git config --global init.defaultBranch main
git lfs install
```

Si GitHub CLI no está autenticado:

```powershell
gh auth status
gh auth login
```

## 6. Clonar SOFIA

```powershell
New-Item -ItemType Directory -Force C:\Dev | Out-Null
Set-Location C:\Dev

if (-not (Test-Path C:\Dev\Sofia\.git)) {
    git clone https://github.com/yecos/Sofia.git C:\Dev\Sofia
}

Set-Location C:\Dev\Sofia
git fetch --all --prune
git switch main
git pull --ff-only
```

Crear rama de setup:

```powershell
git switch -c setup/unity-bootstrap
```

Si ya existe:

```powershell
git switch setup/unity-bootstrap
```

## 7. Unity-MCP CLI

Unity-MCP CLI requiere Node.js ^20.19 o >=22.12.

Instalar:

```powershell
npm install -g unity-mcp-cli
unity-mcp-cli --version
```

## 8. Unity 6.6

Versión fijada:

```
6000.6.4f1
```

Instalar:

```powershell
unity-mcp-cli install-unity 6000.6.4f1
```

Si Unity Hub requiere sesión/licencia, completar esa autenticación y continuar.

## 9. Crear proyecto Unity

Ruta:

```
C:\Dev\Sofia\game\SofiaUnityProject
```

Si no existe `Packages\manifest.json`:

```powershell
unity-mcp-cli create-project C:\Dev\Sofia\game\SofiaUnityProject --unity 6000.6.4f1
```

No recrear el proyecto si ya existe.

## 10. Instalar Unity MCP

```powershell
unity-mcp-cli install-plugin C:\Dev\Sofia\game\SofiaUnityProject --with-server
```

Instalar extensiones:

```powershell
unity-mcp-cli install-extension Animation C:\Dev\Sofia\game\SofiaUnityProject
unity-mcp-cli install-extension Cinemachine C:\Dev\Sofia\game\SofiaUnityProject
unity-mcp-cli install-extension "Input System" C:\Dev\Sofia\game\SofiaUnityProject
unity-mcp-cli install-extension "Particle System" C:\Dev\Sofia\game\SofiaUnityProject
unity-mcp-cli install-extension ProBuilder C:\Dev\Sofia\game\SofiaUnityProject
unity-mcp-cli install-extension Splines C:\Dev\Sofia\game\SofiaUnityProject
unity-mcp-cli install-extension Tilemap C:\Dev\Sofia\game\SofiaUnityProject
unity-mcp-cli install-extension Timeline C:\Dev\Sofia\game\SofiaUnityProject
```

Antes de instalar, Hermes puede ejecutar:

```powershell
unity-mcp-cli install-extension --list
```

Si el nombre de catálogo cambia, usar el nombre exacto listado.

## 11. Login de Unity MCP

```powershell
unity-mcp-cli login
```

Esto usa OAuth device flow.

Completar autorización una vez y continuar.

## 12. Abrir Unity y esperar MCP

```powershell
unity-mcp-cli open C:\Dev\Sofia\game\SofiaUnityProject --unity 6000.6.4f1
unity-mcp-cli wait-for-ready C:\Dev\Sofia\game\SofiaUnityProject
unity-mcp-cli status C:\Dev\Sofia\game\SofiaUnityProject
```

Unity-MCP descarga/arranca automáticamente su servidor local.

## 13. Paquetes Unity requeridos por SOFIA

Usar Package Manager/MCP y dejar resueltas las versiones compatibles con 6000.6.4f1.

Paquetes objetivo:

- Universal Render Pipeline
- 2D Animation
- 2D Sprite
- 2D SpriteShape
- 2D PSD Importer
- Input System
- Cinemachine
- Timeline
- Test Framework
- Shader Graph / URP dependencies

No fijar versiones arbitrarias si Unity ofrece versiones verified/compatible para 6000.6.

## 14. Configuración del proyecto

Configurar mediante MCP o Editor API:

### Version Control
- Mode: Visible Meta Files

### Asset Serialization
- Force Text

### Color Space
- Linear

### Target frame rate
- 60 fps como objetivo inicial de gameplay.

### Render
- URP.
- Crear URP Asset específico de SOFIA.
- Crear Renderer apropiado para mezcla 2D/2.5D.
- Mantener postprocesado moderado.
- Evitar Bloom exagerado.

### Input
Crear Input Actions base:

```
Player/Move
Player/Jump
Player/Interact
Player/HeleAction
Player/Pause
```

## 15. Estructura Unity

Crear:

```
Assets/Sofia/
  Art/
    Characters/
    Environments/
    Props/
    UI/
  Animation/
    Father/
    Hele/
    Sofi/
  Audio/
  Characters/
  Environments/
  Materials/
  Prefabs/
    Characters/
    Environment/
    Gameplay/
  Scenes/
    Bootstrap/
    Dev/
    VerticalSlice/
  Scripts/
    Core/
    Character/
    Camera/
    Gameplay/
    Hele/
    Narrative/
    Audio/
    Editor/
    Tests/
  Shaders/
  VFX/
  Settings/
```

Crear asmdefs por dominios solo cuando existan suficientes scripts para justificarlo; no sobrearquitectar el día 1.

## 16. Naming

- Scene: `SCN_`
- Prefab: `PF_`
- Sprite: `SPR_`
- Texture: `TEX_`
- Material: `MAT_`
- Shader: `SH_`
- Animation: `AN_`
- Animation Controller: `AC_`
- Audio: `AUD_`
- VFX: `VFX_`
- ScriptableObject: `SO_`

## 17. Git LFS

El repo contiene `.gitattributes`.

Ejecutar:

```powershell
git lfs install
git lfs track
```

No meter `Library/`, `Temp/`, `Logs/` ni builds.

## 18. Herramientas de arte

### Blender

Usarlo para:
- blockout 3D;
- arquitectura;
- perspectiva;
- geometría de apoyo;
- cámaras de referencia;
- props;
- mesh simple para 2.5D.

### Krita

Usarlo para:
- paintovers;
- fondos;
- texturas pictóricas;
- color keys;
- correcciones frame-by-frame;
- concept art.

No generar assets finales masivamente hasta aprobar Style Bible + VS01.

## 19. Animación

Primer stack:

- Unity 2D Animation;
- Animator;
- Timeline;
- Cinemachine;
- herramientas MCP de Animation.

Regla:
**código mueve al personaje; animación interpreta.**

No usar root motion como locomoción principal.

Primer set del padre:
- idle;
- walk;
- run;
- start/stop;
- turn;
- jump;
- airborne;
- fall;
- soft land;
- hard land;
- reach;
- memory interaction.

Hele:
- hover;
- follow;
- curious;
- playful;
- activate;
- orbit;
- concern;
- rest.

## 20. Audio

### REAPER
DAW recomendado para composición/edición.

### FMOD
Usar FMOD Studio 2.03.x y FMOD for Unity de la misma rama.

Crear fuera de `Assets`:

```
C:\Dev\Sofia\audio\fmod\SofiaAudio.fspro
```

Estructura conceptual:

```
Father
Sofi
Hele
Memory
Environment
UI
```

Parámetros futuros:

```
Emotion
SofiDistance
HeleEnergy
MemoryIntensity
Danger
Chapter
```

No bloquear el primer smoke test por FMOD. Integrarlo después de que movimiento + cámara + MCP estén operativos.

## 21. Smoke Test obligatorio

Hermes debe demostrar:

### A. MCP operativo
- listar herramientas;
- consultar estado;
- no hay error de conexión.

### B. Crear escena

Crear:

```
SCN_MCP_SmokeTest
```

Contenido:
- cámara;
- suelo;
- Player proxy;
- Hele proxy amarilla;
- 3 plataformas;
- luz/ambiente simple.

### C. Gameplay mínimo
Player puede:
- moverse;
- saltar.

Hele:
- sigue al Player con una distancia suave.

### D. Play Mode
Entrar en Play Mode y dejar correr al menos 10 segundos.

### E. Console
- 0 errores;
- 0 exceptions;
- warnings revisados.

### F. Captura
Tomar screenshot de Game View.

Guardar evidencia en:

```
docs/evidence/setup/
```

### G. Tests
Crear al menos:
- EditMode smoke test;
- PlayMode smoke test básico.

### H. Git
Ejecutar:

```powershell
git status
git add .
git commit -m "chore: bootstrap Unity project and MCP"
git push -u origin setup/unity-bootstrap
```

No fusionar a main todavía.

## 22. Criterio final de READY

Hermes solo puede reportar `SOFIA_PC_READY = TRUE` si cumple todos:

- Unity 6000.6.4f1 instalado.
- Repo clonado.
- Proyecto abre.
- MCP conectado.
- Plugins/extensiones instalados.
- Unity compila.
- Play Mode entra.
- Input funciona.
- Screenshot generado.
- Consola sin errores.
- Smoke tests pasan.
- Rama de setup pusheada.
- Reporte final escrito.

## 23. Reporte final de Hermes

Crear:

```
docs/evidence/setup/HERMES_SETUP_REPORT.md
```

Debe incluir:

- fecha;
- PC;
- GPU/driver;
- RAM;
- espacio libre;
- versiones instaladas;
- Unity exacto;
- Node;
- unity-mcp-cli;
- plugin MCP;
- extensiones;
- estado MCP;
- paquetes Unity;
- tests;
- errores encontrados;
- correcciones realizadas;
- screenshot paths;
- commit SHA;
- pendientes manuales.

## 24. Prohibiciones

Hermes NO debe:

- borrar el repo;
- force-push;
- trabajar en main;
- resetear cambios ajenos;
- instalar dos Unity MCP distintos;
- meter secretos al repo;
- meter Library o builds;
- cambiar de Unity 6.6 a 6000.6 sin decisión documentada;
- instalar Spine/Toon Boom de pago sin aprobación;
- generar assets finales en masa antes de VS01.
