# SOFIA — Unity 6.6 + MCP + ChatGPT

## Decisión

Motor base: **Unity 6.6**.

SOFIA usará MCP para conectar un agente de IA con el Unity Editor y automatizar operaciones técnicas repetitivas, inspección del proyecto, pruebas y construcción de escenas.

## Arquitectura de trabajo

```
ChatGPT / OpenAI
        ↓
Director creativo + planificación + revisión
        ↓
MCP client / OpenAI API / Codex
        ↓
Unity MCP Server
        ↓
Unity 6.6 Editor
        ↓
Scene / Scripts / Tests / Profiler / Screenshots
        ↓
GitHub
```

## Nota sobre ChatGPT

El servidor MCP de Unity será local. ChatGPT no debe depender de una conexión local directa. El diseño debe permitir tres rutas:

1. Cliente MCP local compatible para trabajo cotidiano.
2. OpenAI Responses API + Secure MCP Tunnel cuando queramos orquestación OpenAI sobre el Unity local.
3. ChatGPT con app MCP de escritura cuando el plan/workspace lo permita.

El proyecto no debe quedar acoplado a un único cliente de IA.

## MCP recomendado para spike

Evaluar primero **IvanMurzak/Unity-MCP** por:
- herramientas de escena;
- GameObjects;
- assets;
- scripts;
- screenshots;
- consola;
- tests;
- profiler;
- posibilidad de herramientas MCP personalizadas.

Mantener **CoplayDev/unity-mcp** como alternativa.

No integrar dos MCP simultáneamente en producción.

## Rol de ChatGPT

ChatGPT actúa como:
- director creativo;
- diseñador de sistemas;
- diseñador narrativo;
- reviewer de arte;
- reviewer de animación;
- planificador de tareas;
- diseñador de pruebas;
- analista de screenshots;
- generador/revisor de código cuando sea necesario.

ChatGPT NO debe:
- decidir automáticamente que un asset está aprobado;
- sobrescribir arte fuente;
- modificar main sin revisión;
- generar cientos de cambios antes de verificar una escena.

## Flujo por tarea

### 1. Brief
Toda tarea empieza con:
- objetivo;
- emoción;
- escena;
- criterios de aceptación;
- referencias;
- restricciones.

### 2. Plan MCP
El agente traduce el brief a operaciones pequeñas.

Ejemplo:
```
crear escena proxy
→ colocar plataforma
→ configurar cámara
→ añadir PlayerSpawn
→ añadir HeleAnchor
→ entrar a Play Mode
→ tomar screenshot
→ leer consola
→ revisar
```

### 3. Ejecución
MCP realiza cambios en Unity.

### 4. Validación automática
- compila;
- consola sin errores;
- tests;
- Play Mode;
- profiler básico;
- screenshot.

### 5. Revisión visual
ChatGPT/humano revisa screenshot/video.

### 6. Iteración
Solo corregir diferencias concretas.

### 7. Commit
Commit únicamente después de pasar QA.

## Herramientas MCP personalizadas de SOFIA

Crear progresivamente:

### `sofia_scene_report`
Devuelve:
- escena;
- GameObjects importantes;
- cámara;
- luces;
- colliders;
- checkpoints;
- audio state;
- warnings.

### `sofia_capture_review`
- entra en Play Mode;
- va a checkpoint;
- captura screenshot;
- devuelve consola + métricas.

### `sofia_validate_scene`
Valida:
- naming;
- layers;
- prefabs rotos;
- referencias nulas;
- colliders;
- sorting;
- cámara;
- audio emitters.

### `sofia_playtest_route`
Ejecuta un recorrido automatizado del vertical slice y genera reporte.

### `sofia_emotion_state`
Permite fijar estados visuales:
- Silence;
- Joy;
- Sorrow;
- Distance;
- Guilt;
- Hele;
- Hope.

Sirve para probar arte, shaders y audio sin rejugar toda la escena.

## Regla de seguridad

La IA trabaja en ramas feature y no en `main`.

Estructura:
- `main`: estable;
- `develop`: integración;
- `feature/*`: trabajo normal;
- `art/*`: integración de arte;
- `animation/*`: animación;
- `audio/*`: audio.

## Definition of Done técnica

Una tarea automatizada no está terminada hasta que:
- Unity compila;
- no hay errores nuevos;
- tests relevantes pasan;
- la escena abre;
- Play Mode funciona;
- hay screenshot de validación;
- existe commit identificable.
