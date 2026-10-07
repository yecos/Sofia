# VS01 — Despertar / primer bloque jugable

Fecha: 2026-10-07. Unity: **6000.6.4f1**.
Rama: `feature/vs01-despertar-90s`, basada en `setup/unity-bootstrap`.

## Escena y alcance

Abrir `game/SofiaUnityProject/Assets/Sofia/Scenes/VerticalSlice/SCN_VS01_Despertar.unity` y entrar en Play Mode.

Se implementan los cuatro beats de 0:00 a 1:30 del storyboard. Los nombres temporales identifican beats y checkpoints; **no son una grabación continua de 90 segundos ni una duración ya validada**. El greybox está comprimido para probar control y relaciones. El ritmo de exploración y la duración ±5 s requieren una pasada de diseño con el jugador; la ruta automatizada a velocidad de carrera tarda unos 14 s después del despertar.

| Beat | Implementación | Evidencia |
| --- | --- | --- |
| 0:00 | Plataforma circular con borde roto, arquitectura monumental en planos azul/gris, pose comprimida de despertar, control bloqueado 2,5 s, plano amplio que se acerca suavemente. | `../evidence/vs01/vs01_000.png` |
| 0:30 | Plataforma ancha, subida de 0,6 u, piedra de 0,7 u, hueco de 2 u y aterrizaje seguro. | `../evidence/vs01/vs01_030.png` |
| 1:00 | Grieta, núcleo amarillo/halo; aparición por proximidad, dos órbitas, retroceso si el padre se aproxima, pausa y Follow; push-in de 8,75 %. | `../evidence/vs01/vs01_060.png` |
| 1:30 | Arcos y dos ascensos con descenso posterior, espacio seguro al final, Hele se adelanta con distancia limitada y acompaña los cambios de altura. | `../evidence/vs01/vs01_090.png` |

El límite derecho cierra la implementación. No existe puente, mecanismo, Activate, memoria ni contenido de 2:00+.

## Controles

- A/D o flechas: caminar a 2,3 u/s.
- Shift: correr a 5 u/s.
- Espacio/W/flecha arriba: salto de 9,4 u/s, gravedad 1,8; vuelo aproximado de 1,06 s en suelo plano.
- R: volver al último checkpoint.
- En Editor, F1/F2/F3/F4: saltar a CP_000/030/060/090 para revisión.

Coyote time y jump buffer: 0,13 s cada uno. Movimiento en FixedUpdate con Rigidbody2D interpolado, contacto por cast filtrado al suelo, aceleración 22 y frenado 28 u/s². Caída profunda: retorno al checkpoint con velo visual suave, sin contador de vidas.

## Organización

`Assets/Sofia/VS01/` contiene runtime y sus assemblies, `Editor/VS01Builder.cs`, `Tests/`, `Materials/`, `Meshes/` y `Prefabs/`.

Prefabs: `PF_Father`, `PF_Hele`, `PF_DespertarGreybox`. Las referencias entre padre, Hele y cámara se asignan en la escena. El constructor es reproducible desde `SOFIA > VS01 > Build Despertar 0-90s`; recrea la escena y sus assets de greybox, por lo que debe usarse antes de realizar ajustes manuales que se quieran conservar.

CinemachineCamera + PositionComposer y Brain en LateUpdate siguen un anchor basado en la posición interpolada del padre. Look-ahead horizontal ±2,5 u, damping horizontal 0,45 y vertical 0,8; horizonte con desplazamiento vertical limitado. No usa el CameraFollow del smoke test.

Se reparó la referencia vacía de renderer del URP del bootstrap con un UniversalRendererData persistente. Se conservó la escena smoke y se añadió VS01 a Build Settings.

## Validación ejecutada

Unity MCP `tests_run`, PlayMode, assembly `Sofia.VS01.Tests`: **4 pruebas individuales PASS, 0 fallos**.

- Recorrido completo usando movimiento físico y saltos; frenado verificado al detener la entrada.
- Coyote jump tras dejar el borde y buffer antes de aterrizar.
- Despertar, Curious → Follow, cuatro capturas reales Game View en checkpoints y retorno tras caída.
- Cámara estable en reposo y sin retrocesos durante marcha constante.

Métricas de cámara: 120 frames, 60,00 FPS medios, máximo 0,018 s por frame, desplazamiento máximo en reposo 0,000000 u y retroceso máximo 0,000000 u. Esta medición corta en Editor no sustituye un perfil de rendimiento de build ni valida todas las maniobras.

EditMode existente: **2 PASS** (estructura SOFIA y versión de Unity). Consulta de consola posterior: **0 errores** en los últimos cinco minutos. El resumen MCP cuenta un nodo adicional de suite; se reportan aquí los cuatro casos individuales, no cinco pruebas.

## Estado para revisión

Greybox técnicamente jugable y con QA automático pasado. Pendientes dentro de este mismo tramo: revisión humana del feel, expansión/ajuste del ritmo para alcanzar la duración del storyboard, animación del padre y arte/niebla/audio finales. No se considera aprobación artística ni del VS01 completo. Mantener el desarrollo en 0:00–1:30 hasta cerrar esa revisión.
