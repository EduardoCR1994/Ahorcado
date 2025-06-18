# Proyecto Ahorcado - Programación Avanzada SC-601

## Integrantes

- Castro Rojas Eduardo (FI13005258)
- Cespedes Morales Brandon Steven (FH22012992)
- Flores Carmona Jimena (FH23014559)
- Hidalgo De La O Mariana (FH23015127)

## Tecnología

- ASP.NET Web Application (.NET Framework 4.8.1)
- Entity Framework 6 (Code First)
- SQL Azure

## Diagrama de Base de Datos (Mermaid)

# Modelo de Base de Datos

A continuación se presenta el modelo entidad-relación utilizado para el desarrollo del juego Ahorcado.

```mermaid
erDiagram
    Jugador {
        INT Identificacion PK
        NVARCHAR Nombre
    }

    Palabra {
        INT PalabraID PK
        NVARCHAR Texto
        NVARCHAR TextoNormalizado
        BIT TieneTilde
        BIT Usada
    }

    Partida {
        INT PartidaID PK
        INT JugadorID FK
        INT PalabraID FK
        NVARCHAR Nivel
        DATETIME FechaInicio
        INT DuracionSegundos
        NVARCHAR Resultado
    }

    Intento {
        INT IntentoID PK
        INT PartidaID FK
        CHAR Letra
        BIT EsCorrecta
        DATETIME FechaIntento
    }

    Escalafon {
        INT Identificacion
        NVARCHAR Nombre
        INT Marcador
        INT Ganadas
        INT Perdidas
    }

    Jugador ||--o{ Partida : juega
    Palabra ||--o{ Partida : contiene
    Partida ||--o{ Intento : tiene
    Jugador ||--o{ Escalafon : tiene
