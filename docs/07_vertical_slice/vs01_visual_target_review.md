# VS01 — revisión del objetivo visual

Referencia facilitada por el usuario: [Despertar entre ruinas y niebla](visual_refs/VS01_target_2026-10-07.png). Los rótulos y la cuadrícula del documento son anotaciones de dirección artística, no interfaz para el juego.

## Qué comunica la referencia

La imagen usa arquitectura monumental de piedra con profundidad clara entre cielo, ruinas lejanas, cascadas, ruinas medias, puente de juego y follaje muy próximo. El contraste se concentra en el primer plano y en los rayos cálidos; la lejanía se enfría y se disuelve en niebla. El padre mantiene una silueta legible sobre el puente y Hele aporta un foco amarillo pequeño pero inequívoco. A 0:30 el mundo revela cascadas; a 1:00 la luz de Hele altera el foco de atención; a 1:30 la composición abre verticalmente el camino.

## Integrado en la escena jugable

- Cuatro nuevas pinturas originales para despertar, primeros pasos, encuentro con Hele y continuación. Son assets de juego sin texto de la referencia.
- Puentes con arcos de piedra, desconchados, musgo e hiedra, más pasarela de piedra lisa donde la lectura de colisión lo pide.
- Marco de hojas y ramas transparentes ligado a la cámara, por delante del jugador; no tapa el área central de movimiento.
- Cinco bandas de niebla con deriva y parallax sutil, más cuatro velos de cascada independientes con desplazamiento de agua animado.
- Encuadres y capturas de los cuatro checkpoints revisados en Game View; los bloques grises grandes y los arcos geométricos quedaron fuera de las composiciones.
- Se conservaron los colliders y la lógica de movimiento. La prueba Play Mode de capturas y recuperación pasó con las nuevas capas.

## Brecha restante para alcanzar el nivel final de la lámina

Las pinturas principales siguen siendo placas completas: la profundidad de cielo y ruinas dentro de cada placa no está separada en capas propias. Las bandas de niebla y los velos de cascada sí son elementos aparte; la niebla deriva suavemente con parallax y el agua tiene un flujo UV sutil. El follaje delantero también es una capa propia. El padre usa un perfil estático mientras se prepara el arte por piezas y el rig; su capa no tiene animación secundaria. Hele mantiene comportamiento jugable, pero necesita un pase final de partículas, brillo y respuesta a la luz. También faltan audio ambiental, mezcla de iluminación por zonas y un ajuste de las uniones entre pinturas y módulos del puente.

La referencia muestra el objetivo de producción; el primer bloque ya tiene arte pintado y movimiento ambiental, pero todavía requiere trabajo de personaje y de capas de fondo para alcanzar el acabado final. El siguiente pase visual debe riggear y animar al padre y separar cielo y arquitectura lejana de las placas antes de añadir contenido posterior a 1:30.
