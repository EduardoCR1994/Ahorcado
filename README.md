# 🎮 Proyecto Final - Juego del Ahorcado

**Curso:** Programación Avanzada
**Código:** SC-601
**Profesor:** Luis Andrés Rojas Matey
**Grupo:** Eduardo Castro, Brandon Cespedes, Jimena Flores, Mariana Hidalgo

---

## 🧠 Descripción

Este es un proyecto web desarrollado en ASP.NET MVC 5 (Framework 4.8.1) que simula el juego del Ahorcado. Incluye:

* Módulo de administración de palabras.
* Registro de jugadores.
* Creación de partidas con niveles de dificultad.
* Lógica del juego y validación de letras.
* Escalafón de jugadores según su desempeño.

---

## 🛠 Tecnologías usadas

* ASP.NET MVC 5 (.NET Framework 4.8.1)
* C#
* Entity Framework Model First
* SQL Server (localdb o Azure opcional)
* Bootstrap 5

---

## 📁 Estructura del proyecto

```
Ahorcado/
├── Controllers/
│   ├── JugadoresController.cs
│   ├── PalabrasController.cs
│   └── PartidasController.cs
├── Models/
│   ├── Model1.edmx (Entity Framework Model)
│   └── EscalafonViewModel.cs
├── Views/
│   ├── Jugadores/
│   ├── Palabras/
│   ├── Partidas/
│   │   └── Escalafon.cshtml
│   └── Shared/_Layout.cshtml
├── Content/ (Bootstrap, CSS)
├── Scripts/ (jQuery, validación)
└── README.md
```

---

## 🧩 Instrucciones de ejecución

### 1. Requisitos

* Visual Studio 2022
* SQL Server Express o LocalDB
* .NET Framework 4.8.1

### 2. Restaurar base de datos

**Opcion 1:** Ejecutar `AhorcadoDB.sql` en SQL Server Management Studio:

1. Crear una base vacía llamada `AhorcadoDB`
2. Ejecutar el script completo.

**Opcion 2:** Adjuntar el archivo `.bak` desde SSMS

### 3. Ejecutar el proyecto

1. Abrir `Ahorcado.sln` en Visual Studio
2. Asegurarse que la cadena de conexión (`Web.config`) apunta a la base `AhorcadoDB`
3. Compilar y ejecutar (Ctrl + F5)

---

## 🎮 Lógica del juego

* Se escoge una palabra aleatoria (no repetida).
* Se tiene un tiempo según nivel:

  * Fácil: 90s  | Normal: 60s | Difícil: 30s
* Se permite un máximo de 5 errores.
* Se muestran botones de letras (A-Z).
* El juego termina al adivinar la palabra, agotar los errores o vencer el tiempo.

---

## 🏆 Escalafón

* Cada victoria suma puntos según nivel:

  * Fácil: +1 | Normal: +2 | Difícil: +3
* Cada derrota resta:

  * Fácil: -1 | Normal: -2 | Difícil: -3
* Tabla ordenada por puntaje.

---

## 📄 Extras

* Archivo `AhorcadoDB.sql`: script con la estructura y datos precargados (jugadores y palabras)
* Archivo `.bak`: respaldo opcional de la base.
* Layout visualmente optimizado con Bootstrap.
* Comentarios dentro del código para guiar la lógica.

---

## 🤝 Integrantes

| Nombre           | Carné      |
| ---------------- | ---------- |
| Eduardo Castro   | FI13005258 |
| Brandon Cespedes | FH22012992 |
| Jimena Flores    | FH23014559 |
| Mariana Hidalgo  | FH23015127 |

---

## 📬 Contacto

Cualquier duda sobre la implementación puede consultarse durante la exposición o vía Campus Virtual.
