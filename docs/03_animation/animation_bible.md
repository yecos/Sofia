# SOFIA — Biblia de Animación v0.1

## Objetivo

La animación debe ser uno de los rasgos distintivos de SOFIA. No buscamos únicamente fluidez: buscamos intención, acting, peso, claridad y conexión emocional.

## Filosofía

**El movimiento cuenta la historia.**

La postura del padre, la forma de flotar de Hele y la presencia de Sofi deben comunicar incluso con el audio apagado.

## Pipeline recomendado

### Gameplay
Rig 2D de alta calidad para iteración rápida, complementado con deformation, secondary motion y correcciones dibujadas.

### Momentos hero/cinemáticos
Frame-by-frame o passes de animación específicos cuando el acting lo requiera.

### FX
Partículas, trails, shader animation y deformación procedural para potenciar, no reemplazar, el trabajo de animación.

## Herramientas candidatas

- Unity 2D Animation
- Spine
- Toon Boom Harmony
- herramientas frame-by-frame
- Timeline para secuencias
- Cinemachine para coordinación cámara/acting

La combinación definitiva se decide tras pruebas del vertical slice.

## Principios

- poses claras;
- arcos limpios;
- anticipación;
- spacing controlado;
- follow-through;
- overlap;
- cambio de peso;
- respiración;
- microgestos;
- silueta legible;
- timing emocional.

## Padre — set mínimo

- idle neutro;
- idle cansado/emocional;
- walk;
- run;
- start/stop;
- turn;
- jump start;
- airborne;
- fall;
- land soft;
- land hard;
- ledge interaction;
- touch memory;
- reach toward Sofi;
- reaction to Hele;
- sorrow pose;
- recovery;
- final embrace/connection.

## Hele — set mínimo

- neutral hover;
- curious hover;
- playful burst;
- concern;
- sadness;
- call player;
- orbit;
- activate;
- illuminate;
- stretch through gap;
- carry energy;
- protection;
- celebration;
- quiet resting state.

## Sofi — set mínimo

- distant idle;
- turn;
- walk;
- run memory;
- hand reach;
- disappearance;
- close presence;
- final connection.

## Criterios de polish

Una animación no se aprueba solo porque “funciona”.

Debe superar:
1. lectura de silueta;
2. peso;
3. timing;
4. transición in/out;
5. respuesta de input;
6. continuidad con cámara;
7. secondary motion;
8. coherencia con audio;
9. coherencia emocional;
10. revisión a velocidad real.

## Performance target

Separar claramente:
- input responsiveness;
- visual smoothing;
- animation anticipation.

Nunca introducir lag perceptible al control por buscar una animación más bonita.
