# VS01 — revisión del objetivo visual

Referencia facilitada por el usuario: [Despertar entre ruinas y niebla](visual_refs/VS01_target_2026-10-07.png). Los rótulos y la cuadrícula del documento son anotaciones de dirección artística, no interfaz para el juego.

## Qué comunica la referencia

La imagen usa arquitectura monumental de piedra con profundidad clara entre cielo, ruinas lejanas, cascadas, ruinas medias, puente de juego y follaje muy próximo. El contraste se concentra en el primer plano y en los rayos cálidos; la lejanía se enfría y se disuelve en niebla. El padre mantiene una silueta legible sobre el puente y Hele aporta un foco amarillo pequeño pero inequívoco. A 0:30 el mundo revela cascadas; a 1:00 la luz de Hele altera el foco de atención; a 1:30 la composición abre verticalmente el camino.

## Integrado en la escena jugable

- Cuatro nuevas pinturas originales para despertar, primeros pasos, encuentro con Hele y continuación. Son assets de juego sin texto de la referencia.
- Puentes con arcos de piedra, desconchados, musgo e hiedra, más pasarela de piedra lisa donde la lectura de colisión lo pide.
- Marco de hojas y ramas transparentes ligado a la cámara, por delante del jugador; no tapa el área central de movimiento.
- Encuadres y capturas de los cuatro checkpoints revisados en Game View; los bloques grises grandes y los arcos geométricos quedaron fuera de las composiciones.
- Se conservaron los colliders y la lógica de movimiento existentes. Las cinco pruebas Play Mode siguieron pasando tras el cambio visual.

## Brecha restante para alcanzar el nivel final de la lámina

Las nuevas pinturas de arquitectura y niebla son placas completas: su profundidad interior está pintada, pero aún no son capas físicas separadas con parallax independiente. Las cascadas son imagen fija. El follaje delantero sí es una capa propia. El padre usa un perfil estático mientras se crea el PSD/PSB por piezas y el rig; su capa todavía no tiene animación secundaria. Hele mantiene comportamiento jugable, pero requiere un tratamiento final de partículas, brillo y respuesta a la luz. También faltan audio ambiental, mezcla de iluminación por zonas y un pase de dirección artística sobre las uniones entre placas y módulos del puente.

La referencia muestra un objetivo de producción, no una afirmación de que el slice actual ya esté finalizado. La siguiente pasada visual debe empezar por separar cielo, ruinas lejanas, niebla, arquitectura media, cascadas y primer plano en assets animables antes de añadir contenido posterior a 1:30.
