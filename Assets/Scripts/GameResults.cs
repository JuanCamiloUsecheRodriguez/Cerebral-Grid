using System.Collections.Generic;

/// <summary>
/// Representa los resultados de una partida del juego.
/// </summary>
[System.Serializable]
public class GameResults
{
    /// <summary>
    /// El número total de clics realizados durante la partida.
    /// </summary>
    public int total_clicks;

    /// <summary>
    /// El tiempo total que tomó completar la partida, medido en segundos.
    /// </summary>
    public float total_time;

    /// <summary>
    /// La cantidad de pares encontrados durante la partida.
    /// </summary>
    public int pairs;

    /// <summary>
    /// La puntuación total obtenida en la partida.
    /// </summary>
    public int score;
}

/// <summary>
/// Contiene una instancia de los resultados del juego para ser empaquetados.
/// </summary>
[System.Serializable]
public class ResultsWrapper
{
    /// <summary>
    /// Los resultados de la partida representados como una lista de objetos GameResults.
    /// </summary>
    public List<GameResults> results = new List<GameResults>();
}
