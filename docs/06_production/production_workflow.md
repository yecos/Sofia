# SOFIA — Flujo de Producción End-to-End

## Fuentes de verdad

### GitHub
Código, documentación y versiones.

### Unity
Estado ejecutable del juego.

### Archivos fuente de arte
Ilustración y rigs originales.

### DAW / FMOD
Fuentes musicales e implementación de audio.

### GDD
Intención y reglas del diseño.

## Unidad de trabajo

La unidad mínima no será “hacer una escena bonita”.

Será un **Beat Jugable**.

Cada beat tiene:
- emoción;
- objetivo;
- mecánica;
- entrada;
- salida;
- camera beat;
- arte;
- animación;
- audio;
- validación.

## Flujo

### 1. Diseño
Crear beat.

### 2. Greybox
Unity + MCP.

### 3. Playtest temprano
Sin arte final.

### 4. Camera pass
Aprobar composición.

### 5. Visual Development
Keyframe + palette.

### 6. Character/Environment production
Crear assets.

### 7. Animation
Blocking → integrate → polish.

### 8. Audio
Music brief → stems → FMOD.

### 9. Integration
Shaders + VFX + camera + audio.

### 10. Automated QA
MCP:
- compile;
- console;
- tests;
- profiler;
- screenshot.

### 11. Creative QA
Revisión humana/ChatGPT:
- emoción;
- lectura;
- belleza;
- pacing;
- acting.

### 12. Commit/Merge
Solo después de gate.

## Kanban recomendado

Estados:
- Idea
- Brief
- Blockout
- Playable
- Art Pass
- Animation Pass
- Audio Pass
- Polish
- QA
- Approved

## Definition of Ready

Una tarea entra a producción si tiene:
- brief;
- referencia;
- criterio de aceptación;
- responsable;
- dependencias.

## Definition of Done

- funciona;
- compila;
- se ve correcto;
- se escucha correcto;
- animación aprobada;
- performance aceptable;
- screenshot/video final;
- documentación actualizada.

## Ritmo de desarrollo

Trabajar en pequeños slices verticales.

No hacer:
- todos los niveles primero;
- luego todo el arte;
- luego todas las animaciones.

Hacer:
```
Beat completo
→ aprender
→ mejorar pipeline
→ siguiente Beat
```

## Vertical Slice

VS01 es el laboratorio para fijar:
- controller;
- cámara;
- estilo;
- shaders;
- Hele;
- animation rig;
- music system;
- FMOD;
- MCP tools;
- QA.

No comenzar producción masiva antes de cerrar VS01.

## Uso correcto de IA

### IA excelente para
- variantes;
- análisis;
- scripting;
- greybox;
- documentación;
- QA;
- refactors;
- tests;
- screenshots/review;
- tooling.

### IA con revisión humana obligatoria
- concept art;
- música;
- acting;
- composición;
- narrativa;
- ritmo.

### IA no decide sola
- arte final;
- emoción final;
- diseño de personajes;
- edición musical final;
- acting final.
