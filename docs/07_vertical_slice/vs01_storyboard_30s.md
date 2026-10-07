# VS01 — El Primer Destello — Storyboard de producción cada 30 s

Duración objetivo: **6:00 min**
Formato: **2D/2.5D side-scroller cinematográfico**
Objetivo del nivel: presentar al padre, presentar a Hele, enseñar cooperación básica, mostrar la primera transformación emocional del mundo y cerrar con la primera visión clara de Sofi.

## Reglas globales

- Sin HUD permanente.
- Sin combate.
- Sin muerte ni castigo duro; una caída profunda debe devolver al jugador al último punto seguro mediante transición visual suave.
- Gameplay objetivo: 60 FPS.
- Movimiento preciso, cámara suave, animación expresiva.
- El plano jugable debe ser siempre legible.
- Hele debe ser el único amarillo saturado al inicio.
- El mundo comienza casi monocromático y gana color solo después del primer recuerdo.
- El nivel debe poder entenderse sin diálogo.

## Lenguaje visual

Inicio:
- azul grisáceo;
- marfil frío;
- niebla;
- arquitectura monumental;
- mucho espacio negativo.

Después de 4:00:
- aparecen melocotón, rosa suave y dorado;
- el color entra desde el punto de memoria hacia el entorno;
- Hele aumenta ligeramente su intensidad.

## Macro-layout

Sector A — Despertar
Sector B — Primer contacto con Hele
Sector C — Arcos rotos y primer recorrido
Sector D — Puente imposible / cooperación
Sector E — Caída a la cuenca de memoria
Sector F — Recuerdo y transformación
Sector G — Carrera de alegría
Sector H — Mirador de Sofi

---

# Timeline

## 0:00 — Despertar

### Imagen
Plano extremadamente amplio. El padre ocupa aproximadamente 3–4 % de la altura de pantalla. Está tumbado o arrodillado sobre una gran plataforma circular rota. Al fondo: arcos gigantes, columnas semienterradas y una niebla profunda.

### Jugador
Durante 2–3 s no tiene control. Luego puede levantarse y caminar.

### Cámara
Plano fijo inicial. Después empieza un follow muy lento.

### Animación
Padre:
- respiración;
- pequeña tensión al incorporarse;
- mirar alrededor;
- ponerse de pie con peso.

### Audio
- viento;
- piedra;
- respiración;
- un piano muy aislado;
- sin melodía completa.

### Mecánica
Solo Move.

### Objetivo
Transmitir vacío y escala.

---

## 0:30 — Primeros pasos

### Imagen
El padre camina hacia la derecha por una plataforma ancha. El fondo tiene profundidad clara: foreground oscuro, plano jugable medio, estructuras lejanas en niebla.

### Jugador
Aprende:
- caminar;
- correr;
- pequeña subida;
- salto corto.

### Level design
Introducir:
- desnivel de 0.5–1 m;
- hueco pequeño;
- pequeña piedra que obliga a saltar.

### Cámara
Follow lateral con damping suave. Nunca centrar al personaje exactamente; dejar más aire hacia la dirección de avance.

### Audio
El piano agrega una segunda nota muy tenue.

### Objetivo
Que moverse ya se sienta placentero antes de cualquier puzzle.

---

## 1:00 — Aparición de Hele

### Imagen
En una grieta del suelo aparece un punto amarillo muy pequeño. Primero parece reflejo; luego se eleva y revela movimiento propio.

### Jugador
Se detiene de forma natural porque Hele cruza delante.

### Hele
Comportamiento:
- curiosidad;
- dos pequeñas órbitas;
- retroceso cuando el padre se acerca;
- breve pausa;
- luego avanza hacia la derecha.

### Cámara
Hace un pequeño push-in de 8–10 %.

### Audio
Aparece el primer timbre de Hele:
- celesta;
- campana suave;
- shimmer muy corto.

### Mecánica
Hele todavía no es controlable.

### Objetivo
Presentarla como personaje, no como indicador de objetivo.

---

## 1:30 — Seguir a Hele

### Imagen
Hele atraviesa una secuencia de arcos a distintas alturas. El padre debe subir y bajar por ruinas simples.

### Jugador
Aprende:
- salto medio;
- caída controlada;
- pequeño borde;
- cambio de altura.

