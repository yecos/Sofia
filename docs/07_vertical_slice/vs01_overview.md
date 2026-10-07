# VS01 — El Primer Destello

## Duración
5–10 minutos.

## Objetivo

Demostrar la identidad completa de SOFIA en una pieza pequeña:
- emoción;
- arte;
- movimiento;
- Hele;
- Sofi;
- música;
- cámara;
- puzzle;
- transformación del mundo.

## Beat 1 — Despertar

El padre despierta en una plataforma fría y silenciosa.

### Gameplay
Movimiento básico y cámara.

### Visual
Casi monocromático, azul/gris, niebla.

### Audio
Viento, respiración, ambiente mínimo.

## Beat 2 — Aparición de Hele

Una pequeña luz amarilla aparece y evita acercarse de inmediato. Observa al padre, juega con su atención y finalmente lo guía.

### Objetivo
Enseñar que Hele es un personaje.

## Beat 3 — Primer obstáculo

Puente roto.

Hele puede atravesar el vacío y activar un mecanismo circular al otro lado.

### Aprendizaje
El jugador no puede avanzar siempre solo.

## Beat 4 — Reconstrucción

La activación atrae fragmentos de piedra y forma un paso temporal.

### Mecánica
Luz + fragmentos + timing.

## Beat 5 — Primer recuerdo

El padre toca un punto de memoria. El mundo recibe una primera entrada de color y la música gana una nueva capa.

### Visual
Un recuerdo cálido aparece sin cortar completamente el control.

## Beat 6 — Alegría breve

Pequeña secuencia de movimiento fluido:
- impulso;
- plataformas;
- vegetación que responde;
- Hele jugando alrededor.

## Beat 7 — Sofi

La cámara se abre. Muy lejos, Sofi aparece sobre una estructura.

El padre se detiene.

Hele también.

Sofi desaparece tras un cambio de luz.

## Cierre

El camino continúa.

Pantalla a negro solo después de devolver control brevemente al jugador.

## Sistemas que deben existir

- character controller;
- camera rig;
- Hele follower;
- Hele contextual action;
- memory trigger;
- platform reconstruction;
- color-state manager;
- adaptive music hook;
- VFX event system;
- animation state machine.

## Lista de arte

- padre;
- Hele;
- silueta Sofi;
- set de ruina;
- arcos;
- plataformas;
- mecanismo;
- vegetación;
- niebla;
- cielo;
- partículas;
- memory FX;
- color transition FX.

## Lista de animación

- padre: idle/walk/run/jump/fall/land/turn/reach;
- Hele: hover/follow/play/activate/orbit/react;
- Sofi: distant idle/turn/disappear;
- world: bridge reconstruction/vegetation response.

## Métricas de éxito

- control agradable sin arte final;
- cero jitter de cámara;
- transición de animación invisible para el jugador;
- Hele legible sin HUD;
- el primer recuerdo produce cambio emocional claro;
- una captura del nivel puede usarse como key visual;
- rendimiento estable en hardware objetivo.

## No incluir todavía

- combate;
- inventario;
- árboles de habilidad;
- coleccionables complejos;
- múltiples finales;
- sistemas online.
