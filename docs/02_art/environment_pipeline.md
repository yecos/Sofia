# SOFIA — Pipeline de Entornos

## Objetivo

Construir escenarios que funcionen simultáneamente como:
- espacio jugable;
- composición pictórica;
- vehículo narrativo;
- sistema emocional.

## Principio

**Primero gameplay y composición; después detalle.**

No pintar un escenario final antes de aprobar el recorrido y la cámara.

## Pipeline

### 1. Narrative Beat
Definir:
- emoción principal;
- recuerdo asociado;
- función jugable;
- entrada;
- salida;
- revelación;
- estado de color.

### 2. Greybox en Unity
Usar geometría simple.

Debe validar:
- distancias;
- salto;
- ritmo;
- verticalidad;
- rutas;
- cámara;
- escala del personaje.

### 3. Camera Lock
Antes del concept final:
- framing;
- zoom;
- dead zones;
- cámara de seguimiento;
- reveal shots;
- límites.

### 4. Screenshot del Greybox
MCP genera screenshots de los puntos principales.

### 5. Paintover / Keyframe
Sobre el screenshot:
- arquitectura;
- luz;
- atmósfera;
- foreground;
- midground;
- background;
- focos narrativos.

ChatGPT Image Generation puede apoyar:
- exploraciones;
- color keys;
- variaciones;
- mood;
- composición.

Las imágenes generadas sirven como desarrollo visual, no como asset final automático.

### 6. Diseño modular

Biblioteca propia de SOFIA:
- arcos;
- columnas;
- puentes;
- plataformas;
- mecanismos;
- vegetación;
- ruinas;
- elementos circulares de memoria.

Crear siluetas propias y evitar replicar escenarios de referencias externas.

### 7. Producción

Pipeline híbrido recomendado:

```
Blender / blockout 3D
        +
Krita/Photoshop / pintura
        ↓
capas 2D + geometría simple
        ↓
Unity
```

Usar 3D donde ayude a:
- perspectiva;
- parallax;
- cámaras;
- cascadas;
- plataformas;
- luz.

Usar pintura 2D donde aporte:
- textura;
- cielo;
- arquitectura distante;
- vegetación;
- detalle poético.

### 8. Separación por profundidad

Capas mínimas:
1. foreground occlusion;
2. foreground decorative;
3. gameplay plane;
4. near midground;
5. far midground;
6. background architecture;
7. sky;
8. atmosphere/VFX.

### 9. Shading

Crear Shader Graph propios para:
- expansión de color;
- desaturación emocional;
- dissolve de memoria;
- rim light de Hele;
- niebla;
- agua estilizada;
- distorsión por culpa;
- grano/papel muy sutil.

El efecto debe ser estable en movimiento, no solo bonito en captura.

### 10. Lighting Pass

La luz guía el ojo.

Reglas:
- gameplay plane siempre legible;
- Hele tiene prioridad visual;
- Sofi puede romper la paleta cuando aparece;
- evitar bloom excesivo;
- contraste de profundidad controlado.

### 11. VFX

Capas:
- polvo;
- pétalos;
- lluvia;
- niebla;
- fragmentos;
- luz;
- agua;
- vegetación reactiva.

Los VFX deben responder a gameplay y música.

### 12. In-engine Paint Review

Comparar:
- key art;
- screenshot actual;
- lectura en movimiento.

Iterar en Unity, no solo en Photoshop/Krita.

## Scene Brief

Cada escena debe tener un asset/documento con:
- emoción;
- paleta;
- mecánica;
- referencias;
- keyframe;
- ruta;
- camera beats;
- audio state;
- VFX state;
- memoria/revelación.

## Uso del MCP

MCP puede:
- construir greybox;
- colocar prefabs;
- configurar layers;
- crear cámaras;
- cambiar parámetros de materiales;
- activar estados emocionales;
- tomar screenshots;
- verificar referencias;
- medir performance.

MCP no aprueba composición ni arte final.

## Gate de aprobación

### Gate A — Greybox
¿Es divertido recorrerlo?

### Gate B — Composition
¿La lectura visual funciona?

### Gate C — Key Art
¿Se siente como SOFIA?

### Gate D — In-engine
¿El key art sobrevivió al movimiento?

### Gate E — Polish
¿Puede una captura del gameplay ser usada como imagen promocional?