### Level design
No más de 3 saltos consecutivos.

### Cámara
Introduce verticalidad:
- pequeño desplazamiento arriba/abajo;
- mantener horizonte estable.

### Hele
Se adelanta, espera, vuelve un poco si el jugador tarda.

### Audio
Pequeñas respuestas sonoras de Hele según distancia.

### Objetivo
Introducir relación de acompañamiento.

---

## 2:00 — El puente imposible

### Imagen
Gran vacío. El padre llega a un borde. Al otro lado hay una estructura circular apagada. Hele puede cruzar; el padre no.

### Jugador
Primera lectura clara de:
“yo no puedo pasar; Hele sí”.

### Mecánica
Se introduce HeleAction.

### Acción
Al mantener/activar HeleAction, Hele viaja al mecanismo.

### Cámara
Se abre para mostrar simultáneamente:
- padre a la izquierda;
- vacío;
- mecanismo a la derecha.

### Audio
La música casi desaparece. Queda viento + sonido de energía.

### Objetivo
Primera cooperación.

---

## 2:30 — Reconstrucción del puente

### Imagen
El mecanismo responde a Hele. Fragmentos de piedra flotan desde el fondo y forman una ruta irregular.

### Mecánica
Plataformas temporales.

### Jugador
Debe:
- esperar lectura visual;
- correr;
- saltar entre 3–4 fragmentos.

### Arte
Los fragmentos tienen una línea dorada interior, no un glow agresivo.

### Cámara
Follow más dinámico, con leve zoom-out durante la secuencia.

### Audio
Primer pulso rítmico suave.

### Objetivo
Primer momento claramente “jugable” y memorable.

---

## 3:00 — Ruptura y caída

### Imagen
Cuando el padre llega a la mitad del puente, una plataforma se rompe. No es castigo: es parte del guion.

### Jugador
Pierde apoyo y cae.

### Cámara
La cámara acompaña la caída y revela una capa inferior del escenario.

### 2.5D
Por primera vez el juego cambia de plano:
- el fondo pasa a primer plano;
- una estructura antes decorativa se convierte en ruta jugable.

### Hele
Desciende rápidamente junto al padre.

### Audio
Silencio momentáneo + viento + impacto suave.

### Objetivo
Demostrar que el mundo no es un side-scroll plano.

---

## 3:30 — Cuenca de memoria

### Imagen
Zona baja, húmeda, con una lámina de agua muy quieta. Las ruinas se reflejan. Hele ilumina pequeños fragmentos bajo la superficie.

### Jugador
El ritmo baja.

### Mecánica
Caminar por agua poco profunda.
Pequeños reflejos reaccionan a la presencia del jugador.

### Sofi
No aparece físicamente.
En un reflejo muy breve puede insinuarse una silueta infantil que no coincide con el padre.

### Cámara
Más cercana.

### Audio
Agua + respiración + casi nada de música.

### Objetivo
Crear intimidad después de la caída.

---

## 4:00 — Primer recuerdo

### Imagen
El padre encuentra un pequeño objeto/forma abstracta vinculada a memoria. No debe ser demasiado literal todavía.

### Acción
Interactuar.

### Cinemática integrada
No cortar a video externo.
El padre extiende la mano.

### Hele
Se posa o flota cerca de la mano.

### Transformación
Una onda de color viaja desde el recuerdo:
- primero dorado;
- luego melocotón;
- luego rosa suave;
- finalmente parte del cielo cambia.

### Audio
Por primera vez se escucha un fragmento claro del leitmotiv de Sofi.

### Objetivo
Primer gran payoff emocional del juego.

---

## 4:30 — La alegría entra en el movimiento

### Imagen
La misma arquitectura fría ahora tiene flores pequeñas, partículas cálidas y zonas de luz.

### Mecánica
Introducir impulso de alegría.

Puede ser:
- corriente luminosa;
- flor/plataforma que impulsa;
- zona que amplifica salto.

### Jugador
Cadena corta:
- correr;
- impulso;
- salto largo;
- aterrizaje;
- pequeño ascenso.

### Hele
Más juguetona:
- órbitas;
- overshoot;
- adelantos;
- retorno.

### Cámara
Más viva, sin perder suavidad.

### Audio
Entran cuerdas suaves y pulso.

