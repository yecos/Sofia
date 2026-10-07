# SOFIA — Música y Audio

## Objetivo

La música no será una pista de fondo. Será un sistema narrativo que cambia con el vínculo entre el padre, Sofi y Hele.

## Stack

- DAW principal: a definir; Reaper es una opción sólida para producción.
- Instrumentos virtuales y grabaciones propias.
- FMOD Studio para implementación adaptativa.
- Unity recibe estados y parámetros de gameplay.

## Identidad musical

### Padre
Piano íntimo + registro medio/grave de cuerdas.

### Sofi
Motivo melódico limpio, vulnerable y fácil de recordar.

### Hele
Timbre brillante y cálido:
- celesta;
- piano preparado suave;
- campanas muy delicadas;
- texturas luminosas.

### Familia
El tema final combina motivos del padre, Sofi y Hele.

## Regla central

Crear primero un **leitmotiv reconocible** antes de escribir horas de música.

## Pipeline de composición

### 1. Music Brief
Para cada escena:
- emoción;
- duración aproximada;
- intensidad;
- personajes;
- mecánica;
- transición;
- punto narrativo.

### 2. Sketch
Piano/MIDI simple.

### 3. Theme Review
Aprobar melodía y armonía sin producción compleja.

### 4. Arrangement
Instrumentación y textura.

### 5. Stem Design
Exportar capas separadas.

Ejemplo:
- `MUS_Father_Piano`
- `MUS_Sofi_Melody`
- `MUS_Hele_Light`
- `MUS_Strings`
- `MUS_Texture`
- `MUS_Pulse`

### 6. FMOD
Construir evento adaptativo.

### 7. Unity
Gameplay modifica parámetros.

### 8. Playtest
La música se revisa jugando, no escuchando solo el WAV.

## Parámetros FMOD propuestos

### `Emotion`
0–1.

Controla intensidad general.

### `SofiDistance`
0–1.

Introduce o aleja el motivo de Sofi.

### `HeleEnergy`
0–1.

Controla brillo y presencia de Hele.

### `MemoryIntensity`
0–1.

Activa capas armónicas/ambientales de recuerdo.

### `Danger`
0–1.

No es “combate”; representa tensión ambiental.

### `Chapter`
Discrete/labelled:
- Silence
- Joy
- Sorrow
- Distance
- Guilt
- Hele
- Hope
- Sofia

## Ejemplo de comportamiento

### El silencio
Piano incompleto + aire.

### Aparece Hele
Entra un timbre brillante.

### Primer recuerdo
Aparece por primera vez parte del tema de Sofi.

### Alegría
El mismo motivo se expande con cuerdas y movimiento.

### Pena
No escribir un tema totalmente diferente: deformar el mismo material armónico.

### Final
Los motivos dejan de estar separados y forman una sola pieza.

## Audio reactivo

Además de música:
- pasos por superficie;
- tela;
- viento;
- agua;
- partículas;
- arquitectura;
- Hele;
- memoria.

El mundo debe tener silencio real. No llenar todo con sonido.

## Especificación de entrega

Masters/stems:
- WAV;
- 48 kHz;
- 24-bit;
- loops con puntos exactos;
- head/tail documentados;
- loudness coherente.

## IA en música

Usar IA para:
- referencias;
- exploración de instrumentación;
- estructura;
- análisis;
- variaciones de briefs;
- ideas MIDI/armónicas.

No basar la banda sonora final en generación automática sin:
- validar derechos;
- revisar consistencia;
- recrear/editar musicalmente el material;
- mantener una identidad sonora propia.

## Versionado

```
audio/
  source/
    daw/
    recordings/
    midi/
  exports/
    stems/
    sfx/
  fmod/
  references/
```

Archivos binarios pesados mediante Git LFS.

## Gate de aprobación

1. ¿El motivo funciona en piano solo?
2. ¿Representa correctamente al personaje/emoción?
3. ¿Puede transformarse entre capítulos?
4. ¿Loop/transición es invisible?
5. ¿FMOD responde sin cortes?
6. ¿La música deja respirar al juego?
