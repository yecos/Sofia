# SOFIA — Pipeline de Animación

## Objetivo

La animación debe ser uno de los mayores diferenciadores del juego.

Prioridades:
1. respuesta;
2. acting;
3. peso;
4. silueta;
5. fluidez;
6. secondary motion;
7. polish.

## Tecnología

Base recomendada para el primer vertical slice:
- Unity 6.3 LTS;
- Unity 2D Animation;
- Animator;
- Timeline;
- Cinemachine.

Evaluar Spine en un spike separado antes de comprometer producción completa. No migrar a Spine solo por feature list: debe demostrar una mejora real de calidad/velocidad.

## Arquitectura de locomoción

**El código mueve al personaje. La animación lo interpreta.**

No usar root motion como base del platforming.

Ventajas:
- input preciso;
- physics predecible;
- animación puede tener anticipación visual sin añadir lag;
- tuning independiente.

## Pipeline del Padre

### 1. Acting Brief
Antes de animar:
- qué siente;
- qué quiere;
- cuánto pesa emocionalmente;
- hacia dónde mira;
- tempo.

### 2. Model Sheet
- frontal;
- perfil;
- 3/4;
- proporciones;
- manos;
- capa;
- expresiones;
- silueta.

### 3. Layered Art
Separar:
- torso;
- cabeza;
- upper/lower arms;
- hands;
- upper/lower legs;
- feet;
- coat/cape;
- hair;
- accessories.

### 4. Rig
Crear bones + deformation.

### 5. Blocking
Solo poses clave:
- contacto;
- passing;
- extremes;
- anticipation;
- recovery.

### 6. Timing
Probar a velocidad real dentro del juego.

### 7. In-betweens / Curves
Refinar movimiento.

### 8. Secondary Motion
- capa;
- ropa;
- cabello;
- manos;
- respiración.

Evitar física completamente libre. Preferir movimiento controlado y springs limitados.

### 9. Integration
Configurar:
- Animator State Machine;
- Blend Trees;
- transitions;
- exit times solo donde tengan sentido;
- animation events mínimos.

### 10. Polish
Revisar frame a frame y en gameplay.

## Hele

Hele no necesita animación humana. Su personalidad sale de:
- aceleración;
- overshoot;
- squash/stretch;
- trails;
- pausa;
- curiosidad;
- órbitas;
- brillo;
- distancia al padre.

Combinar:
- keyframed motion;
- procedural follow;
- spring;
- VFX;
- intensidad de luz.

## Sofi

Al principio debe ser más simple y distante.
Aumentar detalle y acting conforme se acerca al protagonista.

Esto convierte la propia calidad/percepción de la animación en recurso narrativo.

## Hero Animations

Momentos clave pueden usar:
- frame-by-frame;
- hand correction;
- deformaciones específicas;
- Timeline;
- cámara diseñada alrededor del acting.

No todo el juego necesita el mismo costo por segundo de animación.

## Frecuencia visual

El juego puede correr a 60 fps mientras ciertas animaciones conservan una cadencia artística de 12/24 dibujos por segundo si el estilo lo pide.

Esto se decide por prueba, no por dogma.

## Animation Review Loop

```
brief
→ thumbnails
→ blocking
→ Unity
→ gameplay capture
→ review
→ timing pass
→ polish
→ final capture
```

Nunca aprobar una animación fuera del motor.

## MCP + animación

MCP puede:
- asignar clips;
- configurar Animator;
- cambiar parámetros;
- reproducir escenas;
- fijar estados;
- capturar screenshots;
- iniciar grabaciones/pruebas;
- detectar referencias rotas.

MCP no debe:
- retocar acting automáticamente y marcarlo como final;
- cambiar timings aprobados sin comparación;
- reemplazar el archivo fuente del animador.

## Métricas de locomoción

Mantener documentados:
- acceleration;
- deceleration;
- max speed;
- jump takeoff;
- coyote time;
- input buffering;
- fall gravity;
- landing recovery.

Cada animación debe diseñarse alrededor de esas métricas.

## Gate de aprobación

1. Silueta.
2. Peso.
3. Respuesta.
4. Timing.
5. Acting.
6. Secondary motion.
7. Cámara.
8. Audio.
9. Loop/transición.
10. Lectura emocional.
