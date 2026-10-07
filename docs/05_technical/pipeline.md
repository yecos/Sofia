# SOFIA — Pipeline Técnico y Creativo v0.1

## Stack inicial

- Unity 6
- URP
- Cinemachine
- Input System
- Shader Graph
- 2D Animation
- Timeline
- Git + GitHub
- Git LFS para binarios de arte y audio
- FMOD/Wwise a validar

## Principio de producción

Separar exploración, aprobación y producción final.

No convertir concept art en asset final hasta que:
- la dirección esté aprobada;
- el layout funcione;
- el gameplay esté validado.

## Flujo por escena

### 1. Narrative brief
Qué debe sentir el jugador y qué cambia al terminar la escena.

### 2. Gameplay blockout
Geometría simple y navegación.

### 3. Camera pass
Escala, framing, transiciones, límites.

### 4. Art concept
Thumbnails → color key → key art.

### 5. Art production
Capas, personajes, props, FX.

### 6. Animation pass
Blocking → spline/cleanup → secondary → polish.

### 7. Integration
Materiales, luces, parallax, partículas, triggers.

### 8. Audio
Ambiente, SFX, música adaptativa.

### 9. Polish
Microtiming, partículas, cámara, feedback y transiciones.

### 10. QA
Colisiones, input, legibilidad, performance y regresión.

## Flujo de assets

```
Concept
  ↓
Approval
  ↓
Layered source
  ↓
Export preset
  ↓
Unity import
  ↓
Material/Shader
  ↓
Animation
  ↓
Prefab
  ↓
Scene
  ↓
Polish
```

## Convenciones iniciales

### Unity
- Scenes: `SCN_`
- Prefabs: `PF_`
- Sprites: `SPR_`
- Materials: `MAT_`
- Shaders: `SH_`
- Animations: `AN_`
- Audio: `AUD_`
- VFX: `VFX_`

### Carpetas de producción sugeridas

```
Assets/Sofia/
  Art/
  Animation/
  Audio/
  Characters/
  Environments/
  Materials/
  Prefabs/
  Scenes/
  Scripts/
  Shaders/
  VFX/
```

## Git

- `main`: estable.
- `develop`: integración.
- feature branches: trabajo específico.
- commits pequeños y descriptivos.
- arte pesado mediante Git LFS.
- evitar subir Library/, Temp/, builds o caches.

## Hermes / automatización futura

Hermes puede apoyar:
- generación de scaffolding;
- validación de naming;
- ejecución de tests;
- builds;
- captura de logs;
- screenshot regression;
- revisión de assets faltantes;
- generación de reportes;
- automatización de tareas repetitivas.

Hermes no debe tomar decisiones artísticas finales sin revisión humana.