### Objetivo
Que la emoción cambie también el game feel.

---

## 5:00 — Coordinación con Hele

### Imagen
Dos puntos de luz separados controlan una gran estructura vertical.

### Mecánica
Hele activa un ancla mientras el padre aprovecha el cambio en el entorno.

Ejemplo:
- Hele mantiene una corriente de luz;
- esa corriente eleva una plataforma;
- el padre debe atravesar mientras permanece activa.

### Diseño
Sin precisión extrema.
Debe sentirse elegante, no castigador.

### Cámara
Plano más alto y vertical.

### Audio
Música responde a cada activación.

### Objetivo
Mostrar el potencial futuro de Hele como sistema jugable.

---

## 5:30 — La revelación de Sofi

### Imagen
El padre llega a un gran mirador.

La cámara empieza cerca y se abre lentamente hasta mostrar una estructura monumental al fondo.

Allí está Sofi.

Muy pequeña, pero claramente humana.

### Padre
Se detiene.
Respira.
Un pequeño paso involuntario hacia delante.

### Hele
Se detiene también.
Por primera vez no juega ni orbita.

### Sofi
Solo:
- silueta;
- pequeño giro;
- posible movimiento de cabello/ropa.

### Audio
El leitmotiv de Sofi aparece casi completo pero sin resolución.

### Jugador
Puede caminar unos pasos, pero no alcanzarla.

### Objetivo
Cerrar el arco de búsqueda inicial.

---

## 6:00 — Desaparición y promesa

### Imagen
Una nube de luz / niebla cruza entre ambos. Cuando despeja, Sofi ya no está.

Pero queda una pequeña marca cálida en el lugar.

### Hele
Se acerca al padre.
Breve contacto de luz.

### Mundo
Una ruta nueva se abre hacia la derecha.

El mundo no vuelve a gris total:
la primera conquista emocional permanece.

### Cámara
Vuelve lentamente a distancia de gameplay.

### Audio
El tema queda sin resolución y desemboca en ambiente.

### Control
Se devuelve control al jugador durante 3–5 s antes del corte.

### Final
Fade a negro.

Texto interno de producción:
**END VS01**

No mostrar este texto en versión final del juego.

---

# Duración y tolerancias

Cada bloque dura aproximadamente 30 s, pero durante implementación se permite:
- ±5 s en exploración;
- ±3 s en secuencias dirigidas.

Duración total aceptable del vertical slice:
**5:30–6:30 min**

---

# Métricas del nivel

## Player
Objetivo inicial:
- velocidad caminata: 2.0–2.5 u/s;
- carrera: 4.5–5.5 u/s;
- salto: 1.0–1.2 s total;
- coyote time: 0.10–0.15 s;
- jump buffer: 0.10–0.15 s.

Valores definitivos se deciden por feel.

## Cámara
- 60 fps;
- no jitter;
- look-ahead horizontal;
- damping ligero;
- zoom contextual limitado;
- evitar cortes duros.

## Hele
Tres estados mínimos:
- Follow;
- Curious;
- Activate.

## Checkpoints
- 0:00;
- 2:00;
- 3:30;
- 4:00;
- 5:30.

---

# Assets mínimos

## Padre
Proxy primero, después rig final.

## Hele
- núcleo amarillo;
- halo;
- trail;
- partículas;
- 3 estados.

## Sofi
- silueta lejana;
- idle;
- turn.

## Entorno
- set de arco;
- plataforma circular;
- bridge fragments;
- memory basin;
- mecanismo circular;
- mirador;
- arquitectura de fondo;
- agua;
- niebla;
- vegetación reactiva.

## VFX
- Hele trail;
- mecanismo;
- bridge reconstruction;
- memory wave;
- color spread;
- joy particles;
- Sofi reveal.

---

# Criterio de aceptación del VS01

El nivel solo se considera aprobado cuando:
- se entiende sin tutorial escrito;
- el Player se siente bien sin arte final;
- Hele se percibe como personaje;
- el primer puzzle se entiende sin UI;
- el cambio de plano de 3:00 funciona;
- el recuerdo de 4:00 cambia arte + audio + gameplay;
- Sofi es reconocible a 5:30;
- cámara sin jitter;
- 0 errores;
- performance estable;
- cada frame clave puede compararse con su storyboard de 30 s.
