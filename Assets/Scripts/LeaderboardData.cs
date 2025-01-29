using System;
using System.Collections.Generic;

/// <summary>
/// Representa los datos de la tabla de clasificación (Leaderboard), que contiene una lista de puntuaciones de jugadores.
/// </summary>
[Serializable]
public class LeaderboardData
{
    /// <summary>
    /// Lista de puntuaciones de jugadores que conforman la tabla de clasificación.
    /// </summary>
    public List<PlayerScore> leaderboard = new List<PlayerScore>();
}

/// <summary>
/// Representa la puntuación de un jugador, incluyendo su nombre y el puntaje obtenido.
/// </summary>
[Serializable]
public class PlayerScore
{
    /// <summary>
    /// Nombre del jugador.
    /// </summary>
    public string playerName;

    /// <summary>
    /// Puntaje obtenido por el jugador.
    /// </summary>
    public int score;

    /// <summary>
    /// Inicializa una nueva instancia de la clase <see cref="PlayerScore"/>.
    /// </summary>
    /// <param name="playerName">El nombre del jugador.</param>
    /// <param name="score">El puntaje obtenido por el jugador.</param>
    public PlayerScore(string playerName, int score)
    {
        this.playerName = playerName;
        this.score = score;
    }
}
