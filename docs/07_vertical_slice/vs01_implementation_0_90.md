# VS01 — Despertar / primer bloque jugable

Fecha: 2026-10-07. Unity 6000.6.4f1. Rama `feature/vs01-despertar-90s`.

La escena `game/SofiaUnityProject/Assets/Sofia/Scenes/VerticalSlice/SCN_VS01_Despertar.unity` cubre únicamente 0:00–1:30. La ruta física termina en x≈192; no hay contenido de 2:00 en adelante. Se puede abrir directamente y entrar en Play Mode. El constructor reproducible está en **SOFIA > VS01 > Build Despertar 0-90s**; recrea la escena, así que los ajustes posteriores deben incorporarse al constructor.

| Beat | Resultado jugable y visual |
| --- | --- |
| 0:00 | Despertar sobre pasarela de piedra pintada, encuadre cercano, 2,5 s antes de devolver el control. |
| 0:30 | Primeros pasos por ruinas frías, subida suave y salto corto. |
| 1:00 | Hele emerge de la grieta, se aproxima con curiosidad y acaba siguiendo al padre. |
| 1:30 | Tres cambios de altura y espacio seguro al final del bloque. |

El escenario usa fondos pictóricos originales de ruinas, una pasarela ilustrada de contorno irregular, niebla atmosférica integrada en los fondos y un recorte pintado estable del padre. Se quitaron los arcos geométricos superpuestos al fondo. Algunas piedras pequeñas de interacción siguen siendo greybox; el arte de entorno y la integración de capas aún requieren dirección artística final. Los fondos se repiten a lo largo de la ruta y todavía se perciben sus variaciones.

Las cuatro hojas facilitadas por el usuario se guardan en `Assets/Sofia/VS01/Art/SpriteSheets/` **solo como referencias de poses**. Sus cuadros muestran diferencias de proporción y silueta, por lo que no conducen el personaje en Play Mode. El padre jugable usa temporalmente `SPR_Father_Profile.png`. La lámina de cinco vistas `REF_Father_Turnaround.png` establece una base consistente para separar piezas y riggear; no es todavía un PSD/PSB por capas ni un rig de Unity. Véase `vs01_father_rig_direction.md`.

Controles: A/D o flechas para caminar a 2,3 u/s; Shift para correr a 5 u/s; Espacio/W/flecha arriba para saltar; R para volver al checkpoint. F1–F4 saltan a los cuatro checkpoints solo en Editor. La física usa Rigidbody2D interpolado en FixedUpdate, aceleración/frenado, coyote time y buffer de salto de 0,13 s cada uno. El movimiento no depende del arte ni de la animación. Una caída profunda devuelve al último checkpoint.

Cinemachine sigue un ancla calculada tras la física, con damping horizontal/vertical y look-ahead. El encuadre inicial es de 6,5 unidades ortográficas y pasa suavemente a 8; durante la curiosidad de Hele llega a 7,3. `PF_Father`, `PF_Hele` y `PF_DespertarGreybox` mantienen la escena organizada. El nombre de este último prefab es histórico: contiene hoy parte del entorno pintado.

Validación: **5 PlayMode PASS, 0 FAIL**, incluidas ruta caminando sin carrera y tiempos de storyboard, mecánica de coyote/buffer, cámara, capturas y recuperación tras caída. **2 EditMode PASS** del proyecto base. En la ruta automatizada de marcha, los checkpoints se alcanzaron a **28,65 s / 56,91 s / 87,16 s**, con seis saltos. La muestra de cámara de 120 cuadros registró 59,60 FPS medios, desplazamiento de reposo 0,000000 u y retroceso 0,000000 u. Son mediciones de Editor, no una validación de rendimiento de build ni una sesión de juego humana.

Las capturas reales de Game View en `docs/evidence/vs01/` corresponden a los cuatro checkpoints y se toman después de estabilizar la cámara. La experiencia está lista para revisión de controles y composición, pero el rig, las animaciones corporales y de capa, audio y los elementos de arte pequeños no están terminados. El primer tramo no se considera aprobado artísticamente todavía.
