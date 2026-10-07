# VS01 — Despertar / primer bloque jugable

Fecha: 2026-10-07. Unity 6000.6.4f1. Rama `feature/vs01-despertar-90s`.

La escena `game/SofiaUnityProject/Assets/Sofia/Scenes/VerticalSlice/SCN_VS01_Despertar.unity` cubre únicamente 0:00–1:30. La ruta física termina en x≈192; no hay contenido de 2:00 en adelante. Se puede abrir directamente y entrar en Play Mode. El constructor reproducible está en **SOFIA > VS01 > Build Despertar 0-90s**; recrea la escena, así que los ajustes posteriores deben incorporarse al constructor.

| Beat | Resultado jugable y visual |
| --- | --- |
| 0:00 | Despertar sobre pasarela de piedra pintada, encuadre cercano, 2,5 s antes de devolver el control. |
| 0:30 | Primeros pasos por ruinas frías, subida suave y salto corto. |
| 1:00 | Hele emerge de la grieta, se aproxima con curiosidad y acaba siguiendo al padre. |
| 1:30 | Tres cambios de altura y espacio seguro al final del bloque. |

El escenario usa cuatro pinturas originales de ruinas, cascadas, niebla azul, luz dorada e hiedra. Cinco bandas de niebla y cuatro velos de cascada añaden deriva, parallax leve y flujo de agua; el follaje transparente enmarca la cámara por delante del personaje. Los puentes jugables alternan piedra tallada y arcos rotos con vegetación, y los bloques grises visibles se sustituyeron por piezas pintadas. Hay cinco placas de fondo a lo largo de la ruta; la de primeros pasos se repite una vez. El arte principal conserva cielo y arquitectura en placas completas, así que su profundidad interior aún no tiene parallax independiente. La comparación con el objetivo y las limitaciones están en `vs01_visual_target_review.md`.

Las cuatro hojas facilitadas por el usuario se guardan en `Assets/Sofia/VS01/Art/SpriteSheets/` **solo como referencias de poses**. Sus cuadros muestran diferencias de proporción y silueta, por lo que no conducen el personaje en Play Mode. El padre jugable usa temporalmente `SPR_Father_Profile.png`. La lámina de cinco vistas `REF_Father_Turnaround.png` establece una base consistente para separar piezas y riggear; no es todavía un PSD/PSB por capas ni un rig de Unity. Véase `vs01_father_rig_direction.md`.

Controles: A/D o flechas para caminar a 2,3 u/s; Shift para correr a 5 u/s; Espacio/W/flecha arriba para saltar; R para volver al checkpoint. F1–F4 saltan a los cuatro checkpoints solo en Editor. La física usa Rigidbody2D interpolado en FixedUpdate, aceleración/frenado, coyote time y buffer de salto de 0,13 s cada uno. El movimiento no depende del arte ni de la animación. Una caída profunda devuelve al último checkpoint.

Cinemachine sigue un ancla calculada tras la física, con damping horizontal/vertical y look-ahead. El encuadre inicial es de 6,5 unidades ortográficas y pasa suavemente a 8; durante la curiosidad de Hele llega a 7,3. `PF_Father`, `PF_Hele` y `PF_DespertarGreybox` mantienen la escena organizada. El nombre de este último prefab es histórico: contiene hoy parte del entorno pintado.

Validación: **5 PlayMode PASS, 0 FAIL** en la última ejecución (166,32 s), incluidas ruta caminando sin carrera y tiempos de storyboard, coyote/buffer, cámara, capturas y recuperación tras caída. **2 EditMode PASS** del proyecto base. En la ruta automatizada de marcha, los checkpoints se alcanzaron a **28,65 s / 56,90 s / 87,19 s**, con seis saltos. La muestra de cámara de 120 cuadros registró 60,02 FPS medios, tiempo máximo de cuadro de 0,01932 s, desplazamiento de reposo 0,000000 u y retroceso 0,000000 u. Son mediciones de Editor, no una validación de rendimiento de build ni una sesión de juego humana.

Las capturas reales de Game View en `docs/evidence/vs01/` corresponden a los cuatro checkpoints y se toman después de estabilizar la cámara. La niebla y los velos de agua se mueven de forma independiente; aún faltan el rig, las animaciones corporales y de capa, la separación de cielo y ruinas lejanas, audio y el pase final de Hele. El primer tramo queda listo para revisar controles y composición, sin considerarse todavía aprobado artísticamente.
