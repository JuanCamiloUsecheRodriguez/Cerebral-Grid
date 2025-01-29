# **Prueba Unity: Juego de Memoria**

## **Descripción del Proyecto**

Este proyecto consiste en un **juego de memoria** desarrollado en Unity, cuyo objetivo principal es encontrar parejas en una rejilla dinámica y configurable. Las posiciones y valores de las casillas de la rejilla son leídas desde un archivo JSON, y los resultados del juego se guardan también en formato JSON al finalizar la partida.

### **Características del Juego**

- **Rejilla Dinámica**:  
  - Tamaño configurable entre 2x2 y 8x8.  
  - La configuración inicial de las casillas se define mediante un archivo JSON que contiene la posición y el valor de cada casilla.  
  - Los valores de las casillas van del 0 al 9 para limitar las opciones a 10.

- **Reglas de Juego**:  
  - Al iniciar, todas las casillas están cubiertas.  
  - El usuario puede clicar una casilla para revelarla.  
  - Si dos casillas consecutivas tienen el mismo valor, permanecen visibles y cambian de color. Si no coinciden, ambas se ocultan nuevamente.  
  - El juego termina al encontrar todas las parejas.

- **Sistema de Resultados**:  
  Al finalizar el juego, se genera un archivo JSON que incluye:  
  - Tiempo total de juego (en segundos).  
  - Total de clics realizados.  
  - Total de parejas encontradas.  
  - Puntaje final, calculado en función de los clics, el tiempo y las parejas.

- **Verificación de JSON**:  
  Antes de iniciar el juego, el sistema valida que el archivo JSON de configuración cumpla con las restricciones (dimensiones válidas, valores en el rango permitido, etc.).

- **Extras**:  
  - **Leaderboard Local**:  
    Se registra un leaderboard local con los 3 mejores puntajes, identificando a los usuarios con un nombre simple.  
    Este leaderboard se muestra al inicio del juego y se actualiza si el jugador obtiene un puntaje superior.

- **Aspectos Audiovisuales**:  
  Gráficos, animaciones, efectos de partículas y sonidos que enriquecen la experiencia del usuario.

---

## **Instrucciones de Uso**

1. **Cargar un archivo JSON**:  
   - El archivo debe tener una estructura como la siguiente:  
     ```json
     {
       "blocks": [
         { "R": 1, "C": 1, "number": 1 },
         { "R": 2, "C": 1, "number": 5 },
         ...
       ]
     }
     ```
   - Coloca el archivo en la carpeta correspondiente dentro del proyecto antes de iniciar el juego.

2. **Iniciar el Juego**:  
   - Al ejecutar el juego, la rejilla se generará automáticamente a partir del archivo JSON cargado.  
   - Selecciona casillas para revelar sus valores y encuentra las parejas para ganar.

3. **Finalizar el Juego**:  
   - Al completar todas las parejas, los resultados se guardarán automáticamente en un archivo JSON llamado `gamer_esults.json` en la carpeta de Streaming Assets.

4. **Leaderboard**:  
   - Al inicio del juego, se mostrará el leaderboard local con los 3 mejores puntajes registrados.  
   - Si obtienes un nuevo puntaje más alto, el leaderboard se actualizará automáticamente.

---

## **Dispositivos Compatibles**

- **Plataforma**: PC (Windows).  
- **Requisitos mínimos**:  
  - Sistema Operativo: Windows 7/8/10/11.  
  - Procesador: Intel i3 o equivalente.  
  - Memoria RAM: 4 GB.  
  - Espacio en disco: 500 MB disponibles.  
  - Resolución mínima recomendada: 1280x720.

---

## **Tecnologías Utilizadas**

- **Motor de Juego**: Unity (versión 2023.1.1f1.  
- **Lenguaje de Programación**: C#.  

---

## Licencias 

  

Este proyecto utiliza diversas tecnologías y recursos, cada uno con sus propias licencias. A continuación, se detallan las licencias de las tecnologías utilizadas: 

  

- [High Definition RP ](https://docs.unity3d.com/Packages/com.unity.render-pipelines.high-definition@15.0/license/LICENSE.html) 



---

## **Enlace de Descarga**

Descarga el ejecutable del proyecto aquí:  
[**Prueba Unity: Juego de Memoria - Descargar Ejecutable**]([https://tu-enlace-de-descarga.com](https://drive.google.com/file/d/117_LZuVAnEo3Fr3qSISH0ed7xljiozEb/view?usp=sharing))

---

## **Agradecimientos**

Este proyecto ha sido posible gracias a los siguientes recursos gratuitos:

- **Sprites de los cubos**:  
  [Ghost Pixxells - Pixel Food](https://ghostpixxells.itch.io/pixelfood)

- **Sprites de los cuadros de menú**:  
  [BDragon1727 - Border and Panels Menu (Part 3)](https://bdragon1727.itch.io/border-and-panels-menu-part-3)

- **Efectos de sonido**:  
  [Freesound](https://freesound.org/)

- **Material de la tela de la mesa**:  
  [FreePBR - Coarse Old Fabric](https://freepbr.com/product/coarse-old-fabric/)

---


