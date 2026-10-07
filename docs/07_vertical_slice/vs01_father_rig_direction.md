# Padre de SOFIA — dirección de rig para VS01

La referencia de identidad es `Assets/Sofia/VS01/Art/REF_Father_Turnaround.png`: frontal, perfil derecho, espalda y dos vistas de tres cuartos en una base común. `SPR_Father_Profile.png` es el recorte provisional del juego. Las cuatro hojas recibidas del usuario documentan poses; no deben tratarse como cuadros intercambiables de la animación final.

La lámina generada mide 1983×793 px. Es útil para aprobar proporciones y silueta, pero queda por debajo del objetivo de 3000–4000 px de alto por personaje y necesita limpieza artística antes de cortar piezas. El perfil jugable mide 1024×1536 px y tampoco equivale a un maestro de alta resolución.

Para el archivo PSB/PSD definitivo, separar cabeza, cabello trasero y delantero, torso, pelvis, brazos superiores, antebrazos, manos, muslos, pantorrillas, pies, botas, cuello y al menos cinco secciones de capa. Mantener pivotes y posiciones en un único canvas transparente. En Unity, importar con PSD Importer y 2D Animation; crear huesos desde pelvis hacia columna, cuello/cabeza, extremidades y secciones de capa, aplicar pesos y comprobar deformaciones en perfil antes de animar. IK puede ayudar en manos y pies, pero el controller de movimiento continúa gobernando el Rigidbody2D.

Prioridad del primer bloque: despertar, idle, inicio/ciclo/parada de caminar, carrera, giro, anticipación/despegue/subida/ápice/caída/aterrizaje, mirar a Hele y alcance. La capa necesita keyframes con retraso y movimiento secundario acotado: sigue al cuerpo al arrancar, avanza al frenar y sube brevemente al caer. La máquina Animator debe interpretar velocidad horizontal, velocidad vertical, grounded y estado de Hele; no debe aplicar root motion a la física.

Estado actual: turnaround y perfil preparados e integrados como arte; **PSD por capas, Sprite Skin, huesos, pesos, IK, clips y Animator todavía pendientes**. La captura del bloque usa el perfil estático para evitar cambios de silueta entre cuadros de las hojas de referencia.
